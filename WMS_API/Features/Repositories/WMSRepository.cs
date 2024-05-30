using Core.DTOs;
using Core.DTOs.Despacho_PT;
using Core.DTOs.Reduccion_Cajas;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
    }
}