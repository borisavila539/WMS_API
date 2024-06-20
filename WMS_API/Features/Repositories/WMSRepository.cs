﻿using Core.DTOs;
using Core.DTOs.Despacho_PT;
using Core.DTOs.Reduccion_Cajas;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OfficeOpenXml.Style;
using Core.DTOs.DiarioTransferir;

namespace WMS_API.Features.Repositories
{
    public class WMSRepository : IWMSRepository
    {
        private readonly string _connectionString;

        public WMSRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzas");
        }

        public async Task<LoginDTO> PostLogin(LoginDTO datos)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_Login_WMS]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@user", datos.user));
                    cmd.Parameters.Add(new SqlParameter("@pass", datos.pass));

                    var response = new LoginDTO();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = getLogin(reader);
                        }
                    }
                    return response;
                }
            }
        }
        public LoginDTO getLogin(SqlDataReader reader)
        {
            return new LoginDTO()
            {
                user = reader["user"].ToString(),
                pass = reader["pass"].ToString(),
                logeado = reader["logeado"].ToString() != "0" ? true : false,
                Almacen = Convert.ToInt32(reader["Almacen"].ToString())

            };
        }

        public async Task<List<DiariosAbiertosDTO>> GetDiariosAbiertos(string user, string filtro)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_ObtenerDiariosMovimientoAbiertos]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@user", user));
                    cmd.Parameters.Add(new SqlParameter("@filtro", filtro));

                    var response = new List<DiariosAbiertosDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetDiariosAbiertos(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public DiariosAbiertosDTO GetDiariosAbiertos(SqlDataReader reader)
        {
            return new DiariosAbiertosDTO()
            {
                JOURNALID = reader["JOURNALID"].ToString(),
                DESCRIPTION = reader["DESCRIPTION"].ToString(),
                NUMOFLINES = Convert.ToInt32(reader["NUMOFLINES"].ToString()),
                JOURNALNAMEID = reader["JOURNALNAMEID"].ToString(),
            };
        }

        public async Task<List<LineasDTO>> GetLineasDiario(string diario)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_ObtenerLineasDiario]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@JOURNALID", diario));

                    var response = new List<LineasDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetLineasDiario(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public LineasDTO GetLineasDiario(SqlDataReader reader)
        {
            return new LineasDTO()
            {
                ITEMBARCODE = reader["ITEMBARCODE"].ToString(),
                ITEMID = reader["ITEMID"].ToString(),
                INVENTCOLORID = reader["INVENTCOLORID"].ToString(),
                INVENTSIZEID = reader["INVENTSIZEID"].ToString(),
                IMBOXCODE = reader["IMBOXCODE"].ToString(),
                QTY = Convert.ToInt32(Convert.ToDecimal(reader["QTY"].ToString())),

            };
        }

        public async Task<List<EtiquetaDTO>> GetDatosEtiquetaMovimiento(string diario, string IMBoxCode)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_Movimiento_Diario_Etiqueta]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@diario", diario));
                    cmd.Parameters.Add(new SqlParameter("@IMBOXCODE", IMBoxCode));


                    var response = new List<EtiquetaDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetEtiqueta(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public EtiquetaDTO GetEtiqueta(SqlDataReader reader)
        {
            return new EtiquetaDTO()
            {
                IM_CENTRO_DE_COSTOS = reader["IM_CENTRO_DE_COSTOS"].ToString(),
                Numero_caja = Convert.ToInt32(reader["Numero_caja"].ToString()),
                JOURNALNAMEID = reader["JOURNALNAMEID"].ToString(),
                ITEMID = reader["ITEMID"].ToString(),
                INVENTSIZEID = reader["INVENTSIZEID"].ToString(),
                INVENTCOLORID = reader["INVENTCOLORID"].ToString(),
                QTY = Convert.ToInt32(reader["QTY"].ToString()),
                Solicitante = reader["Solicitante"].ToString(),
                Empacador = reader["Empacador"].ToString(),
            };
        }

        public async Task<string> GetImprimirEtiquetaMovimiento(string diario, string IMBoxCode, string PRINT)
        {
            var data = await GetDatosEtiquetaMovimiento(diario, IMBoxCode);

            //creacion de encabezado de la etiqueta
            string encabezado = "";
            encabezado += "^XA";
            encabezado += "^CF0,50";
            encabezado += "^FO280,50^FD" + diario + "^FS";
            encabezado += "^CF0,24";
            encabezado += "^FO50,115^FDFecha: " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "^FS";
            encabezado += "^FO50,140^FDSoliciante: " + data[0].Solicitante + "^FS";
            encabezado += "^FO50,165^FDCaja #" + data[0].Numero_caja + "^FS";
            encabezado += "^FO480,115^FDCentro Costos: " + data[0].IM_CENTRO_DE_COSTOS + "^FS";
            encabezado += "^FO480,165^FDTipo Diario: " + data[0].JOURNALNAMEID + "^FS";
            encabezado += "^FO50,190^GB700,3,3^FS";

            //creacion del pie de pagina de la etiqueta
            string pie = "";
            pie += "^BY2,2,100";
            pie += "^FO50,1060^BC^FD" + IMBoxCode + "^FS";

            pie += "^FO430,1060^FDEmpacador: " + data[0].Empacador + "^FS";
            pie += "^CF0,40";


            string etiqueta = encabezado;
            int totalUnidades = 0;

            var groupData = new Dictionary<string, List<EtiquetaDTO>>();

            foreach (var element in data)
            {
                totalUnidades += element.QTY;
                var key = $"{element.ITEMID}-{element.INVENTCOLORID}";

                if (!groupData.ContainsKey(key))
                {
                    groupData[key] = new List<EtiquetaDTO>();
                }

                groupData[key].Add(element);
            }

            var groupArray = groupData.Select(x => new EtiquetaGroupDTO
            {
                key = x.Key,
                items = x.Value
            }).ToArray();

            int lineaArticulo = 210;
            int total = 0;

            foreach (var element in groupArray)
            {
                etiqueta += $"^FO50,{lineaArticulo}^FD{element.items[0].ITEMID}*{element.items[0].INVENTCOLORID}^FS";
                etiqueta += $"^FO50,{lineaArticulo + 30}^FDTalla: ";
                element.items.ForEach(x =>
               {
                   for (int i = 1; i <= 5 - x.INVENTSIZEID.Length; i++)
                   {
                       etiqueta += "_";
                   }
                   etiqueta += x.INVENTSIZEID;

               });

                etiqueta += "^FS";
                etiqueta += $"^FO50,{lineaArticulo + 60}^FDQTY:  ";
                int totalLinea = 0;
                element.items.ForEach(x =>
                {
                    for (int i = 1; i <= 5 - x.QTY.ToString().Length; i++)
                    {
                        etiqueta += "_";
                    }
                    etiqueta += x.QTY;
                    total += x.QTY;
                    totalLinea += x.QTY;
                });

                etiqueta += $" ={totalLinea}^FS";


                if (lineaArticulo == 910)
                {

                    etiqueta += pie;
                    etiqueta += $"^FO430,1090^FDTotal: {total}/{totalUnidades}^FS";
                    etiqueta += "^XZ";

                    try
                    {
                        using (TcpClient client = new TcpClient(PRINT, 9100))
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);
                                stream.Write(bytes, 0, bytes.Length);

                            }

                        }

                    }
                    catch (Exception err)
                    {
                        return err.ToString();
                    }


                    etiqueta = encabezado;
                    lineaArticulo = 210;
                    total = 0;

                }
                else
                {
                    lineaArticulo += 100;
                }

            }

            if (lineaArticulo != 910)
            {
                etiqueta += pie;
                etiqueta += $"^FO430,1090^FDTotal: {total}/{totalUnidades}^FS";
                etiqueta += "^XZ";
            }



            try
            {
                using (TcpClient client = new TcpClient(PRINT, 9100))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);
                        stream.Write(bytes, 0, bytes.Length);

                    }

                }
                return "OK";
            }
            catch (Exception err)
            {
                return err.ToString();
            }
            //mandar imprimir 
            return etiqueta;


        }

        public async Task<List<ImpresoraDTO>> getImpresoras()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_Impresoras_SanBernardo]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;


                    var response = new List<ImpresoraDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetImpresoras(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public ImpresoraDTO GetImpresoras(SqlDataReader reader)
        {
            return new ImpresoraDTO()
            {
                IM_DESCRIPTION_PRINTER = reader["IM_DESCRIPTION_PRINTER"].ToString(),
                IM_IPPRINTER = reader["IM_IPPRINTER"].ToString(),
            };
        }

        public async Task<List<IM_WMS_Despacho_Tela_Detalle_AX>> GetIM_WMS_Despacho_Telas(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Despacho_Tela_Detalle_AX]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TRANSFERIDFROM", TRANSFERIDFROM));
                    cmd.Parameters.Add(new SqlParameter("@TRANSFERIDTO", TRANSFERIDTO));
                    cmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONIDTO", INVENTLOCATIONIDTO));


                    var response = new List<IM_WMS_Despacho_Tela_Detalle_AX>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetDespacho_Tela_Detalle_AX(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_Despacho_Tela_Detalle_AX GetDespacho_Tela_Detalle_AX(SqlDataReader reader)
        {
            return new IM_WMS_Despacho_Tela_Detalle_AX()
            {
                TRANSFERID = reader["TRANSFERID"].ToString(),
                INVENTSERIALID = reader["INVENTSERIALID"].ToString(),
                APVENDROLL = reader["APVENDROLL"].ToString(),
                QTYTRANSFER = Convert.ToDecimal(reader["QTYTRANSFER"].ToString()),
                NAME = reader["NAME"].ToString(),
                CONFIGID = reader["CONFIGID"].ToString(),
                INVENTBATCHID = reader["INVENTBATCHID"].ToString(),
                ITEMID = reader["ITEMID"].ToString(),
                BFPITEMNAME = reader["BFPITEMNAME"].ToString()
            };
        }

        public async Task<List<IM_WMS_Despacho_Tela_Detalle_Rollo>> Get_Despacho_Tela_Detalle_Rollo(string INVENTSERIALID, string InventTransID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Despacho_Tela_Detalle_Rollo]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@INVENTSERIALID", INVENTSERIALID));
                    cmd.Parameters.Add(new SqlParameter("@InventTransID", InventTransID));

                    var response = new List<IM_WMS_Despacho_Tela_Detalle_Rollo>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetDespacho_Tela_Detalle_Rollo(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public IM_WMS_Despacho_Tela_Detalle_Rollo GetDespacho_Tela_Detalle_Rollo(SqlDataReader reader)
        {
            return new IM_WMS_Despacho_Tela_Detalle_Rollo()
            {
                INVENTSERIALID = reader["INVENTSERIALID"].ToString(),
                Picking = Convert.ToBoolean(reader["Picking"].ToString()),
                Packing = Convert.ToBoolean(reader["Packing"].ToString()),
                Receive = Convert.ToBoolean(reader["Receive"].ToString()),

            };
        }

        public async Task<List<DespachoTelasDetalleDTO>> GetDespacho_Telas(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO, string tipo)
        {
            var response = new List<DespachoTelasDetalleDTO>();
            var AX = await GetIM_WMS_Despacho_Telas(TRANSFERIDFROM, TRANSFERIDTO, INVENTLOCATIONIDTO);

            foreach (var element in AX)
            {
                var detalle = await Get_Despacho_Tela_Detalle_Rollo(element.INVENTSERIALID, element.TRANSFERID);
                if ((tipo == "PACKING" && detalle[0].Picking) || tipo == "PICKING" || tipo == "RECEIVE")
                {

                    DespachoTelasDetalleDTO tmp = new DespachoTelasDetalleDTO();
                    tmp.TRANSFERID = element.TRANSFERID;
                    tmp.INVENTSERIALID = element.INVENTSERIALID;
                    tmp.APVENDROLL = element.APVENDROLL;
                    tmp.QTYTRANSFER = element.QTYTRANSFER;
                    tmp.NAME = element.NAME;
                    tmp.CONFIGID = element.CONFIGID;
                    tmp.INVENTBATCHID = element.INVENTBATCHID;
                    tmp.ITEMID = element.ITEMID;
                    tmp.BFPITEMNAME = element.BFPITEMNAME;
                    tmp.Picking = detalle[0].Picking;
                    tmp.Packing = detalle[0].Packing;
                    tmp.receive = detalle[0].Receive;
                    response.Add(tmp);
                }

            }

            //response = response.OrderBy(x => x.NAME).ThenBy(x => x.CONFIGID).ThenBy(x=> x.INVENTBATCHID).ThenBy(x=> x.ITEMID).ToList();

            return response;


        }

        public async Task<List<IM_WMS_Despacho_Tela_Detalle_Rollo>> GetDespacho_Tela_Picking_Packing(string INVENTSERIALID, string TIPO, string CAMION, string CHOFER, string InventTransID, string USER, int IDremision)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Despacho_Tela_Picking_Packing]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@INVENTSERIALID", INVENTSERIALID));
                    cmd.Parameters.Add(new SqlParameter("@Tipo", TIPO));
                    cmd.Parameters.Add(new SqlParameter("@Camion", CAMION));
                    cmd.Parameters.Add(new SqlParameter("@Chofer", CHOFER));
                    cmd.Parameters.Add(new SqlParameter("@InventTransID", InventTransID));
                    cmd.Parameters.Add(new SqlParameter("@User", USER));
                    cmd.Parameters.Add(new SqlParameter("@ID", IDremision));



                    var response = new List<IM_WMS_Despacho_Tela_Detalle_Rollo>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetDespacho_Tela_Detalle_Rollo(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public async Task<string> postImprimirEtiquetaRollo(List<EtiquetaRolloDTO> data)
        {
            string etiqueta = "";

            etiqueta += "^XA^CF0,22^BY3,2,100";
            etiqueta += $"^FO200,50^BC^FD{data[0].INVENTSERIALID}^FS";
            etiqueta += $"^FO50,220^FDProveedor: {data[0].APVENDROLL}^FS";
            etiqueta += $"^FO500,220^FDCantidad: {data[0].QTYTRANSFER} {(data[0].ITEMID.Substring(0, 2) == "45" ? "lb" : "yd")}^FS";
            etiqueta += $"^FO50,250^FDTela: {data[0].ITEMID}^FS";
            etiqueta += $"^FO500,250^FDColor: {data[0].COLOR}^FS";
            etiqueta += $"^FO50,280^FDLote: {data[0].INVENTBATCHID}^FS";
            etiqueta += $"^FO500,280^FDConfiguracion: {data[0].CONFIGID}^FS^XZ";

            try
            {
                using (TcpClient client = new TcpClient(data[0].PRINT, 9100))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);
                        stream.Write(bytes, 0, bytes.Length);

                    }

                }
                return "OK";
            }
            catch (Exception err)
            {
                return err.ToString();
            }
        }

        public async Task<List<IM_WMS_TrasladosAbiertos>> getTrasladosAbiertos(string INVENTLOXATIONIDTO)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_TrasladosAbiertos]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@location", INVENTLOXATIONIDTO));

                    var response = new List<IM_WMS_TrasladosAbiertos>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetTrasladosAbiertos(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_TrasladosAbiertos GetTrasladosAbiertos(SqlDataReader reader)
        {
            return new IM_WMS_TrasladosAbiertos()
            {
                TRANSFERIDFROM = reader["TRANSFERIDFROM"].ToString(),
                TRANSFERIDTO = reader["TRANSFERIDTO"].ToString(),
                INVENTLOCATIONIDTO = reader["INVENTLOCATIONIDTO"].ToString(),
                DESCRIPTION = reader["DESCRIPTION"].ToString(),
                RecID = reader["RecId"].ToString(),
            };
        }

        public async Task<List<IM_WMS_EstadoTrasladosDTO>> getEstadotraslados(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_EstadoTraslados]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@TRANSFERIDFROM", TRANSFERIDFROM));
                    cmd.Parameters.Add(new SqlParameter("@TRANSFERIDTO", TRANSFERIDTO));
                    cmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONIDTO", INVENTLOCATIONIDTO));

                    var response = new List<IM_WMS_EstadoTrasladosDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetEstadoTraslados(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_EstadoTrasladosDTO GetEstadoTraslados(SqlDataReader reader)
        {
            return new IM_WMS_EstadoTrasladosDTO()
            {
                TRANSFERID = reader["TRANSFERID"].ToString(),
                Estado = reader["Estado"].ToString(),
                QTY = Convert.ToInt32(reader["QTY"].ToString()),
                Enviado = Convert.ToInt32(reader["Enviado"].ToString()),
                Recibido = Convert.ToInt32(reader["Recibido"].ToString()),
            };
        }

        public async Task<List<EstadoTrasladoTipoDTO>> gteEstadoTrasladoTipo(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO)
        {
            var response = new List<EstadoTrasladoTipoDTO>();
            var AX = await GetIM_WMS_Despacho_Telas(TRANSFERIDFROM, TRANSFERIDTO, INVENTLOCATIONIDTO);

            bool tipo = AX[0].ITEMID.Contains("45 0");


            foreach (var element in AX)
            {
                var detalle = await Get_Despacho_Tela_Detalle_Rollo(element.INVENTSERIALID, element.TRANSFERID);
                if (tipo)
                {

                    string tipoS = "";
                    tipoS = response.Find(elemen => elemen.Tipo == element.ITEMID.Substring(0, 8))?.Tipo;

                    if (tipoS == "" || tipoS == null)
                    {
                        EstadoTrasladoTipoDTO tmp = new EstadoTrasladoTipoDTO();
                        tmp.Tipo = element.ITEMID.Substring(0, 8);
                        response.Add(tmp);
                    }

                    int index = response.FindIndex(elemen => elemen.Tipo == element.ITEMID.Substring(0, 8));
                    if (detalle[0].Picking)
                    {
                        response[index].picking++;
                    }
                    if (detalle[0].Packing)
                    {
                        response[index].Enviado++;
                    }

                    if (detalle[0].Receive)
                    {
                        response[index].Recibido++;
                    }
                    response[index].Total++;


                }
                else
                {
                    string tipoS = "";
                    tipoS = response.Find(elemen => elemen.Tipo == element.BFPITEMNAME)?.Tipo;

                    if (tipoS == "" || tipoS == null)
                    {
                        EstadoTrasladoTipoDTO tmp = new EstadoTrasladoTipoDTO();
                        tmp.Tipo = element.BFPITEMNAME;
                        response.Add(tmp);
                    }

                    int index = response.FindIndex(elemen => elemen.Tipo == element.BFPITEMNAME);

                    if (detalle[0].Picking)
                    {
                        response[index].picking++;
                    }

                    if (detalle[0].Packing)
                    {
                        response[index].Enviado++;
                    }

                    if (detalle[0].Receive)
                    {
                        response[index].Recibido++;
                    }
                    response[index].Total++;



                }

            }

            return response;
        }

        public async Task<List<CrearDespachoDTO>> GetCrearDespacho(string RecIDTraslados, string Chofer, string camion)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_CrearDespacho]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@RecIDtraslado", RecIDTraslados));
                    cmd.Parameters.Add(new SqlParameter("@chofer", Chofer));
                    cmd.Parameters.Add(new SqlParameter("@camion", camion));

                    var response = new List<CrearDespachoDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetCrearDespachos(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public CrearDespachoDTO GetCrearDespachos(SqlDataReader reader)
        {
            return new CrearDespachoDTO()
            {

                ID = Convert.ToInt32(reader["ID"].ToString()),
                RecIDTraslados = reader["RecIDTraslados"].ToString(),
                chofer = reader["chofer"].ToString(),
                camion = reader["camion"].ToString(),
                Estado = Convert.ToBoolean(reader["Estado"].ToString()),
            };
        }

        public async Task<List<CrearDespachoDTO>> GetObtenerDespachos(string RecIDTraslados)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_ObtenerDespachos]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@RecIDtraslado", RecIDTraslados));


                    var response = new List<CrearDespachoDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetCrearDespachos(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public async Task<List<CerrarDespachoDTO>> getCerrarDespacho(int id)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_CerrarDespacho]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@ID", id));


                    var response = new List<CerrarDespachoDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetCerrarDespachos(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public CerrarDespachoDTO GetCerrarDespachos(SqlDataReader reader)
        {
            return new CerrarDespachoDTO()
            {

                ID = Convert.ToInt32(reader["ID"].ToString()),
                Estado = Convert.ToBoolean(reader["Estado"].ToString()),
            };

        }

        public async Task<string> getNotaDespacho(int DespachoID, string recid, string empleado, string camion)
        {
            string despacho = "";

            despacho += @"<!DOCTYPE html>
                            <html lang='en'>
                            <head>
                              <meta charset = 'UTF-8'/>
                               <style>
                                    h3,h4,td{
                                        text-align: center;
                                    }
                                    table,tr,th,td{
                                        border: 1px solid black;
                                        border-collapse: collapse;
                                    }
                                    .container{
                                        display: flex;
                                        justify-content: space-between;
                                    }
                                    .observacion {
                                        border-style: solid;
                                        border-width: 2px;
                                    }
                                </style>
                            </head>

                            <body>
                              <h3> Nota de Despacho</h3>
   
                                 <h4> Despacho #";
            //colocar el numero de despacho
            despacho += DespachoID.ToString().PadLeft(8, '0') + "</h4>";

            //colocar detalle del encabezado
            var encabezado = await getEncabezadoDespacho(empleado, recid);
            despacho += @"
                     <p>
                        <strong>Fecha: </strong>"+encabezado[0].fecha.ToString("dd/MM/yyyy HH:mm:ss") + @" <br>
  	                    <strong>Motorista: </strong>" + encabezado[0].Motorista +" / "+camion+ @" <br>
                        <strong>Traslado Inicial: </strong>" + encabezado[0].TRANSFERIDFROM + @" <br>
                        <strong>Traslado Final: </strong>" + encabezado[0].TRANSFERIDTO + @" <br>
                        <strong>Destino: </strong>" + encabezado[0].Destino + @" <br>
                      </p>
                   ";
            //obtener rollos que son parte del despacho
            var rollos = await getRolloDespacho(DespachoID);
            List<RollosDespachoDTO> data = new List<RollosDespachoDTO>();

            //colocar encabezado de la tabla correo
           
            string htmlCorreo = despacho + @"<table style='width: 100%'>
                               <thead> 
                                 <tr> 
                                   <th colspan = '7'> Detalle </th>  
                                  </tr>  
                                  <tr>  
                                    <th> # </th>
                                    <th> Rollo </th> 
                                    <th> Nombre Busqueda </th> 
                                    <th> Importacion </th> 
                                    <th> Color / Referencia </th> 
                                    <th> Ancho </th>  
                                    <th> Cantidad </th>  
                                    <th> Libras/Yardas </th>  
                                  </tr>  
                                  </thead>  
                                <tbody> ";
           

            
            //obtener informacionn de ax de los rollos
            foreach (var element in rollos)                
            {
                RollosDespachoDTO tmp = new RollosDespachoDTO();
                tmp.INVENTSERIALID = element.INVENTSERIALID;
                tmp.InventTransID = element.InventTransID;

                var RolloAX = await getRolloDespachoAX(tmp.InventTransID, tmp.INVENTSERIALID);

                int index = rollos.FindIndex(x => x.INVENTSERIALID == tmp.INVENTSERIALID);
                tmp.Config = RolloAX[0].Config;
                tmp.Color = RolloAX[0].Color;
                tmp.LibrasYardas = RolloAX[0].LibrasYardas;
                tmp.inventBatchId = RolloAX[0].inventBatchId;
                tmp.NameAlias = RolloAX[0].NameAlias;
                data.Add(tmp);               

            }

            var ordenar = data.Where(x => x.NameAlias != null && x.inventBatchId != null)
                            .OrderBy(x => x.inventBatchId).ThenBy(x=> x.NameAlias)
                            .Select(x => new RollosDespachoDTO{
                                Color = x.Color,
                                Config = x.Config,
                                inventBatchId = x.inventBatchId,
                                INVENTSERIALID = x.INVENTSERIALID,
                                InventTransID = x.InventTransID,
                                LibrasYardas = x.LibrasYardas,
                                NameAlias = x.NameAlias
                            });

            int cont = 1;
            decimal totalRolloLY = 0;
            foreach (var element in ordenar)
            {
                htmlCorreo += @"<tr>
                                <td>" + cont + @"</td>
                                <td>" + element.INVENTSERIALID + @"</td>
                                <td>" + element.NameAlias + @"</td>
                                <td>" + element.inventBatchId + @"</td>
                                <td>" + element.Color + @"</td>
                                <td>" + element.Config + @"</td>
                                <td>1</td>
                                <td>" + element.LibrasYardas + @"</td>      
                            </tr>";
                cont++;
                totalRolloLY += Convert.ToDecimal(element.LibrasYardas);
            }

            htmlCorreo += @"</tbody>
                            <tfoot>
                                <tr>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td>Total</td>
                                    <td>" + totalRolloLY + @"</td>      
                             </tr>
                            </tfoot>
                        </table></body></html>";

            //agrupar por color y configuracion la catnidad de rollos
            var resumen = data
                    .GroupBy(x => new { x.Color, x.Config })
                    .Select(g => new 
                    {
                        Color = g.Key.Color,
                        Config = g.Key.Config,
                        Cantidad = g.Sum(x => 1),
                        Total = g.Sum(a => Convert.ToDecimal(a.LibrasYardas))

                    });

            //colocar encabezado de la tabla
            despacho += @"<table style='width: 100%'>
                               <thead> 
                                 <tr> 
                                   <th colspan = '4'> Detalle </th>  
                                  </tr>  
                                  <tr>  
                                    <th> Color/Referencia </th>  
                                    <th> Ancho </th>  
                                    <th> Cantidad </th>  
                                    <th> Libras/Yardas </th>  
                                  </tr>  
                                  </thead>  
                                <tbody> ";
            int totalRollo = 0;
            decimal totalLY = 0;
            foreach(var element in resumen)
            {
                totalRollo += element.Cantidad;
                totalLY += element.Total;
                despacho += @"<tr>
                              <td>"+element.Color+@"</td>
                              <td>"+element.Config+@"</td>
                              <td>"+element.Cantidad+ @"</td>
                              <td>"+element.Total + @"</td>      
                            </tr>";
            }

            //linea que muestra el total de rollos enviados
            despacho += @"</tbody>
                            <tfoot>
                                <tr>
                                  <td></td>
                                  <td>Total</td>
                                  <td>" + totalRollo + @"</td>
                                  <td>"+totalLY+ @"</td>      
                             </tr>
                            </tfoot>
                        </table>";


           

            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress("sistema@intermoda.com.hn");

                var correos = await getCorreosDespacho();

                foreach( IM_WMS_Correos_Despacho correo in correos)
                {
                    mail.To.Add(correo.Correo);
                }
                mail.Subject = "Despacho No."+ DespachoID.ToString().PadLeft(8, '0');
                mail.IsBodyHtml = true;

                mail.Body = htmlCorreo;

                SmtpClient oSmtpClient = new SmtpClient();

                oSmtpClient.Host = "smtp.office365.com";
                oSmtpClient.Port = 587;
                oSmtpClient.EnableSsl = true;
                oSmtpClient.UseDefaultCredentials = false;

                NetworkCredential userCredential = new NetworkCredential("sistema@intermoda.com.hn", "1nT3rM0d@.Syt3ma1l");

                oSmtpClient.Credentials = userCredential;

                oSmtpClient.Send(mail);
                oSmtpClient.Dispose();


            }
            catch(Exception err)
            {

            }


            //colocar la tabla

            despacho += @"<p>
      	                    <strong>Observaciones:</strong>
      	                    <div class='observacion' >
                              <br><br><br>
                            </div>
                        </p>
                         <br>
                        <br>
                        <br>
                        <div class='container'>
                            <div>
                                <p>____________________</p>
                                <p align = 'center'> Despachado </p>
                            </div>
                            <div>
                                <p> ____________________ </p>
                                <p align='center'>Motorista</p>
                            </div>
                            <div>
                                <p>____________________</p>
                                <p align = 'center'> Recibe </p>
                            </div>
                        </div>
                    </body>
                    </html> ";

            

           return despacho;
        }

        public async Task<List<EncabezadoNotaDespachoDTO>> getEncabezadoDespacho(string empleado, string recid)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Encabezado_Despacho]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@empleado", empleado));
                    cmd.Parameters.Add(new SqlParameter("@recid", recid));



                    var response = new List<EncabezadoNotaDespachoDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getEncabezadoDespachos(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public EncabezadoNotaDespachoDTO getEncabezadoDespachos(SqlDataReader reader)
        {
            return new EncabezadoNotaDespachoDTO()
            {

                fecha = Convert.ToDateTime(reader["Fecha"].ToString()),
                Motorista = reader["Motorista"].ToString(),
                TRANSFERIDFROM = reader["TRANSFERIDFROM"].ToString(),
                TRANSFERIDTO = reader["TRANSFERIDTO"].ToString(),
                Destino = reader["Destino"].ToString(),
            };

        }
        public async Task<List<RollosDespachoDTO>> getRolloDespacho(int id)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_RolloDespacho]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@ID", id));
                    



                    var response = new List<RollosDespachoDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getRollosDespacho(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public RollosDespachoDTO getRollosDespacho(SqlDataReader reader)
        {
            return new RollosDespachoDTO()
            {                
                INVENTSERIALID = reader["INVENTSERIALID"].ToString(),
                InventTransID = reader["InventTransID"].ToString()
            };

        }
        public async Task<List<RollosDespachoDTO>> getRolloDespachoAX(string InventTransID,string INVENTSERIALID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_NotaDespachoDetalleAX]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@TRANSFERID", InventTransID));
                    cmd.Parameters.Add(new SqlParameter("@INVENTSERIALID", INVENTSERIALID));

                    var response = new List<RollosDespachoDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getRollosDespachoAX(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public RollosDespachoDTO getRollosDespachoAX(SqlDataReader reader)
        {
            return new RollosDespachoDTO()
            {
                INVENTSERIALID = reader["INVENTSERIALID"].ToString(),
                Color = reader["Color"].ToString(),
                Config = reader["CONFIGID"].ToString(),
                LibrasYardas = reader["LibrasYardas"].ToString(),
                NameAlias = reader["NameAlias"].ToString(),
                inventBatchId = reader["inventBatchId"].ToString(),

            };

        }

        public async Task<List<IM_WMS_Correos_Despacho>> getCorreosDespacho()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_ObtenerCorreosDespachotela]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
               
                    var response = new List<IM_WMS_Correos_Despacho>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getCorreosDespachos(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public IM_WMS_Correos_Despacho getCorreosDespachos(SqlDataReader reader)
        {
            return new IM_WMS_Correos_Despacho()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                Correo = reader["Correo"].ToString()
            };

        }

        public async Task<List<RolloDespachoDTO>> getRollosDespacho(int despachoID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_ObtenerRollosDespacho]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DespachoId", despachoID));

                    var response = new List<RolloDespachoDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getListaRollosDespacho(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public RolloDespachoDTO getListaRollosDespacho(SqlDataReader reader)
        {
            return new RolloDespachoDTO()
            {   ID = Convert.ToInt32(reader["ID"].ToString()),
                INVENTSERIALID = reader["INVENTSERIALID"].ToString()
            };

        }

        public async Task<List<LineasDTO>> GetLineasReducionCajas(string IMBOXCODE)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_ObtenerReduccionCajas]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@IMBOXCODE", IMBOXCODE));

                    var response = new List<LineasDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetLineasDiario(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public async Task<string> getImprimirEtiquetaReduccion(string IMBOXCODE, string ubicacion, string empacador, string PRINT)
        {
            DateTime hoy = DateTime.Now;

            var empleado = await getNombreEmpleado(empacador);
            var data = await GetDatosEtiquetaReduccion(IMBOXCODE);

            var groupData = new Dictionary<string, List<EtiquetaReduccionDTO>>();
            int totalUnidades = 0;

            foreach (var element in data)
            {
                totalUnidades += element.QTY;
                var key = $"{element.ITEMID}-{element.INVENTCOLORID}";

                if (!groupData.ContainsKey(key))
                {
                    groupData[key] = new List<EtiquetaReduccionDTO>();
                }

                groupData[key].Add(element);
            }

            var groupArray = groupData.Select(x => new EtiquetaReduccionGroupDTO
            {
                key = x.Key,
                items = x.Value
            }).ToArray();

            string encabezado = @"^XA^FO700,50^FWN^A0R,30,30^FDFecha: "+hoy+@"^FS^FO670,50^A0R,30,30^FDEmpacador: "+empleado.Nombre+@"^FS";

            string pie = @"^A0R,30,30^BY3,2,100^FO50,50^BC^FD"+IMBOXCODE+ @"^FS^FO50,700^A0R,40,40^FDUbicacion: "+ubicacion+ @"^FS^XZ";

            int cont = 0;
            string etiqueta = "";
            int position = 600;
            int subtotal = 0;

            foreach(var element in groupArray)
            {
                if(cont == 0)
                {
                    etiqueta = encabezado;
                    subtotal = 0;
                    position = 600;
                }
                if (element.items[0].NameAlias.Substring(0, 2) == "MB")
                {
                    etiqueta += $"^FO{position},50^A0R,60,60^FD{element.items[0].NameAlias} *{element.items[0].INVENTCOLORID}^FS";
                    position -= 45;
                    etiqueta += $"^FO{position},50^A0R,40,40^FD{element.items[0].ITEMID}^FS";
                    position -= 45;
                }
                else
                {
                    etiqueta += $"^FO{position},50^A0R,60,60^FD{element.items[0].ITEMID} *{element.items[0].INVENTCOLORID}^FS";
                    position -= 45;
                }
                
                etiqueta += $"^FO{position},50^A0R,40,40^FDTalla: ";

                element.items.ForEach( x=>
                {
                    for(int i = 1; i <= 5 - x.INVENTSIZEID.Length; i++)
                    {
                        etiqueta += "_";
                        
                    }
                    etiqueta += x.INVENTSIZEID;
                });
                etiqueta += "^FS";
                position -= 45;
                etiqueta += $"^FO{position},50^A0R,40,40^FDQTY: ";
                int totalLinea = 0;
                element.items.ForEach(x =>
                {
                    for (int i = 1; i <= 5 - x.QTY.ToString().Length; i++)
                    {
                        etiqueta += "_";

                    }
                    etiqueta += x.QTY;
                    totalLinea += x.QTY;
                });
                etiqueta += $"={totalLinea}^FS";
                subtotal += totalLinea;
                position -= 70;
                cont++;
                if(cont== 2)
                {
                    etiqueta += $"^FO100,700^A0R,40,40^FDTotal: {subtotal} / {totalUnidades}^FS";
                    etiqueta += pie;
                    cont = 0;

                    try
                    {
                        using (TcpClient client = new TcpClient(PRINT, 9100))
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);
                                stream.Write(bytes, 0, bytes.Length);
                            }
                        }                        
                    }
                    catch (Exception err)
                    {
                        return err.ToString();
                    };
                }
                
                
            }
            if (cont != 2 && cont != 0)
            {
                etiqueta += $"^FO100,700^A0R,40,40^FDTotal: {subtotal} / {totalUnidades}^FS";
                etiqueta += pie;
                cont = 0;

                try
                {
                    using (TcpClient client = new TcpClient(PRINT, 9100))
                    {
                        using (NetworkStream stream = client.GetStream())
                        {
                            byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);
                            stream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                catch (Exception err)
                {
                    return err.ToString();
                };
            }

                return "OK";


        }

        public async Task<EmpleadoDTO> getNombreEmpleado(string empleado)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_ObtenerNombreEmpleado]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@empleado", empleado));

                    var response = new EmpleadoDTO();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = getNombreEmpleados(reader);
                        }
                    }
                    return response;
                }
            }
        }
        public EmpleadoDTO getNombreEmpleados(SqlDataReader reader)
        {
            return new EmpleadoDTO()
            {
                Nombre = reader["Nombre"].ToString(),
                Cuenta = reader["Cuenta"].ToString(),
                

            };
        }
        public async Task<List<EtiquetaReduccionDTO>> GetDatosEtiquetaReduccion(string IMBoxCode)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Obtener_Etiqueta_Reduccion]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@IMBOXCODE", IMBoxCode));


                    var response = new List<EtiquetaReduccionDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetEtiquetaReduccion(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public EtiquetaReduccionDTO GetEtiquetaReduccion(SqlDataReader reader)
        {
            return new EtiquetaReduccionDTO()
            {
                NameAlias = reader["NameAlias"].ToString(),
                ITEMID = reader["ITEMID"].ToString(),
                INVENTSIZEID = reader["INVENTSIZEID"].ToString(),
                INVENTCOLORID = reader["INVENTCOLORID"].ToString(),
                QTY = Convert.ToInt32(reader["QTY"].ToString()),                
            };
        }

        //=============================================Despacho  PT
        public async Task<List<IM_WMS_Insert_Boxes_Despacho_PT_DTO>> GetInsert_Boxes_Despacho_PT(string ProdID, string userCreated, int Box)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Insert_Boxes_Despacho_PT]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProdID", ProdID));
                    cmd.Parameters.Add(new SqlParameter("@userCreated", userCreated));
                    cmd.Parameters.Add(new SqlParameter("@Box", Box));

                    var response = new List<IM_WMS_Insert_Boxes_Despacho_PT_DTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetInsert_Boxes_Despacho_PT_Lines(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public IM_WMS_Insert_Boxes_Despacho_PT_DTO GetInsert_Boxes_Despacho_PT_Lines(SqlDataReader reader)
        {
            return new IM_WMS_Insert_Boxes_Despacho_PT_DTO()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                ProdID = reader["ProdID"].ToString(),
                ProdCutSheetID = reader["ProdCutSheetID"].ToString(),
                Size = reader["Size"].ToString(),
                Color = reader["Color"].ToString(),
                UserPicking = reader["UserPicking"].ToString(),
                ItemID = reader["ItemID"].ToString(),
                Box = Convert.ToInt32(reader["Box"].ToString()),
                QTY = Convert.ToInt32(reader["QTY"].ToString()),
            };
        }

        public async Task<List<IM_WMS_Picking_Despacho_PT_DTO>> GetPicking_Despacho_PT(int Almacen)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Picking_Despacho_PT]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Almacen", Almacen));
                   

                    var response = new List<IM_WMS_Picking_Despacho_PT_DTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetPickking_Despacho_PT_Lines(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public IM_WMS_Picking_Despacho_PT_DTO GetPickking_Despacho_PT_Lines(SqlDataReader reader)
        {
            return new IM_WMS_Picking_Despacho_PT_DTO()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                ProdID = reader["ProdID"].ToString(),                
                Size = reader["Size"].ToString(),
                Color = reader["Color"].ToString(),
                fechaPicking = Convert.ToDateTime( reader["fechaPicking"].ToString()),
                ItemID = reader["ItemID"].ToString(),
                Box = Convert.ToInt32(reader["Box"].ToString()),
                QTY = Convert.ToInt32(reader["QTY"].ToString()),
            };
        }

        public async Task<List<IM_WMS_Get_EstatusOP_PT_DTO>> get_EstatusOP_PT(int almacen)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Get_EstatusOP]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@inventlocationid", almacen));


                    var response = new List<IM_WMS_Get_EstatusOP_PT_DTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(get_EstatusOP_PTLines(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public IM_WMS_Get_EstatusOP_PT_DTO get_EstatusOP_PTLines(SqlDataReader reader)
        {
            return new IM_WMS_Get_EstatusOP_PT_DTO()
            {
                
                UserPicking = reader["UserPicking"].ToString(),
                Prodcutsheetid = reader["Prodcutsheetid"].ToString(),
                prodid = reader["prodid"].ToString(),                
                Size = reader["Size"].ToString(),
                Escaneado = Convert.ToInt32(reader["Escaneado"].ToString()),
                Cortado = Convert.ToInt32(reader["Cortado"].ToString()),
            };
        }

        public async Task<IM_WMS_Insert_Estatus_Unidades_OP_DTO> GetM_WMS_Insert_Estatus_Unidades_OPs(IM_WMS_Insert_Estatus_Unidades_OP_DTO data)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Insert_Estatus_Unidades_OP]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@prodID", data.ProdID));
                    cmd.Parameters.Add(new SqlParameter("@size", data.size));
                    cmd.Parameters.Add(new SqlParameter("@costura1", data.Costura1));
                    cmd.Parameters.Add(new SqlParameter("@textil1", data.Textil1));
                    cmd.Parameters.Add(new SqlParameter("@costura2", data.Costura2));
                    cmd.Parameters.Add(new SqlParameter("@textil2", data.Textil2));
                    cmd.Parameters.Add(new SqlParameter("@usuario", data.usuario));


                    var response = new IM_WMS_Insert_Estatus_Unidades_OP_DTO();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = GetM_WMS_Insert_Estatus_Unidades_OPsLines(reader);
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_Insert_Estatus_Unidades_OP_DTO GetM_WMS_Insert_Estatus_Unidades_OPsLines(SqlDataReader reader)
        {
            return new IM_WMS_Insert_Estatus_Unidades_OP_DTO()
            {

                ID = Convert.ToInt32(reader["ID"].ToString()),               
                ProdID = reader["ProdID"].ToString(),
                size = reader["size"].ToString(),
                Costura1 = Convert.ToInt32(reader["Costura1"].ToString()),
                Textil1 = Convert.ToInt32(reader["Textil1"].ToString()),
                Costura2 = Convert.ToInt32(reader["Costura2"].ToString()),
                Textil2 = Convert.ToInt32(reader["Textil2"].ToString())            
            };
        }

        public async Task<IM_WMS_Crear_Despacho_PT> GetCrear_Despacho_PT(string driver, string truck, string userCreated, int almacen)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Crear_Despacho_PT]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@driver", driver));
                    cmd.Parameters.Add(new SqlParameter("@truck", truck));
                    cmd.Parameters.Add(new SqlParameter("@userCreated", userCreated));
                    cmd.Parameters.Add(new SqlParameter("@almacen", almacen));  

                    var response = new IM_WMS_Crear_Despacho_PT();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = GetCrear_Despacho_PLines(reader);
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_Crear_Despacho_PT GetCrear_Despacho_PLines(SqlDataReader reader)
        {
            return new IM_WMS_Crear_Despacho_PT()
            {
                id = Convert.ToInt32(reader["ID"].ToString()),
                driver = reader["driver"].ToString(),
                truck = reader["truck"].ToString(),
                UserCreated = reader["UserCreated"].ToString(),
                almacen = Convert.ToInt32(reader["almacen"].ToString())

            };
        }

        public async Task<List<IM_WMS_Get_Despachos_PT_DTO>> Get_Despachos_PT_DTOs(string estado, int almacen, int DespachoId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Get_Despachos_PT]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Estado", estado));
                    cmd.Parameters.Add(new SqlParameter("@almacen", almacen));
                    cmd.Parameters.Add(new SqlParameter("@DespachoID", DespachoId));
                   

                    var response = new List<IM_WMS_Get_Despachos_PT_DTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(Get_Despachos_PT_Lines(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_Get_Despachos_PT_DTO Get_Despachos_PT_Lines(SqlDataReader reader)
        {
            return new IM_WMS_Get_Despachos_PT_DTO()
            {
                id = Convert.ToInt32(reader["ID"].ToString()),
                Driver = reader["driver"].ToString(),
                truck = reader["truck"].ToString(),
                CreatedDateTime =Convert.ToDateTime(reader["CreatedDateTime"].ToString()),
                

            };
        }

        public async Task<IM_WMS_Packing_DespachoPTDTO> GetPacking_DespachoPT(string ProdID, string userCreated, int Box,int DespachoID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Packing_DespachoPT]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@prodID", ProdID));
                    cmd.Parameters.Add(new SqlParameter("@box", Box));
                    cmd.Parameters.Add(new SqlParameter("@user", userCreated));
                    cmd.Parameters.Add(new SqlParameter("@DespachoID", DespachoID));



                    var response = new IM_WMS_Packing_DespachoPTDTO();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = GetPacking_DespachoPTLine(reader);
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_Packing_DespachoPTDTO GetPacking_DespachoPTLine(SqlDataReader reader)
        {
            return new IM_WMS_Packing_DespachoPTDTO()
            {
                BOX = Convert.ToInt32(reader["BOX"].ToString()),
                PRODID = reader["PRODID"].ToString(),
                Packing = Convert.ToBoolean(reader["Packing"].ToString()),
                FechaPacking = Convert.ToDateTime(reader["FechaPacking"].ToString()),
                UserPacking = reader["UserPacking"].ToString(),
                DespachoID = Convert.ToInt32(reader["DespachoID"].ToString()),
            };
        }

        public async Task<List<IM_WMS_Picking_Despacho_PT_DTO>> GetDetalleDespachoPT(int DespachoID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_ObtenerDetalleDespachoPT]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DespachoID", DespachoID));                   



                    var response = new List<IM_WMS_Picking_Despacho_PT_DTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add( GetPickking_Despacho_PT_Lines(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public async Task<List<DiariosAbiertosDTO>> getObtenerDiarioTransferir(string user, string filtro)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Obtener_Diarios_Transferir]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@user", user));
                    cmd.Parameters.Add(new SqlParameter("@filtro", filtro));

                    var response = new List<DiariosAbiertosDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetDiariosAbiertos(reader));
                        }
                    }
                    return response;
                }
            }
        }

        //obtenre informacion del detalle que se colocara en el archivo de excel Despacho PT Contratistas
        public async Task<List<IM_WMS_Detalle_Despacho_Excel>> getDetalle_Despacho_Excel(int DespachoID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Detalle_Despacho_Excel]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DespachoID", DespachoID));                 

                    var response = new List<IM_WMS_Detalle_Despacho_Excel>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getDetalle_Despacho_ExcelLines(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public IM_WMS_Detalle_Despacho_Excel getDetalle_Despacho_ExcelLines(SqlDataReader reader)
        {
            return new IM_WMS_Detalle_Despacho_Excel()
            {
                InventLocation = reader["InventLocation"].ToString(),
                Base = reader["Base"].ToString(),
                ItemID = reader["ItemID"].ToString(),
                Nombre = reader["Nombre"].ToString(),
                Color = reader["Color"].ToString(),
                Size = reader["Size"].ToString(),
                ProdID = reader["ProdID"].ToString(),
                InventRefId = reader["InventRefId"].ToString(),
                Planificado = Convert.ToInt32(reader["Planificado"].ToString()),
                Cortado = Convert.ToInt32(reader["Cortado"].ToString()),
                Primeras = Convert.ToInt32(reader["Primeras"].ToString()),
                Costura1 = Convert.ToInt32(reader["Costura1"].ToString()),
                Textil1 = Convert.ToInt32(reader["Textil1"].ToString()),          
                Costura2 = Convert.ToInt32(reader["Costura2"].ToString()),
                Textil2 = Convert.ToInt32(reader["Textil2"].ToString()),
                TotalUnidades = Convert.ToInt32(reader["TotalUnidades"].ToString()),
                DifPrdVrsPlan = Convert.ToInt32(reader["DifPrdVrsPlan"].ToString()),
                DifCortVrsExport = Convert.ToInt32(reader["DifCortVrsExport"].ToString()),
                PorCostura = Convert.ToDecimal(reader["PorCostura"].ToString()),
                PorTextil = Convert.ToDecimal(reader["PorTextil"].ToString()),
                Irregulares1PorcCostura = Convert.ToDecimal(reader["Irregulares1PorcCostura"].ToString()),
                IrregularesCobrarCostura = Convert.ToDecimal(reader["IrregularesCobrarCostura"].ToString()),
                Irregulares1PorcTextil = Convert.ToDecimal(reader["Irregulares1PorcTextil"].ToString()),
                IrregularesCobrarTextil = Convert.ToDecimal(reader["IrregularesCobrarTextil"].ToString()),
                Cajas = Convert.ToInt32(reader["Cajas"].ToString()),
                TotalDocenas = Convert.ToDecimal(reader["TotalDocenas"].ToString())               
            };
        }

        public async Task<IM_WMS_EnviarDespacho> Get_EnviarDespachos(int DespachoID,string user, int cajasSegundas, int cajasTerceras)
        {
            var response = new IM_WMS_EnviarDespacho();
            //Cambiar estado del despacho a enviado
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_EnviarDespacho]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DespachoID", DespachoID));
                    cmd.Parameters.Add(new SqlParameter("@cajasSegundas", cajasSegundas));
                    cmd.Parameters.Add(new SqlParameter("@cajasTerceras", cajasTerceras));


                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = Get_EnviarDespachosLine(reader);
                        }
                    }
                   
                }
            }

            if(response.Descripcion == "Enviado")
            {
                var Despacho = await getDetalle_Despacho_Excel(DespachoID);
              

                var almacenes = Despacho.Select(x => x.InventLocation).Distinct().ToList();
                var encabezado = await GetEncabezadoDespachoExcel(user);
                int cont1 = 0;
                foreach (var almacen in almacenes)
                {
                   
                    var data = Despacho.Where(x=> x.InventLocation == almacen).ToList();

                    if (data.Count() > 0)
                    {
                        if(cont1 == 0)
                        {
                            var tmp = new IM_WMS_Detalle_Despacho_Excel();
                            tmp.CajasSegundas = cajasSegundas;
                            tmp.CajasTerceras = cajasTerceras;
                            data.Add(tmp);
                            cont1++;
                        }
                        try
                        {
                            Byte[] fileContents;
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage package = new ExcelPackage())
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Hoja1");
                                int cont = 1;
                                int fila = 12;

                                var rangeEncabezado = worksheet.Cells[2, 1, 5, 1];
                                rangeEncabezado.Style.Font.Size = 16;
                                rangeEncabezado.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                rangeEncabezado.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                                //encabezado del libro
                               
                                for (int x = 2; x <= 5; x++)
                                {
                                    var rangeMerge = worksheet.Cells[x, 1, x, 26];
                                    rangeMerge.Merge = true;
                                    string texto = "";
                                    switch (x)
                                    {
                                        case 2:
                                            texto = encabezado.NAME;
                                            break;
                                        case 3:
                                            texto = "Direccion: " + encabezado.STREET;
                                            break;
                                        case 4:
                                            texto = "Telefono: " + encabezado.LOCATOR;
                                            break;
                                        case 5:
                                            texto = "RTN: " + encabezado.ORGNUMBER;
                                            break;
                                    }
                                    worksheet.Cells[x, 1].Value = texto;
                                }


                                worksheet.Row(fila).Height = 57;
                                var range = worksheet.Cells[12, 1, 12, 28];
                                range.Style.Font.Size = 12;
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.WrapText = true;

                                worksheet.Cells[8, 2].Value = "Cliente:";
                                worksheet.Cells[8, 3].Value = "INTERMODA";
                                worksheet.Cells[8, 6].Value = "Entrega A:";
                                worksheet.Cells[8, 7].Value = data[0].InventLocation;
                                worksheet.Cells[9, 2].Value = "Fecha:";
                                worksheet.Cells[9, 3].Value = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
                                worksheet.Cells[9, 6].Value = "Packing List: ";
                                worksheet.Cells[9, 7].Value = DespachoID.ToString().PadLeft(8, '0');

                                //Encabezado de la tabla
                                worksheet.Cells[fila, 1].Value = "#";
                                worksheet.Column(1).Width = 9.33;

                                worksheet.Cells[fila, 2].Value = "Base";
                                worksheet.Column(2).Width = 12.56;

                                worksheet.Cells[fila, 3].Value = "Codigo de Articulo";
                                worksheet.Column(3).Width = 37.11;

                                worksheet.Cells[fila, 4].Value = "Nombre Producto";
                                worksheet.Column(4).Width = 27.67;

                                worksheet.Cells[fila, 5].Value = "Nombre Color";
                                worksheet.Column(5).Width = 43.11;

                                worksheet.Cells[fila, 6].Value = "Talla";
                                worksheet.Column(6).Width = 11.67;

                                worksheet.Cells[fila, 7].Value = "Numero Orden Produccion";
                                worksheet.Column(7).Width = 38.22;

                                worksheet.Cells[fila, 8].Value = "PC";
                                worksheet.Column(8).Width = 43.11;

                                worksheet.Cells[fila, 9].Value = "Unidades Planificadas";
                                worksheet.Column(9).Width = 21.67;

                                worksheet.Cells[fila, 10].Value = "Unidades Cortadas";
                                worksheet.Column(10).Width = 15.11;

                                worksheet.Cells[fila, 11].Value = "Und. De Primeras";
                                worksheet.Column(11).Width = 12.56;

                                worksheet.Cells[fila - 1, 12].Value = "Irregulares";
                                range = worksheet.Cells[fila - 1, 12, fila - 1, 13];
                                range.Merge = true;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                worksheet.Cells[fila, 12].Value = "Costura1";
                                worksheet.Column(12).Width = 10.56;

                                worksheet.Cells[fila, 13].Value = "Textil1";
                                worksheet.Column(13).Width = 8.56;

                                worksheet.Cells[fila - 1, 14].Value = "Terceras";
                                range = worksheet.Cells[fila - 1, 14, fila - 1, 15];
                                range.Merge = true;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);


                                worksheet.Cells[fila, 14].Value = "Costura2";
                                worksheet.Column(14).Width = 11.89;

                                worksheet.Cells[fila, 15].Value = "Textil2";
                                worksheet.Column(15).Width = 9.89;

                                worksheet.Cells[fila, 16].Value = "Total de Unidades por Orden";
                                worksheet.Column(16).Width = 18.56;

                                worksheet.Cells[fila, 17].Value = "Dif Prd vrs Plan";
                                worksheet.Column(17).Width = 13.67;

                                worksheet.Cells[fila, 18].Value = "Dif Cortado vrs Exportado";
                                worksheet.Column(18).Width = 20.11;

                                worksheet.Cells[fila - 1, 19].Value = "% de Irregular y Tercera";
                                range = worksheet.Cells[fila - 1, 19, fila - 1, 20];
                                range.Merge = true;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range = worksheet.Cells[fila - 1, 19, fila - 1, 24];
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                worksheet.Cells[fila, 19].Value = "Por Costura";
                                worksheet.Column(19).Width = 14.78;

                                worksheet.Cells[fila, 20].Value = "Por Textil";
                                worksheet.Column(20).Width = 12.78;

                                worksheet.Cells[fila, 21].Value = "Irregulares Permitidos del 1% por Costura";
                                worksheet.Column(21).Width = 15.56;

                                worksheet.Cells[fila, 22].Value = "Irregulares arriba del 1% Por Costura";
                                worksheet.Column(22).Width = 15.56;

                                worksheet.Cells[fila, 23].Value = "Irregulares Permitidas del 1% por Defecto Textil";
                                worksheet.Column(23).Width = 15.56;

                                worksheet.Cells[fila, 24].Value = "Irregulares arriba del 1% por Defecto Textil";
                                worksheet.Column(24).Width = 15.56;

                                worksheet.Cells[fila, 25].Value = "Cajas de Primeras";
                                worksheet.Column(25).Width = 17.56;

                                worksheet.Cells[fila, 26].Value = "Cajas de Segundas";
                                worksheet.Column(26).Width = 17.56;

                                worksheet.Cells[fila, 27].Value = "Cajas de Terceras";
                                worksheet.Column(27).Width = 17.56;

                                worksheet.Cells[fila, 28].Value = "Total de Docenas";
                                worksheet.Column(28).Width = 14.78;


                                fila++;

                                data.ForEach(element =>
                                {
                                    worksheet.Row(fila).Height = 36;
                                    worksheet.Cells[fila, 1].Value = cont;
                                    worksheet.Cells[fila, 2].Value = element.Base;
                                    worksheet.Cells[fila, 3].Value = element.ItemID;
                                    worksheet.Cells[fila, 4].Value = element.Nombre;
                                    worksheet.Cells[fila, 5].Value = element.Color;
                                    worksheet.Cells[fila, 6].Value = element.Size;
                                    worksheet.Cells[fila, 7].Value = element.ProdID;
                                    worksheet.Cells[fila, 8].Value = element.InventRefId;
                                    worksheet.Cells[fila, 9].Value = element.Planificado;
                                    worksheet.Cells[fila, 10].Value = element.Cortado;
                                    worksheet.Cells[fila, 11].Value = element.Primeras;
                                    worksheet.Cells[fila, 12].Value = element.Costura1;
                                    worksheet.Cells[fila, 13].Value = element.Textil1;
                                    worksheet.Cells[fila, 14].Value = element.Costura2;
                                    worksheet.Cells[fila, 15].Value = element.Textil2;
                                    worksheet.Cells[fila, 16].Value = element.TotalUnidades;
                                    worksheet.Cells[fila, 17].Value = element.DifPrdVrsPlan;
                                    worksheet.Cells[fila, 18].Value = element.DifCortVrsExport;
                                    worksheet.Cells[fila, 19].Value = Math.Round(element.PorCostura, 2) / 100;
                                    worksheet.Cells[fila, 19].Style.Numberformat.Format = "0.00%";
                                    worksheet.Cells[fila, 20].Value = Math.Round(element.PorTextil, 2) / 100;
                                    worksheet.Cells[fila, 20].Style.Numberformat.Format = "0.00%";
                                    worksheet.Cells[fila, 21].Value = element.Irregulares1PorcCostura;
                                    worksheet.Cells[fila, 22].Value = element.IrregularesCobrarCostura;
                                    worksheet.Cells[fila, 23].Value = element.Irregulares1PorcTextil;
                                    worksheet.Cells[fila, 24].Value = element.IrregularesCobrarTextil;
                                    worksheet.Cells[fila, 25].Value = element.Cajas;
                                    worksheet.Cells[fila, 26].Value = element.CajasSegundas;
                                    worksheet.Cells[fila, 27].Value = element.CajasTerceras;
                                    worksheet.Cells[fila, 28].Value = element.TotalDocenas;

                                    fila++;
                                    cont++;
                                });

                                var range2 = worksheet.Cells[13, 1, fila, 28];
                                range2.Style.Font.Size = 16;
                                range2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range2.Style.WrapText = true;

                                fila--;
                                var rangeTable = worksheet.Cells[12, 1, fila, 28];
                                var table = worksheet.Tables.Add(rangeTable, "MyTable");
                                table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;

                                int col = 11;
                                for (char c = 'K'; c <= 'Z'; c++)
                                {
                                    worksheet.Cells[fila + 1, col].Formula = "sum(" + c + "13:" + c + fila + ")";
                                    col++;
                                }

                                for (char c = 'A'; c <= 'B'; c++)
                                {
                                    worksheet.Cells[fila + 1, col].Formula = "sum(A" + c + "13:A" + c + fila + ")";
                                    col++;
                                }

                                range = worksheet.Cells[fila + 1, 11, fila + 1, col - 1];
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                //pie de pagina 
                                worksheet.HeaderFooter.OddFooter.LeftAlignedText = "___________________________";

                                worksheet.HeaderFooter.OddFooter.LeftAlignedText = "___________________________\nFIRMA DEL AUDITOR INTERMODA";
                                worksheet.HeaderFooter.OddFooter.CenteredText = "___________________________\nFIRMA DEL TRANSPORTISTA";
                                worksheet.HeaderFooter.OddFooter.RightAlignedText = "___________________________\nFIRMA DE CONTROL DE INVENTARIO";
                                fila += 3;

                                range = worksheet.Cells[fila, 26, fila, 27];
                                range.Merge = true;
                                worksheet.Cells[fila, 26].Value = "Total Cajas Primera";
                                worksheet.Cells[fila, 28].Value = data.Sum(x => x.Cajas);

                                fila++;
                                range = worksheet.Cells[fila, 26, fila, 27];
                                range.Merge = true;
                                worksheet.Cells[fila, 26].Value = "Total Cajas Irregulares";
                                worksheet.Cells[fila, 28].Value = cajasSegundas;

                                fila++;
                                range = worksheet.Cells[fila, 26, fila, 27];
                                range.Merge = true;
                                worksheet.Cells[fila, 26].Value = "Total Cajas Terceras";
                                worksheet.Cells[fila, 28].Value = cajasTerceras;

                                fila++;
                                range = worksheet.Cells[fila, 26, fila, 27];
                                range.Merge = true;
                                worksheet.Cells[fila, 26].Value = "Total Cajas";
                                worksheet.Cells[fila, 28].Value = data.Sum(x => x.Cajas) + cajasTerceras + cajasSegundas;

                                range = worksheet.Cells[fila - 3, 26, fila, 28];
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Color.SetColor(Color.Black);

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Bottom.Color.SetColor(Color.Black);

                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Color.SetColor(Color.Black);

                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Color.SetColor(Color.Black);

                                fileContents = package.GetAsByteArray();
                                try
                                {
                                    MailMessage mail = new MailMessage();

                                    mail.From = new MailAddress("sistema@intermoda.com.hn");

                                    var correos = await getCorreosDespachoPT(user);

                                    foreach (IM_WMS_Correos_DespachoPTDTO correo in correos)
                                    {
                                        mail.To.Add(correo.Correo);
                                    }
                                    mail.Subject = "Despacho PT No." + DespachoID.ToString().PadLeft(8, '0');
                                    mail.IsBodyHtml = false;

                                    mail.Body = "Despacho de PT desde " + encabezado.NAME + " a " + data[0].InventLocation;

                                    using (MemoryStream ms = new MemoryStream(fileContents))
                                    {
                                        DateTime date = DateTime.Now;
                                        string fecha = date.Day + "_" + date.Month + "_" + date.Year;
                                        Attachment attachment = new Attachment(ms, "Despacho N" + DespachoID + "-" + fecha + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                                        mail.Attachments.Add(attachment);

                                        SmtpClient oSmtpClient = new SmtpClient();

                                        oSmtpClient.Host = "smtp.office365.com";
                                        oSmtpClient.Port = 587;
                                        oSmtpClient.EnableSsl = true;
                                        oSmtpClient.UseDefaultCredentials = false;

                                        NetworkCredential userCredential = new NetworkCredential("sistema@intermoda.com.hn", "1nT3rM0d@.Syt3ma1l");

                                        oSmtpClient.Credentials = userCredential;

                                        oSmtpClient.Send(mail);
                                        oSmtpClient.Dispose();
                                    }
                                }
                                catch (Exception err)
                                {
                                    response.Descripcion = err.ToString();
                                }


                            }
                        }
                        catch (Exception err)
                        {
                            response.Descripcion = err.ToString();
                        }
                    }
                }

                
            }

            return response;
        }

        public IM_WMS_EnviarDespacho Get_EnviarDespachosLine(SqlDataReader reader)
        {
            return new IM_WMS_EnviarDespacho()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                Descripcion = reader["Descripcion"].ToString()
            };
        }
        public async Task<IM_WMS_EncabezadoDespachoExcelDTO> GetEncabezadoDespachoExcel(string user)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_EncabezadoDespachoExcel]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@user", user));



                    var response = new IM_WMS_EncabezadoDespachoExcelDTO();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = GetEncabezadoDespachoExcelLine(reader);
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_EncabezadoDespachoExcelDTO GetEncabezadoDespachoExcelLine(SqlDataReader reader)
        {
            return new IM_WMS_EncabezadoDespachoExcelDTO()
            {
                NAME = reader["NAME"].ToString(),
                STREET = reader["STREET"].ToString(),
                LOCATOR = reader["LOCATOR"].ToString(),
                ORGNUMBER = reader["ORGNUMBER"].ToString()
            };
        }

        public async Task<List<IM_WMS_Get_Despachos_PT_DTO>> GetDespachosEstado(string estado)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_DespachosEstado]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@estado", estado));



                    var response = new List< IM_WMS_Get_Despachos_PT_DTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add( Get_Despachos_PT_Lines(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public async Task<List<IM_WMS_ObtenerDespachoPTEnviados>> GetObtenerDespachoPTEnviados(int despachoID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_ObtenerDespachoPTEnviados]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DespachoID", despachoID));

                    var response = new List<IM_WMS_ObtenerDespachoPTEnviados>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetObtenerDespachoPTEnviadosLines(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public IM_WMS_ObtenerDespachoPTEnviados GetObtenerDespachoPTEnviadosLines(SqlDataReader reader)
        {
            return new IM_WMS_ObtenerDespachoPTEnviados()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                ProdID = reader["ProdID"].ToString(),
                Size = reader["Size"].ToString(),
                Color = reader["Color"].ToString(),
                fechaPacking = Convert.ToDateTime(reader["fechaPacking"].ToString()),
                ItemID = reader["ItemID"].ToString(),
                Box = Convert.ToInt32(reader["Box"].ToString()),
                QTY = Convert.ToInt32(reader["QTY"].ToString()),
                NeedAudit = Convert.ToBoolean(reader["NeedAudit"].ToString()),
                Receive = Convert.ToBoolean(reader["Receive"].ToString())
            };
        }

        public async Task<IM_WMS_DespachoPT_RecibirDTO> GetRecibir_DespachoPT(string ProdID, string userCreated, int Box)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_DespachoPT_Recibir]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@prodID", ProdID));
                    cmd.Parameters.Add(new SqlParameter("@box", Box));
                    cmd.Parameters.Add(new SqlParameter("@user", userCreated));

                    var response = new IM_WMS_DespachoPT_RecibirDTO();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = GetRecibir_DespachoPTLine(reader);
                        }
                    }
                    return response;
                }
            }
        }

        public IM_WMS_DespachoPT_RecibirDTO GetRecibir_DespachoPTLine(SqlDataReader reader)
        {
            return new IM_WMS_DespachoPT_RecibirDTO()
            {
                BOX = Convert.ToInt32(reader["BOX"].ToString()),
                PRODID = reader["PRODID"].ToString(),
                Receive = Convert.ToBoolean(reader["Receive"].ToString()),
                FechaReceive = Convert.ToDateTime(reader["FechaReceive"].ToString()),
                UserReceive = reader["UserReceive"].ToString(),                
            };
        }

        public async Task<List<IM_WMS_DespachoPT_CajasAuditarDTO>> getCajasAuditar(int despachoID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_DespachoPT_CajasAuditar]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DespachoID", despachoID));
                    

                    var response = new List<IM_WMS_DespachoPT_CajasAuditarDTO> ();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getCajasAuditarline(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_DespachoPT_CajasAuditarDTO getCajasAuditarline(SqlDataReader reader)
        {
            return new IM_WMS_DespachoPT_CajasAuditarDTO()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                ProdID = reader["ProdID"].ToString(),               
                Size = reader["Size"].ToString(),
                Color = reader["Color"].ToString(),                                
                ItemID = reader["ItemID"].ToString(),
                Box = Convert.ToInt32(reader["Box"].ToString()),
                QTY = Convert.ToInt32(reader["QTY"].ToString()),
                Auditado = Convert.ToInt32(reader["Auditado"].ToString()),
            };
        }

        public async Task<List<IM_WMS_Detalle_Auditoria_CajaDTO>> getDetalleAuditoriaCaja(string ProdID, int box)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Detalle_Auditoria_Caja]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProdID", ProdID));
                    cmd.Parameters.Add(new SqlParameter("@Box", box));


                    var response = new List<IM_WMS_Detalle_Auditoria_CajaDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getDetalleAuditoriaCajaLine(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_Detalle_Auditoria_CajaDTO getDetalleAuditoriaCajaLine(SqlDataReader reader)
        {
            return new IM_WMS_Detalle_Auditoria_CajaDTO()
            {
                ItemID = reader["ItemID"].ToString(),
                Size = reader["Size"].ToString(),
                Color = reader["Color"].ToString(),                
                QTY = Convert.ToInt32(reader["QTY"].ToString()),
                Auditada = Convert.ToInt32(reader["Auditada"].ToString()),
            };
        }

        public async Task<List<IM_WMS_Get_Despachos_PT_DTO>> getDespachosPTEstado(int DespachoID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_DespachosPT]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DespachoID", DespachoID));              

                    var response = new List<IM_WMS_Get_Despachos_PT_DTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(Get_Despachos_PT_Lines(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public async Task<List<IM_WMS_Consulta_OP_DetalleDTO>> getConsultaOPDetalle(string Prodcutsheetid)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Consulta_OP_Detalle]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Prodcutsheetid", Prodcutsheetid));

                    var response = new List<IM_WMS_Consulta_OP_DetalleDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getConsultaOPDetalleLines(reader));
                        }
                    }
                    return response;
                }
            }
        }

        public IM_WMS_Consulta_OP_DetalleDTO getConsultaOPDetalleLines(SqlDataReader reader)
        {
            return new IM_WMS_Consulta_OP_DetalleDTO()
            {
                ProdCutSheetID = reader["ProdCutSheetID"].ToString(),
                ProdID = reader["ProdID"].ToString(),
                Color = reader["Color"].ToString(),
                Size = reader["Size"].ToString(),                
                Cortado = Convert.ToInt32(reader["Cortado"].ToString()),
                Receive = Convert.ToInt32(reader["Receive"].ToString()),
                Segundas = Convert.ToInt32(reader["Segundas"].ToString()),
                terceras = Convert.ToInt32(reader["terceras"].ToString()),
                cajas = Convert.ToInt32(reader["cajas"].ToString()),
                DespachoID = Convert.ToInt32(reader["DespachoID"].ToString()),
            };
        }

        public async Task<List<IM_WMS_ConsultaOP_OrdenesDTO>> getConsultaOpOrdenes(string ProdCutSheetID, int DespachoID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_ConsultaOP_Ordenes]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Prodcutsheetid", ProdCutSheetID));
                    cmd.Parameters.Add(new SqlParameter("@DespachoID", DespachoID));


                    var response = new List<IM_WMS_ConsultaOP_OrdenesDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getConsultaOPOrdenesLines(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_ConsultaOP_OrdenesDTO getConsultaOPOrdenesLines(SqlDataReader reader)
        {
            return new IM_WMS_ConsultaOP_OrdenesDTO()
            {
                ProdCutSheetID = reader["ProdCutSheetID"].ToString(),                
                DespachoID = Convert.ToInt32(reader["DespachoID"].ToString())
            };
        }

        public async Task<List<IM_WMS_Consulta_OP_Detalle_CajasDTO>> getConsultaOPDetalleCajas(string ProdCutSheetID, int DespachoID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Consulta_OP_Detalle_Cajas]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Prodcutsheetid", ProdCutSheetID));
                    cmd.Parameters.Add(new SqlParameter("@DespachoID", DespachoID));


                    var response = new List<IM_WMS_Consulta_OP_Detalle_CajasDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getConsultaOPDetalleCajasLines(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_Consulta_OP_Detalle_CajasDTO getConsultaOPDetalleCajasLines(SqlDataReader reader)
        {
            return new IM_WMS_Consulta_OP_Detalle_CajasDTO()
            {
                Size = reader["Size"].ToString(),
                Box = Convert.ToInt32(reader["Box"].ToString()),
                QTY = Convert.ToInt32(reader["QTY"].ToString())

            };
        }

        public async Task<List<IM_WMS_Correos_DespachoPTDTO>> getCorreosDespachoPT(string user)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Correos_DespachoPTDTO]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@user", user));


                    var response = new List<IM_WMS_Correos_DespachoPTDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getCorreosDespachoPTLines(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_Correos_DespachoPTDTO getCorreosDespachoPTLines(SqlDataReader reader)
        {
            return new IM_WMS_Correos_DespachoPTDTO()
            {
                Correo = reader["Correo"].ToString(),
                ID = Convert.ToInt32(reader["ID"].ToString()),

            };
        }

        public async Task<IM_WMS_EnviarDiarioTransferirDTO> getEnviarDiarioTransferir(string JournalID, string userID)
        {
            var response = new IM_WMS_EnviarDiarioTransferirDTO();
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_EnviarDiarioTransferir]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@JournalID", JournalID));
                    cmd.Parameters.Add(new SqlParameter("@user", userID));

                    
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = getEnviarDiarioTransferirLines(reader);
                        }
                    }
                    
                }
            }

            var detalle = await getDetalle_Diario_Transferir_Correo(JournalID);
            string html = @"<!DOCTYPE html>
                            <html lang='en'>
                            <head>
                              <meta charset = 'UTF-8'/>
                               <style>
                                    h3,h4,td{
                                        text-align: center;
                                    }
                                    table,tr,th,td{
                                        border: 1px solid black;
                                        border-collapse: collapse;
                                    }
                                    .container{
                                        display: flex;
                                        justify-content: space-between;
                                    }
                                    .observacion {
                                        border-style: solid;
                                        border-width: 2px;
                                    }
                                </style>
                            </head>

                            <body>
                              <h3> Diario Transferir</h3>
   
                                 <h4>"+ JournalID + @"</h4>";
            //colocar en encabezado el almacen hacia donde va
            var encabezado = await GetEncabezadoTransferir(JournalID);

            //obtener encabezado
            html += @"
                     <p>           
  	                    <strong>Desde Almacen: </strong>" + encabezado.INVENTLOCATIONID + @" <br>
                        <strong>Hasta Almacen: </strong>" + encabezado.IM_INVENTLOCATIONID_TO + @" <br>
                        <strong>Asignado a: </strong>" + encabezado.PERSONNELNUMBER + @" <br>
                        
                      </p>
                   ";
            //tabla

            html +=@"<table style='width: 100%'>
                               <thead> 
                                 <tr> 
                                   <th colspan = '7'> Detalle </th>  
                                  </tr>  
                                  <tr>  
                                    <th> Articulo </th> 
                                    <th> Color </th> 
                                    <th> Talla </th> 
                                    <th> QTY </th> 
                                      
                                  </tr>  
                                  </thead>  
                                <tbody> ";
            int total = 0;

            foreach (var element in detalle)
            {
                total += element.QTY;
                html += @"<tr>
                              <td>" + element.ITEMID + @"</td>
                              <td>" + element.INVENTCOLORID + @"</td>
                              <td>" + element.INVENTSIZEID + @"</td>
                              <td>" + element.QTY + @"</td>      
                            </tr>";
            }

            //linea que muestra el total de rollos enviados
            html += @"</tbody>
                            <tfoot>
                                <tr>
                                  <td></td>
                                  <td></td>
                                  <td>Total</td>
                                  <td>" + total + @"</td>      
                             </tr>
                            </tfoot>
                        </table></body></html>";



            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress("sistema@intermoda.com.hn");

                var correos = await getCorreosDespachoTransferir();

                foreach (IM_WMS_Correos_Despacho correo in correos)
                {
                    mail.To.Add(correo.Correo);
                }
                mail.Subject = "Diario Transferir "+JournalID + " desde almacen "+ encabezado.INVENTLOCATIONID + " a "+ encabezado.IM_INVENTLOCATIONID_TO;
                mail.IsBodyHtml = true;

                mail.Body = html;

                SmtpClient oSmtpClient = new SmtpClient();

                oSmtpClient.Host = "smtp.office365.com";
                oSmtpClient.Port = 587;
                oSmtpClient.EnableSsl = true;
                oSmtpClient.UseDefaultCredentials = false;

                NetworkCredential userCredential = new NetworkCredential("sistema@intermoda.com.hn", "1nT3rM0d@.Syt3ma1l");

                oSmtpClient.Credentials = userCredential;

                oSmtpClient.Send(mail);
                oSmtpClient.Dispose();


            }
            catch (Exception err)
            {

            }


            return response;
        }
        public IM_WMS_EnviarDiarioTransferirDTO getEnviarDiarioTransferirLines(SqlDataReader reader)
        {
            return new IM_WMS_EnviarDiarioTransferirDTO()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                JournalID = reader["JournalID"].ToString(),
                userID = reader["userID"].ToString(),
                Fecha = Convert.ToDateTime(reader["Fecha"].ToString()),
            };
        }

        public async Task<IM_Encabezado_Diario_TransferirDTO> GetEncabezadoTransferir(string journalID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_Encabezado_Diario_Transferir]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@JournalID", journalID));


                    var response = new IM_Encabezado_Diario_TransferirDTO();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = GetEncabezadoTransferir(reader);
                        }
                    }
                    return response;
                }
            }
        }
        public IM_Encabezado_Diario_TransferirDTO GetEncabezadoTransferir(SqlDataReader reader)
        {
            return new IM_Encabezado_Diario_TransferirDTO()
            {
                JOURNALID = reader["JOURNALID"].ToString(),
                INVENTLOCATIONID = reader["INVENTLOCATIONID"].ToString(),
                IM_INVENTLOCATIONID_TO = reader["IM_INVENTLOCATIONID_TO"].ToString(),
                PERSONNELNUMBER = reader["PERSONNELNUMBER"].ToString(),
            };
        }

        public async Task<List<IM_WMS_Correos_Despacho>> getCorreosDespachoTransferir()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Correos_Transferirsp]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    var response = new List<IM_WMS_Correos_Despacho>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getCorreosDespachos(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public async Task<List<IM_WMS_Detalle_Diario_Transferir_CorreoDTO>> getDetalle_Diario_Transferir_Correo(string JournalID)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_WMS_Detalle_Diario_Transferir_Correo]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@JournalID", JournalID));

                    var response = new List<IM_WMS_Detalle_Diario_Transferir_CorreoDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(getDetalle_Diario_Transferir_CorreoLines(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public IM_WMS_Detalle_Diario_Transferir_CorreoDTO getDetalle_Diario_Transferir_CorreoLines(SqlDataReader reader)
        {
            return new IM_WMS_Detalle_Diario_Transferir_CorreoDTO()
            {
                ITEMID = reader["ITEMID"].ToString(),
                INVENTCOLORID = reader["INVENTCOLORID"].ToString(),
                INVENTSIZEID = reader["INVENTSIZEID"].ToString(),
                QTY = Convert.ToInt32(Convert.ToDecimal(reader["QTY"].ToString()))
            };
        }


    }
}