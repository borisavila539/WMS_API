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
    public class WMSRepository: IWMSRepository
    {
        private readonly string _connectionString;

        public WMSRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("MicrosoftDynamicsAX_PRO");
        }

        public async Task<LoginDTO> PostLogin(LoginDTO datos)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_Login_WMS]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@user",datos.user));
                    cmd.Parameters.Add(new SqlParameter("@pass", datos.pass));

                    var response = new LoginDTO();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
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
                logeado = reader["logeado"].ToString() != "0" ? true:false

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
            var data =  await GetDatosEtiquetaMovimiento(diario, IMBoxCode);

            //creacion de encabezado de la etiqueta
            string encabezado = "";
            encabezado += "^XA";
            encabezado += "^CF0,50";
            encabezado += "^FO280,50^FD"+diario+"^FS";
            encabezado += "^CF0,24";
            encabezado += "^FO50,115^FDFecha: "+ DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year+ "^FS";
            encabezado += "^FO50,140^FDSoliciante: "+data[0].Solicitante+"^FS";
            encabezado += "^FO50,165^FDCaja #" + data[0].Numero_caja + "^FS";
            encabezado += "^FO480,115^FDCentro Costos: " + data[0].IM_CENTRO_DE_COSTOS + "^FS";
            encabezado += "^FO480,165^FDTipo Diario: " + data[0].JOURNALNAMEID + "^FS";
            encabezado += "^FO50,190^GB700,3,3^FS";

            //creacion del pie de pagina de la etiqueta
            string pie = "";
            pie += "^BY2,2,100";
            pie += "^FO50,1060^BC^FD"+IMBoxCode+"^FS";
            
            pie += "^FO430,1060^FDEmpacador: "+ data[0].Empacador+ "^FS";
            pie += "^CF0,40";
            

            string etiqueta = encabezado;
            int totalUnidades = 0;
            
            var groupData = new Dictionary<string,List<EtiquetaDTO>>();

            foreach (var element in data)
            {
                totalUnidades += element.QTY;
                var key = $"{element.ITEMID}-{element.INVENTCOLORID}";

                if(!groupData.ContainsKey(key))
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

            foreach(var element in groupArray)
            {
                etiqueta += $"^FO50,{lineaArticulo}^FD{element.items[0].ITEMID}*{element.items[0].INVENTCOLORID}^FS";
                etiqueta += $"^FO50,{lineaArticulo+30}^FDTalla: ";
                element.items.ForEach(x =>
               {
                   for(int i = 1; i<= 5 - x.INVENTSIZEID.Length;i++)
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


                if(lineaArticulo == 910)
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

            if(lineaArticulo != 910)
            {
                etiqueta += pie;
                etiqueta += $"^FO430,1090^FDTotal: {total}/{totalUnidades}^FS";
                etiqueta += "^XZ";
            }

                

            try{
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
    } 
}
