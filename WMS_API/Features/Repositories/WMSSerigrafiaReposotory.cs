
using Core.DTOs;
using Core.DTOs.RecepcionUbicacionCajas;
using Core.DTOs.Serigrafia;
using Core.DTOs.Serigrafia.Enums;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        public async Task<List<MateriaPrimaPorOpDTO>> GetMateriaPrimaPorOpAsync()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            List<MateriaPrimaPorOpDTO> result = await executeProcedure.ExecuteStoredProcedureList<MateriaPrimaPorOpDTO>("[IM_WMS_GetCodigoMateriaPrimaPorOps]", null);

            //List<MateriaPrimaPorOpDTO> data = new List<MateriaPrimaPorOpDTO>
            //{
            //    new MateriaPrimaPorOpDTO
            //    {
            //        ItemId = "71 15 15 947 0001"
            //    },
            //    new MateriaPrimaPorOpDTO
            //    {
            //        ItemId = "71 01 14 516 0005"
            //    }
            //};

            //if (result.Count == 0) result = data;
            return result; 
        }
        public async Task<List<ConsolidadoOpsPorColorDTO>> GetConsolidadoOpsPorColorAsync(string ItemId)
        {
            List<OpPorBaseDTO> data = await GetOpsPorBaseAsync(ItemId);
            var consolidado = data
                .GroupBy(op => op.INVENTCOLORID)
                .Select(g => new ConsolidadoOpsPorColorDTO
                {
                    INVENTCOLORID = g.Key,
                    OpsIds = g.Select(op => op.ProdMasterId).ToList(),
                    Tallas = g
                        .SelectMany(op => op.Tallas)
                        .GroupBy(t => t.Talla)
                        .Select(tg => new TallaOpDTO
                        {
                            Talla = tg.Key,
                            CantidadSolicitada = tg.Sum(x => x.CantidadSolicitada),
                            CantidadPreparada = tg.Sum(x => x.CantidadPreparada)
                        })
                        .ToList()
                })
            .ToList();
            return consolidado;
            //ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            //var parametro = new SqlParameter("@ItemId",ItemId);
            //var resultado = executeProcedure.ExecuteStoredProcedureList<OpPorBaseDTO>
        }

        public async Task<List<OpPorBaseDTO>> GetOpsPorBaseAsync(string ItemId)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@ItemId", ItemId),
            };
            var listPlanaOpsContalla = await executeProcedure
                .ExecuteStoredProcedureList<ConsultaPlanaOpsContallaDTO>("[IM_WMS_GetOpsConTallasAx]", parametros);

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
                        Tallas = new List<TallaOpDTO>()
                    };
                    resultadoDict[key] = dto;
                }

                dto.Tallas.Add(new TallaOpDTO
                {
                    Talla = op.Talla,
                    CantidadSolicitada = Convert.ToInt32(op.CantidadSolicitada),
                    CantidadPreparada = Convert.ToInt32(op.CantidadPreparada)
                    
                });
            }

            return resultadoDict.Values.ToList();
            //List<OpPorBaseDTO> data = new List<OpPorBaseDTO>
            //{
            //    new OpPorBaseDTO
            //    {
            //        ProdMasterId = "OP-00040855 001",
            //        ItemIdEstilo = "35 121 0114 516 0694",
            //        INVENTCOLORID = "09",
            //        Tallas = new List<TallaOpDTO>
            //        {
            //            new TallaOpDTO { Talla = "S", CantidadSolicitada = 481, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "M", CantidadSolicitada = 559, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "L", CantidadSolicitada = 414, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "XL", CantidadSolicitada = 281, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "XS", CantidadSolicitada = 227, CantidadPreparada = 0 }
            //        }
            //    },
            //    new OpPorBaseDTO
            //    {
            //        ProdMasterId = "OP-00040856 001",
            //        ItemIdEstilo = "35 121 0114 516 0693",
            //        INVENTCOLORID = "9I",
            //        Tallas = new List<TallaOpDTO>
            //        {
            //            new TallaOpDTO { Talla = "XS", CantidadSolicitada = 199, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "M", CantidadSolicitada = 462, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "S", CantidadSolicitada = 398, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "L", CantidadSolicitada = 352, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "XL", CantidadSolicitada = 237, CantidadPreparada = 0 }
            //        }
            //    },
            //    new OpPorBaseDTO
            //    {
            //        ProdMasterId = "OP-00040857 001",
            //        ItemIdEstilo = "35 121 0114 516 0692",
            //        INVENTCOLORID = "55",
            //        Tallas = new List<TallaOpDTO>
            //        {
            //            new TallaOpDTO { Talla = "XL", CantidadSolicitada = 227, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "S", CantidadSolicitada = 378, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "L", CantidadSolicitada = 345, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "XS", CantidadSolicitada = 183, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "M", CantidadSolicitada = 455, CantidadPreparada = 0 }
            //        }
            //    },
            //    new OpPorBaseDTO
            //    {
            //        ProdMasterId = "OP-00040859 001",
            //        ItemIdEstilo = "35 121 0114 516 0695",
            //        INVENTCOLORID = "SL",
            //        Tallas = new List<TallaOpDTO>
            //        {
            //            new TallaOpDTO { Talla = "XS", CantidadSolicitada = 170, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "XL", CantidadSolicitada = 204, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "L", CantidadSolicitada = 325, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "S", CantidadSolicitada = 362, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "M", CantidadSolicitada = 435, CantidadPreparada = 0 }
            //        }
            //    },
            //    // 👉 Nueva OP con colorId repetido ("09") pero cantidades diferentes
            //    new OpPorBaseDTO
            //    {
            //        ProdMasterId = "OP-00040860 001",
            //        ItemIdEstilo = "35 121 0114 516 0700",
            //        INVENTCOLORID = "09",
            //        Tallas = new List<TallaOpDTO>
            //        {
            //            new TallaOpDTO { Talla = "XS", CantidadSolicitada = 300, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "S", CantidadSolicitada = 410, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "M", CantidadSolicitada = 500, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "L", CantidadSolicitada = 290, CantidadPreparada = 0 },
            //            new TallaOpDTO { Talla = "XL", CantidadSolicitada = 260, CantidadPreparada = 0 }
            //        }
            //    }
            //};
            //return data;
        }

        public async Task<List<OpPorBaseDTO>> CreaOpsPreparadasAsync(string ItemId, ConsolidadoOpsPorColorDTO consolidadoOpsPorColorPrerarado)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var resultado = new List<OpPorBaseDTO>();
            var opsConTalla =  await GetOpsPorBaseAsync(ItemId);

            foreach (var op in consolidadoOpsPorColorPrerarado.OpsIds)
            {
                var OpOriginal = opsConTalla.FirstOrDefault(a => a.ProdMasterId == op.ToString());
                if (OpOriginal == null) return resultado;

                var tallasPreparadas = GenerarTallasPreparadas(OpOriginal, consolidadoOpsPorColorPrerarado);

                var TallasFinales = ConvertTallasToDataTable(tallasPreparadas);
                var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@ProdMasterId", op.ToString()),
                    new SqlParameter("@ItemIdBase", ItemId),
                    new SqlParameter("@INVENTCOLORID", consolidadoOpsPorColorPrerarado.INVENTCOLORID),
                    new SqlParameter("@EstadoOp",EstadoOp.PREPARADO),
                    new SqlParameter("@Tallas",TallasFinales)
                };


                //resultado = await executeProcedure.ExecuteStoredProcedureList<OpPorBaseDTO>("[IM_WMS_InsertOpPreparadaEncabezado]", parameter);
               var excuteResultado = await executeProcedure.ExecuteStoredProcedureJson<OpPorBaseDTO>("[IM_WMS_InsertOpPreparada]", parameter);
               resultado.Add(excuteResultado);
            }

            return resultado;
        }

        private List<TallaOpDTO> GenerarTallasPreparadas(OpPorBaseDTO opOriginal,ConsolidadoOpsPorColorDTO consolidadoOpsPorColorPrerarado)
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
                    CantidadPreparada = asignado
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

            foreach (var t in tallas)
            {
                table.Rows.Add(t.Talla, t.CantidadSolicitada, t.CantidadPreparada);
            }

            return table;
        }

        public async Task<List<OpPorBaseDTO>> GetOpsPrepardasAsync(string ItemId)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parameter = new List<SqlParameter>
            {
                new SqlParameter("@ItemIdBase",ItemId)
            };

            var resultado = await executeProcedure.ExecuteStoredProcedureJson<List<OpPorBaseDTO>>("[IM_WMS_GetOPsPreparas]", parameter);
            return resultado;
        }
    }
}
