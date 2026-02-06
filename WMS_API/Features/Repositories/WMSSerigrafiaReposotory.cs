
using Azure.Core;
using Core.DTOs;
using Core.DTOs.RecepcionUbicacionCajas;
using Core.DTOs.Serigrafia;
using Core.DTOs.Serigrafia.ClaseRespuesta;
using Core.DTOs.Serigrafia.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Style;
using OfficeOpenXml.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class WMSSerigrafiaReposotory : IWMSSerigrafiaRepository
    {
        private readonly string _connectionString;


        public WMSSerigrafiaReposotory(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzasDev");
        }
        public async Task<List<ConsultaLote>> GetLotesAsync()
        {
            var result = new List<ConsultaLote>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

                result = await executeProcedure.ExecuteStoredProcedureList<ConsultaLote>("[IM_WMS_SRG_GetLotesRecientes]", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public async Task<List<MateriaPrimaPorOpDTO>> GetMateriaPrimaPorOpAsync(string lote)
        {
            var result = new List<MateriaPrimaPorOpDTO>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);


                var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@Lote",lote),
                };

                result = await executeProcedure.ExecuteStoredProcedureList<MateriaPrimaPorOpDTO>("[IM_WMS_SRG_GetCodigoMateriaPrimaPorOp]", parameter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public async Task<List<OpPorBaseDTO>> GetOpsPorBaseAsync(string ItemId, string lote)
        {
            var resultado = new List<OpPorBaseDTO>();
            try
            {

                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@ItemId", ItemId),
                    new SqlParameter("@Lote", lote)
                };
                var listPlanaOpsContalla = await executeProcedure
                    .ExecuteStoredProcedureList<ConsultaPlanaOpsContallaDTO>("[IM_WMS_SRG_GetOpsConTallasAx_V2]", parametros);

                var resultadoDict = new Dictionary<(string ProdMasterId, string ItemId), OpPorBaseDTO>();

                foreach (var op in listPlanaOpsContalla)
                {
                    var key = (op.ProdMasterId, op.ItemIdEstilo);
                    if (!resultadoDict.TryGetValue(key, out var dto))
                    {
                        dto = new OpPorBaseDTO
                        {
                            ProdMasterId = op.ProdMasterId,
                            ItemIdEstilo = op.ItemIdEstilo,
                            INVENTCOLORID = op.INVENTCOLORID,
                            EstadoOp = op.EstadoOp,
                            ColorName = op.ColorName,
                            Tallas = new List<TallaOpDTO>()
                        };
                        resultadoDict[key] = dto;
                    }

                    dto.Tallas.Add(new TallaOpDTO
                    {
                        Talla = op.Talla,
                        CantidadSolicitada = Convert.ToInt32(op.CantidadSolicitada),
                        CantidadPreparada = Convert.ToInt32(op.CantidadPreparada),
                        CantidadEmpacada = Convert.ToInt32(op.CantidadEmpacada),
                        EstadoOP = op.EstadoOp

                    });
                }
                resultado = resultadoDict.Values.ToList();
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL en GetOpsPorBaseAsync: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado en GetOpsPorBaseAsync: {ex.Message}");
            }


            return resultado;

        }

        public async Task<string> CreaOpsPreparadasAsync(string ItemId, string lote, ConsolidadoOpsPorColorDTO consolidadoOpsPorColorPrerarado)
        {
            var resultado = new List<OpPorBaseDTO>();
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var opsConTalla = await GetOpsPorBaseAsync(ItemId, lote);
            try
            {
                foreach (var op in consolidadoOpsPorColorPrerarado.OpsIds)
                {
                    var OpOriginal = opsConTalla.FirstOrDefault(a => a.ProdMasterId == op.ToString());
                    if (OpOriginal == null) return "Error: no se encontro la Op en base de datos local";

                    var tallasPreparadas = GenerarTallasPreparadas(OpOriginal, consolidadoOpsPorColorPrerarado);

                    //var TallasFinales = ConvertTallasToDataTable(tallasPreparadas);
                    var parameter = new List<SqlParameter>
                    {
                        new SqlParameter("@ProdMasterId", op.ToString()),
                        new SqlParameter("@ItemIdBase", ItemId),
                        new SqlParameter("@ItemIdEstiloInput",""),
                        new SqlParameter("@INVENTCOLORID", consolidadoOpsPorColorPrerarado.INVENTCOLORID),
                        new SqlParameter("@EstadoOp",EstadoOp.Liberado),
                    };

                    var excuteResultado = await executeProcedure.ExecuteStoredProcedureList<OpPorBaseDTO>("[IM_WMS_SRG_GestioOpBaseLocal]", parameter);

                    if (excuteResultado == null || excuteResultado.Count == 0)
                    {
                        return "Error al ingresar el encabezado";
                    }
                    foreach(var talla in tallasPreparadas)
                    {
                        if (talla == null) continue;
                        var tallaParams = new List<SqlParameter>
                        {
                            new SqlParameter("@ProdMasterId",op),
                            new SqlParameter("@Talla", talla.Talla ?? string.Empty),
                            new SqlParameter("@CantidadSolicitada", talla.CantidadSolicitada),
                            new SqlParameter("@CantidadPreparada", talla.CantidadPreparada),
                            new SqlParameter("@EstadoOP", talla.EstadoOP)
                        };
                        try
                        {
                            var resultadoTallas = await executeProcedure.ExecuteStoredProcedureList<TallaOpDTO>("IM_WMS_SRG_UpsertTallaOpPorBase", tallaParams);
                            if (resultadoTallas == null && resultadoTallas.Count == 0)
                            {
                                return "Error en Datos de Tallas enviados";
                            }
                        }
                        catch (Exception error)
                        {
                            return "Error al insertar lineas en base de datos" + error;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Ok";
        }

        private List<TallaOpDTO> GenerarTallasPreparadas(OpPorBaseDTO opOriginal, ConsolidadoOpsPorColorDTO consolidadoOpsPorColorPrerarado)
        {
            var tallasPreparadas = new List<TallaOpDTO>();

            foreach (var tallaOp in opOriginal.Tallas)
            {
                var stock = consolidadoOpsPorColorPrerarado.Tallas.FirstOrDefault(t => t.Talla == tallaOp.Talla);
                int disponible = stock?.CantidadPreparada ?? 0;

                int asignado = Math.Min(tallaOp.CantidadSolicitada, disponible);

                tallasPreparadas.Add(new TallaOpDTO
                {
                    Talla = tallaOp.Talla,
                    CantidadSolicitada = tallaOp.CantidadSolicitada,
                    CantidadPreparada = asignado,
                    EstadoOP = tallaOp.EstadoOP,
                });

                if (stock != null)
                    stock.CantidadPreparada -= asignado;
            }

            return tallasPreparadas;
        }

        private DataTable ConvertTallasToDataTable(List<TallaOpDTO> tallas)
        {
            var table = new DataTable();
            table.Columns.Add("Talla", typeof(string));
            table.Columns.Add("CantidadSolicitada", typeof(int));
            table.Columns.Add("CantidadPreparada", typeof(int));
            table.Columns.Add("EstadoOp", typeof(int));

            foreach (var t in tallas)
            {
                table.Rows.Add(t.Talla, t.CantidadSolicitada, t.CantidadPreparada, t.EstadoOP);
            }

            return table;
        }

        public async Task<List<OpPorBaseDTO>> GetOpsPrepardasAsync(string ItemId)
        {
            var resultado = new List<OpPorBaseDTO>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@ItemIdBase",ItemId)
                };

                resultado = await executeProcedure.ExecuteStoredProcedureJson<List<OpPorBaseDTO>>("[IM_WMS_SRG_GetOPsPreparas]", parameter);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: al obtener las Op Preparadas", ex.Message);
            }
            return resultado;
        }


        public async Task<bool> ExisteOpEnBaseLocal(string orden)
        {
            bool resultado = false;
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@Orden",orden)
                };

                resultado = await executeProcedure.ExecuteStoredProcedureJson<bool>("[IM_WMS_SRG_ExiteOpLocalmente]", parameter);
            }
            catch (Exception ex)
            {
                resultado = true;
                Console.WriteLine(ex.Message);
            }

            return resultado;
        }

        public async Task<string> GestionarOPBaseLocal(OpPorBaseDTO opPorBaseDTO, string ItemBase, int stToUpdate)
        {

            try
            {
                List<ConsultaPlanaOpsContallaDTO> OpActual = await GetOPActualizadaAX(opPorBaseDTO.ProdMasterId);
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
 
            var tallasActualizadas = OpActual.Select(t => new TallaOpDTO
            {
                Talla = t.Talla,
                CantidadSolicitada = t.CantidadSolicitada,
                CantidadPreparada = t.CantidadSolicitada,
                EstadoOP = t.EstadoOp
            }).ToList();


            var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@ProdMasterId", opPorBaseDTO.ProdMasterId),
                    new SqlParameter("@ItemIdBase", ItemBase),
                    new SqlParameter("@ItemIdEstiloInput", opPorBaseDTO.ItemIdEstilo),
                    new SqlParameter("@INVENTCOLORID", opPorBaseDTO.INVENTCOLORID),
                    new SqlParameter("@EstadoOp", stToUpdate),
                };



                var excuteResultado = await executeProcedure.ExecuteStoredProcedureList<OpPorBaseDTO>("[IM_WMS_SRG_GestioOpBaseLocal]", parameter);

                if (excuteResultado == null || excuteResultado.Count == 0)
                {
                    return "Error al ingresar el encabezado";
                }
                foreach (var talla in tallasActualizadas)
                {
                    if (talla == null) continue;
                    var tallaParams = new List<SqlParameter>
                        {
                            new SqlParameter("@ProdMasterId",opPorBaseDTO.ProdMasterId),
                            new SqlParameter("@Talla", talla.Talla ?? string.Empty),
                            new SqlParameter("@CantidadSolicitada", talla.CantidadSolicitada),
                            new SqlParameter("@CantidadPreparada", talla.CantidadPreparada),
                            new SqlParameter("@EstadoOP", talla.EstadoOP)
                        };
                    try
                    {
                        var resultadoTallas = await executeProcedure.ExecuteStoredProcedureList<TallaOpDTO>("IM_WMS_SRG_UpsertTallaOpPorBase", tallaParams);
                        if (resultadoTallas == null && resultadoTallas.Count == 0)
                        {
                            return "Error en Datos de Tallas enviados";
                        }
                    }
                    catch (Exception error)
                    {
                        return "Error al insertar lineas en base de datos" + error;
                    }

                }
            } catch (Exception ex)
            {
                return ex.Message;
            }
 
            return "OK";
        }

        public async Task<List<ConsultaPlanaOpsContallaDTO>> GetOPActualizadaAX(string orden)
        {
            List<ConsultaPlanaOpsContallaDTO> resultado = new List<ConsultaPlanaOpsContallaDTO>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@ProdMasterTable",orden)
                };
                resultado = await executeProcedure.ExecuteStoredProcedureList<ConsultaPlanaOpsContallaDTO>("[IM_WMS_SRG_GetOpActualizadaAX]", parameter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return resultado;
        }

        public async Task<List<ArticulosDisponiblesTraslado>> GetArticulosPisponiblesParaTraslado(string loteId)
        {
            var result = new List<ArticulosDisponiblesTraslado>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

                var param = new List<SqlParameter>
                {
                    new SqlParameter("@LoteId",loteId)
                };

                result = await executeProcedure.ExecuteStoredProcedureList<ArticulosDisponiblesTraslado>("[IM_WMS_SRG_GetArticulosTerminadoParaTraslado]", param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }


        public async Task<List<ArticulosDisponiblesTraslado>> GetDetallaArticuloATrasladar(string ItemId)
        {
            var result = new List<ArticulosDisponiblesTraslado>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@ItemId",ItemId)
                };

                result = await executeProcedure.ExecuteStoredProcedureList<ArticulosDisponiblesTraslado>("[IM_WMS_SRG_GetDetalleArticuloPorTrasladar]", parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public async Task<List<LineasTrasladoDTO>> GetLineasDeTraslado(string ItemId)
        {
            var resutl = new List<LineasTrasladoDTO>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@ItemId", ItemId)
                };

                resutl = await executeProcedure.ExecuteStoredProcedureList<LineasTrasladoDTO>("[IM_WMS_SRG_GetDetalleArticuloPorTrasladar]", parameter);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return resutl;
        }

        public async Task<string> CrearTrasladoBaseLocal(TrasladoDTO trasladoDTO, string transferId)
        {
            var resutlEncabezado = new List<TrasladoDTO>();
            var resutlLineas = new List<LineasTrasladoDTO>();
            try
            {
                ExecuteProcedure executeProcedureEncabezado = new ExecuteProcedure(_connectionString);
                var parameterEncabezado = new List<SqlParameter>
                {
                    new SqlParameter("@TrasladoId", transferId),
                    new SqlParameter("@AlmacenSalida", trasladoDTO.AlmacenDeSalida),
                    new SqlParameter("@AlmacenEntrada", trasladoDTO.AlmacenDeEntrada)
                };

                // Corregido: resutlEncabezado es una lista, no un string
                resutlEncabezado = await executeProcedureEncabezado.ExecuteStoredProcedureList<TrasladoDTO>("[IM_WMS_SRG_CrearTrasladoEncabezado]", parameterEncabezado);

                foreach (var linea in trasladoDTO.Lineas)
                {
                    ExecuteProcedure executeProcedureLineas = new ExecuteProcedure(_connectionString);
                    var parameterLineas = new List<SqlParameter>
                    {
                        new SqlParameter("@ItemId", linea.ItemId),
                        new SqlParameter("@TrasladoId", transferId),
                        new SqlParameter("@ColorId", linea.Color),
                        new SqlParameter("@LoteId", linea.LoteId),
                        new SqlParameter("@LocationId", linea.LocationId),
                        new SqlParameter("@CantidadDeTranferencia", linea.CantidadDisponible),
                        new SqlParameter("@SizeId", linea.SizeId),
                    };

                    resutlLineas = await executeProcedureLineas.ExecuteStoredProcedureList<LineasTrasladoDTO>("[IM_WMS_SRG_CrearTrasladoLineas]", parameterLineas);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Error al registrar traslado en base local";
            }
            // Devuelve un string como resultado final

            var resultadoNoNullos = resutlEncabezado != null && resutlLineas != null;
            var hayRegistros = resutlEncabezado.Count > 0 && resutlLineas.Count > 0;
            if (!resultadoNoNullos && !hayRegistros)
            {

                return "Error al registrar traslado en base local: resultado Nullo";
            }
            return "OK";
        }

        public async Task<List<TrasladoDespachoDTO>> GetTrasladoPorLote(string LoteId)
        {
            var resutl = new List<TrasladoDespachoDTO>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@LoteId", LoteId)
                };

                resutl = await executeProcedure.ExecuteStoredProcedureList<TrasladoDespachoDTO>("[IM_WMS_SRG_GetTrasladosPorLote]", parameter);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return resutl;
        }

        public async Task<List<DiariosAbiertosDTO>> GetDiariosAbiertosAsync(string userId, string diarioId)
        {
            var result = new List<DiariosAbiertosDTO>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var param = new List<SqlParameter>
                {

                    new SqlParameter("@UserId",userId),
                    new SqlParameter("@DiarioId", diarioId)
                };
                result = await executeProcedure.ExecuteStoredProcedureList<DiariosAbiertosDTO>("[IM_WMS_SRG_GetDiariosAbiertos]", param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public async Task<List<IM_WMS_SRG_TipoDiario>> GetTiposDiario()
        {
            var result = new List<IM_WMS_SRG_TipoDiario>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);


                result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_SRG_TipoDiario>("[IM_WMS_SRG_GetTipoDiarios]", null);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;


        }

        public async Task<string> CrearDespacho(IM_WMS_SRG_Despacho despacho)
        {
            int despachoId = 0;
            try
            {
               //Crear Header
               ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
               var parameterHeader = new List<SqlParameter>
                 {
                      new SqlParameter("@Driver", despacho.Driver),
                      new SqlParameter("@Truck", despacho.Truck),
                      new SqlParameter("@StatusId", despacho.StatusId),
                      new SqlParameter("@CreatedBy", despacho.CreatedBy),
                      new SqlParameter("@Store", despacho.Store)
                 };
                var resultadoHeader = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_SRG_Despacho>("[IM_WMS_SRG_CreateDespachoHeader]", parameterHeader);


                //CrearLineas
                if (resultadoHeader.Count == 0)
                {
                    return "Error al crear el despacho";
                }

                 despachoId = resultadoHeader[0].Id;
                foreach (var linea in despacho.Traslados)
                {
                    var parameterDespachoTraslado = new List<SqlParameter>
                    {
                        new SqlParameter("@DespachoId", despachoId),
                        new SqlParameter("@TransferId", linea.TransferId),
                        new SqlParameter("@InventLocationIdFrom", linea.InventLocationIdFrom),
                        new SqlParameter("@InventLocationIdTo", linea.InventLocationIdTo), 
                        new SqlParameter("@ItemId", linea.ItemId),
                        new SqlParameter("@MontoTraslado", linea.MontoTraslado),

                    };
                    var parameterLineas = new List<SqlParameter>
                    {
                        new SqlParameter("@ItemId", linea.ItemId),
                        new SqlParameter("@TransferId", linea.TransferId),
                        new SqlParameter("@DespachoId", despachoId),

                    };
                    var resultadoDespachoTralados = await executeProcedure.ExecuteStoredProcedureList<TrasladoDespachoDTO>("[IM_WMS_SRG_CreateDespachoTraslado]", parameterDespachoTraslado);
                    if (resultadoDespachoTralados.Count == 0)
                    {
                        return "Error al crear las lineas de traslado del despacho";
                    }
                    var resultadoLineas = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_SRG_Despacho_Lines_Packing>("[IM_WMS_SRG_CreateDepachoLine_Picking]", parameterLineas);
                    if (resultadoLineas.Count == 0)
                    {
                        return "Error al crear las lineas del despacho";
                    }
                }

            }
            catch (Exception ex)
            { 
                return ex.Message;
            }
            return despachoId.ToString();
        }

        public async Task<List<IM_WMS_SRG_Despacho>> GetDespachosByBatchId(string batchId, int tipo)
        {
            var respuesta = new List<IM_WMS_SRG_Despacho>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parm = new List<SqlParameter>
                {
                    new SqlParameter("@tipo",tipo),
                    new SqlParameter("@BatchId",batchId)

                };

                respuesta = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_SRG_Despacho>("IM_WMS_SRG_GetDespachosByBatchId", parm);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  
            }
            return respuesta;
        }

        public async Task<List<IM_WMS_SRG_Despacho_Lines_Packing>> GetDespachoLinesByIdAEnviar(string despachoId)
        {
            var respuesta = new List<IM_WMS_SRG_Despacho_Lines_Packing>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@DespachoId",despachoId),

                };
                respuesta = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_SRG_Despacho_Lines_Packing>("[IM_WMS_SRG_GetDespachoLinesByIdAEnviar]", parameter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return respuesta;
        }


        public async Task<List<IM_WMS_SRG_Despacho_Lines_Packing>> GetDespachoLinesByIdARecibir(string despachoId)
        {
            var respuesta = new List<IM_WMS_SRG_Despacho_Lines_Packing>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@DespachoId",despachoId),

                };
                respuesta = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_SRG_Despacho_Lines_Packing>("[IM_WMS_SRG_GetDespachoLinesById_Recibir]", parameter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return respuesta;
        }

        public async Task<List<TrasladoDespachoDTO>> GetDespachoTrasladosById(string despachoId)
        {
            var respuesta = new List<TrasladoDespachoDTO>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@DespachoId",despachoId),

                };
                respuesta = await executeProcedure.ExecuteStoredProcedureList<TrasladoDespachoDTO>("[IM_WMS_SRG_GetDespachoTrasladosByDespachoId]", parameter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return respuesta;
        }

        public async Task<List<PackingResponseDTO>> SetPackingAsync(PackingRequestDTO packingRequestDTO)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@DespachoId", packingRequestDTO.DespachoId),
                    new SqlParameter("@ProdMasterId", packingRequestDTO.ProdMasterId),
                    new SqlParameter("@Box", packingRequestDTO.Box),
                    new SqlParameter("@UserPacking",packingRequestDTO.UserPacking)
                };

            var response = await executeProcedure
                .ExecuteStoredProcedureList<PackingResponseDTO>("[IM_WMS_SRG_SetPacking_ByDespachoOpBox]",parameters
                );

            return response;
        }
        public async Task<List<PackingResponseDTO>> SetReceiveAsync(PackingRequestDTO packingRequestDTO)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@DespachoId", packingRequestDTO.DespachoId),
                    new SqlParameter("@ProdMasterId", packingRequestDTO.ProdMasterId),
                    new SqlParameter("@Box", packingRequestDTO.Box),
                    new SqlParameter("@UserPacking",packingRequestDTO.UserPacking)
                };

            var response = await executeProcedure
                .ExecuteStoredProcedureList<PackingResponseDTO>("[IM_WMS_SRG_SetReceive_ByDespachoOpBox]", parameters
                );

            return response;
        }


        public async Task<string> ChangeEstadoTraslado(int despachoId, int statusId)
        {

            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@DespachoId", despachoId),
                    new SqlParameter("@StatusId", statusId),
                };

                var response = await executeProcedure
                    .ExecuteStoredProcedureList<TrasladoDespachoDTO>("[IM_WMS_SRG_CambiarEstadoDeDespacho]", parameters
                    );
                if (response.Count == 0 || response == null)
                {
                    return "Error al actualizar el estado del despacho";
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public async Task<Respuesta<List<IM_WMS_SRG_ArticulosGenericosSegundas>>> GetArticulosGenericoSegundas(string itemId)
        {
            var CodigoBase = itemId.Split(" ")[3].Trim();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@CodigoBase", CodigoBase)
                };

                List<IM_WMS_SRG_ArticulosGenericosSegundas> respProcedure = await executeProcedure
                   .ExecuteStoredProcedureList<IM_WMS_SRG_ArticulosGenericosSegundas>("[IM_WMS_SRG_GetArticulosGenericoSegundas]", parameters
                   );
                if (respProcedure.Count == 0 || respProcedure == null)
                {
                    return Respuesta<List<IM_WMS_SRG_ArticulosGenericosSegundas>>.Error("Error no se encontraron articulos ");
                }
                return Respuesta<List<IM_WMS_SRG_ArticulosGenericosSegundas>>.Ok(respProcedure);
            }
            catch (Exception ex)
            {
                return Respuesta<List<IM_WMS_SRG_ArticulosGenericosSegundas>>.Error("Error no se encontraron articulos ");
            }
        }

    }
}
