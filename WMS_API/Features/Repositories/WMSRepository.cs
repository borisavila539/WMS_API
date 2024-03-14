using Core.DTOs;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;


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
                logeado = reader["logeado"].ToString() != "0" ? true : false

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
                    cmd.Parameters.Add(new SqlParameter("@ID", USER));



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

            bool tipo = AX[0].ITEMID.Contains("45 00 1");


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

        public async Task<string> getNotaDespacho(int DespachoID, string recid, string empleado)
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
                        <strong>Fecha: </strong>"+encabezado[0].fecha+ @" <br>
  	                    <strong>Motorista: </strong>" + encabezado[0].Motorista + @" <br>
                        <strong>Traslado Inicial: </strong>" + encabezado[0].TRANSFERIDFROM + @" <br>
                        <strong>Traslado Final: </strong>" + encabezado[0].TRANSFERIDTO + @" <br>
                        <strong>Destino: </strong>" + encabezado[0].Destino + @" <br>
                      </p>
                   ";
            //obtener rollos que son parte del despacho
            var rollos = await getRolloDespacho(DespachoID);
            List<RollosDespachoDTO> data = new List<RollosDespachoDTO>();
           
            rollos.ForEach(async(element) =>
            {
                var RolloAX = await getRolloDespachoAX(element.InventTransID,element.INVENTSERIALID);
                int index = rollos.FindIndex(x => x.INVENTSERIALID == element.INVENTSERIALID);
                rollos[index].Color = RolloAX[0].Color;
                rollos[index].Config = RolloAX[0].Config;
                rollos[index].LibrasYardas = RolloAX[0].LibrasYardas;
                data.Add(rollos[index]);
            });

            data.ForEach(element =>
            {
                despacho += "<p>" + element.Color + "," + element.Config + "," + element.LibrasYardas + "</p>";
            });

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
                InventTransID = reader["InventTransID"].ToString(),

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

            };

        }
    }
}