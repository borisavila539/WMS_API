using Core.DTOs.CAEX;
using Core.DTOs.CAEX.Departamento;
using Core.DTOs.CAEX.Guia;
using Core.DTOs.CAEX.Municipios;
using Core.DTOs.CAEX.Poblados;
using Core.DTOs.CAEX.TipoPieza;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class WMSCAEXRepository : IWMSCAEXRepository
    {
        private readonly string _connectionString;
        private readonly string _url;
        public WMSCAEXRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzas");
            //Pruebas
            //_url = "http://wsqa.caexlogistics.com:1880/wsCAEXLogisticsSB/wsCAEXLogisticsSB.asmx";

            //Produccion
            _url = "https://ws.caexlogistics.com/wsCAEXLogisticsSB/wsCAEXLogisticsSB.asmx";


        }
        public async Task<IM_WMSCAEX_CompanyCredencial> GetCompanyCredencial(string company)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Company",company)
            };

            IM_WMSCAEX_CompanyCredencial result = await executeProcedure.ExecuteStoredProcedure<IM_WMSCAEX_CompanyCredencial>("[IM_WMSCAEX_getCompanyCredencial]", parametros);

            return result;
        }

        public async Task<List<Departamento>> getDepartamentos(string user, string pass)
        {
            DepartamentoXMLRequest request = new DepartamentoXMLRequest
            {
                Body = new BodyRequestDepartamento
                {
                    ObtenerListadoDepartamentos = new ObtenerListadoDepartamentos
                    {
                        Autenticacion = new Autenticacion
                        {
                            Login = user,
                            Password = pass
                        }
                    }
                }
            };

            string xmlRequest = SerializeXml(request);
            string response = await SendPostRequestAsync(_url, xmlRequest);
            DepartamentosXMLResponse mappedResponse = DeserializeXml<DepartamentosXMLResponse>(response);
            return mappedResponse.Body.ObtenerListadoDepartamentosResponse.ResultadoObtenerDepartamentos.Departamentos.ToList();
        }

        public async Task<List<Municipio>> GetMunicipios(string user, string pass)
        {
            MunicipiosXMLRequest request = new MunicipiosXMLRequest
            {
                Body = new BodyRequestMunicipio
                {
                    ObtenerListadoMunicipios = new ObtenerListadoMunicipios
                    {
                        Autenticacion = new Autenticacion
                        {
                            Login = user,
                            Password = pass
                        }
                    }
                }
            };

            string xmlRequest = SerializeXml(request);
            string response = await SendPostRequestAsync(_url, xmlRequest);
            MunicipioXMLResponse mappedResponse = DeserializeXml<MunicipioXMLResponse>(response);
            return mappedResponse.Body.ObtenerListadoMunicipiosResponse.ResultadoObtenerMunicipios.Municipios.ToList();
        }

        public async Task<List<Poblado>> GetPoblados(string user, string pass)
        {
            PobladoXMLRequest request = new PobladoXMLRequest
            {
                Body = new BodyRequestPoblado
                {
                    ObtenerListadoPoblados = new ObtenerListadoPoblados
                    {
                        Autenticacion = new Autenticacion
                        {
                            Login = user,
                            Password = pass
                        }
                    }
                }
            };

            string xmlRequest = SerializeXml(request);
            string response = await SendPostRequestAsync(_url, xmlRequest);
            PobladoXMLResponse mappedResponse = DeserializeXml<PobladoXMLResponse>(response);
            return mappedResponse.Body.ObtenerListadoPobladosResponse.ResultadoObtenerPoblados.Poblados.ToList();
        }

        public async Task<List<Pieza>> GetPiezas(string user, string pass)
        {
            TipoPiezaXMLRequest request = new TipoPiezaXMLRequest
            {
                Body = new BodyRequestPieza
                {
                    ObtenerTiposPiezas = new ObtenerListadoPiezas
                    {
                        Autenticacion = new Autenticacion
                        {
                            Login = user,
                            Password = pass
                        }
                    }
                }
            };

            string xmlRequest = SerializeXml(request);
            string response = await SendPostRequestAsync(_url, xmlRequest);
            TipoPiezaXMLResponse mappedResponse = DeserializeXml<TipoPiezaXMLResponse>(response);
            return mappedResponse.Body.ObtenerTiposPiezasResponse.ResultadoObtenerPiezas.Piezas.ToList();
        }

        public async Task<IM_WMSCAEX_GetDatosCliente> GetDatosCliente(string cliente)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Cliente",cliente)
            };

            IM_WMSCAEX_GetDatosCliente result = await executeProcedure.ExecuteStoredProcedure<IM_WMSCAEX_GetDatosCliente>("[IM_WMSCAEX_GetDatosCliente]", parametros);

            return result;
        }

        public async Task<IM_WMS_CAEX_CrearRutas> getIM_WMS_CAEX_CrearRutas(string ruta,string usuario,int consolidado)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Ruta",ruta),
                new SqlParameter("@usuario",usuario),
                new SqlParameter("@idConsolidado",consolidado)
            };

            IM_WMS_CAEX_CrearRutas result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_CAEX_CrearRutas>("[IM_WMS_CAEX_CrearRutas]", parametros);

            return result;
        }

        public async Task<IM_WMSCAEX_CrearRutas_Cajas> getIM_WMSCAEX_CrearRutas_Cajas(int idConsolidado,int NumeroPieza,string NumeroGuia,string URLConsulta)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@idConsolidado",idConsolidado),
                new SqlParameter("@NumeroPieza",NumeroPieza),
                new SqlParameter("@NumeroGuia",NumeroGuia),
                new SqlParameter("@URLConsulta",URLConsulta)

            };

            IM_WMSCAEX_CrearRutas_Cajas result = await executeProcedure.ExecuteStoredProcedure<IM_WMSCAEX_CrearRutas_Cajas>("[IM_WMSCAEX_CrearRutas_Cajas]", parametros);

            return result;
        }

        public async Task<IM_WMSCAEX_ObtenerDetallePickingRouteID> getDetallePickingRoute(string BoxCode)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@BoxCode",BoxCode)
            };

            IM_WMSCAEX_ObtenerDetallePickingRouteID result = await executeProcedure.ExecuteStoredProcedure<IM_WMSCAEX_ObtenerDetallePickingRouteID>("[IM_WMSCAEX_ObtenerDetallePickingRouteID]", parametros);

            return result;
        }

        public async Task<IM_WMS_CAEX_ObtenerPedido> GetObtenerPedido(string ListaEmpaque)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@ListaEmpaque",ListaEmpaque)
            };

            IM_WMS_CAEX_ObtenerPedido result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_CAEX_ObtenerPedido>("[IM_WMS_CAEX_ObtenerPedido]", parametros);

            return result;
        }



        public async Task<ResultadoGenerarGuia> GetGenerarGuia(RequestGenerarGuia data)
        {
            ResultadoGenerarGuia err = new ResultadoGenerarGuia();
            string empresa = data.Cliente.Substring(0,4);
            //obtener las credenciales por empresa
            var credencial = await GetCompanyCredencial(empresa);            

            //obtener informacion del cliente
            //var cliente = await GetDatosCliente(data.Cliente);//"IMGT-000000704"

            
            if (data.ListasEmpaque[0].Codigo == "")
            {   
                err.ResultadoOperacionMultiple = new ResultadoOperacionMultiple{
                    ResultadoExitoso = false,
                    MensajeError = "No existe " + data.ListasEmpaque[0].County + " como Poblado ",
                    CodigoRespuesta = 0
                }
                ;
                return err;
            }           

            //crear la lista de piezas a enviar
            List<PiezaGuia> piezas = new List<PiezaGuia>();            

            for (int i = 1; i <= data.Cajas; i++)
            {
                PiezaGuia tmp = new PiezaGuia();

                tmp.NumeroPieza = i;
                tmp.TipoPieza = "2";
                tmp.PesoPieza = "30.00";
                piezas.Add(tmp);
            }
            List<string> pedidos = new List<string>();
            string ListasEmpaque = "";
            foreach(var element in data.ListasEmpaque)
            {
                ListasEmpaque += element.PickingRouteID + ',';

                //obtener lista de Pedidos
                //var resp = await GetObtenerPedido(element.PickingRouteID);
                var ped = pedidos.Find(x => x == element.SalesID);
                if (ped == null)
                {                    
                    pedidos.Add(element.SalesID);

                }
            }

            string reference1 = "#" + data.ListasEmpaque.First().Embarque + ",";
            pedidos.ForEach(ele =>
            {
                reference1 += ele + ",";
            });

            GuiaXMLRequest request = new GuiaXMLRequest
            {
                Body = new BodyRequestGenerarGuia
                {
                    GenerarGuia = new GenerarGuia
                    {
                        Autenticacion = new Autenticacion
                        {
                            Login = credencial.Username,
                            Password = credencial.Password
                        },
                        ListaRecolecciones = new ListaRecolecciones
                        {
                            DatosRecoleccion = new List<DatosRecoleccion>
                            {
                                new DatosRecoleccion
                                {
                                    RecoleccionID="",
                                    RemitenteNombre = credencial.Name,
                                    RemitenteDireccion = credencial.Address,
                                    RemitenteTelefono = credencial.ContactPhone,
                                    DestinatarioNombre = data.ListasEmpaque[0].Cliente,
                                    DestinatarioDireccion = data.ListasEmpaque[0].Address,
                                    DestinatarioTelefono = data.ListasEmpaque[0].Telefono,
                                    DestinatarioContacto = data.ListasEmpaque[0].Cliente,
                                    DestinatarioNIT = "",
                                    ReferenciaCliente1 = reference1,
                                    ReferenciaCliente2 = ListasEmpaque,
                                    CodigoPobladoDestino = data.ListasEmpaque[0].Codigo,
                                    CodigoPobladoOrigen = credencial.Poblado,
                                    TipoServicio = "1",
                                    FormatoImpresion = "1",
                                    CodigoCredito= credencial.CodigoCredito,
                                    Piezas = piezas
                                }
                            }
                        }

                    }
                }
            };

            string xmlRequest = SerializeXml(request);
            string response = await SendPostRequestAsync(_url, xmlRequest);
            GuiaXMLResponse mappedResponse = DeserializeXml<GuiaXMLResponse>(response);
            //guardar las rutas
            int IDConsolidado = 0;
            if (mappedResponse.Body.GenerarGuiaResponse.ResultadoGenerarGuia.ResultadoOperacionMultiple.ResultadoExitoso)
            {
                                
                foreach(var ruta in data.ListasEmpaque)
                {
                    if(IDConsolidado == 0)
                    {
                        
                        var crear = await getIM_WMS_CAEX_CrearRutas(ruta.PickingRouteID, data.usuario, IDConsolidado);
                        IDConsolidado = crear.ID;
                    }
                    else
                    {
                        await getIM_WMS_CAEX_CrearRutas(ruta.PickingRouteID, data.usuario, IDConsolidado);
                    }
                    
                }

            }


            //validar si se creo la guia y guardarla
            List<IM_WMSCAEX_CrearRutas_Cajas> imprimir = new List<IM_WMSCAEX_CrearRutas_Cajas>();

            foreach (var element in mappedResponse.Body.GenerarGuiaResponse.ResultadoGenerarGuia.ListaRecolecciones.DatosRecoleccion)
            {
            

                if (element.ResultadoOperacion.ResultadoExitoso)
                {
                    var insertarCaja = await getIM_WMSCAEX_CrearRutas_Cajas(IDConsolidado, element.NumeroPieza, element.NumeroGuia, element.URLConsulta);
                    imprimir.Add(insertarCaja);  

                }
            }
            var impresion = await postImprimirEtiqueta(imprimir);



            return mappedResponse.Body.GenerarGuiaResponse.ResultadoGenerarGuia;
        }

        public async Task<List<IM_WMSCAEX_ObtenerReimpresionEtiquetas>> getObtenerReimpresionEtiquetas(string BoxCode)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@BoxCode",BoxCode)
            };

            List<IM_WMSCAEX_ObtenerReimpresionEtiquetas> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMSCAEX_ObtenerReimpresionEtiquetas>("[IM_WMSCAEX_ObtenerReimpresionEtiquetas]", parametros);

            return result;
        }

        public async Task<string> postImprimirEtiqueta(List<IM_WMSCAEX_CrearRutas_Cajas> urls)
        {
            string response = "";
            urls = urls.OrderBy(x => x.NumeroPieza).ToList();
            foreach (var url in urls)
            {
                using (HttpClient client = new HttpClient())
                {
                    var download = await client.GetAsync(url.URLConsulta);
                    if (download.IsSuccessStatusCode)
                    {
                        byte[] pdfData = await download.Content.ReadAsByteArrayAsync();

                        // Convertir PDF a imagen o ZPL
                        string zplData = ConvertPdfToZpl(pdfData); 

                        try
                        {
                            using (TcpClient print = new TcpClient("10.1.1.221", 9100))
                            {
                                using (NetworkStream stream = print.GetStream())
                                {
                                    byte[] zplBytes = System.Text.Encoding.UTF8.GetBytes(zplData);
                                    stream.Write(zplBytes, 0, zplBytes.Length);
                                    response = "OK";
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            response = err.ToString();
                        }
                    }
                    else
                    {
                        response = "Error al descargar el archivo";
                    }
                }
            }

            return response;
        }

        public string ConvertPdfToZpl(byte[] pdfData)
        {
            // Convierte el PDF a una imagen con alta resolución
            using (MemoryStream pdfStream = new MemoryStream(pdfData))
            using (PdfDocument pdfDoc = PdfDocument.Load(pdfStream))
            {
                // Renderiza la primera página con mayor DPI (e.g., 600 DPI)
                Image pdfImage = pdfDoc.Render(0, 600, 600, PdfRenderFlags.CorrectFromDpi);

                // Ajusta el tamaño de la imagen para que se corresponda con 4x4 pulgadas (812x812 puntos)
                Bitmap resizedBitmap = ResizeBitmap(pdfImage, 812, 812);

                // Generar el código ZPL a partir de la imagen redimensionada
                return ConvertBitmapToZpl(resizedBitmap);
            }
        }

        private Bitmap ResizeBitmap(Image originalImage, int width, int height)
        {
            Bitmap resizedBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(resizedBitmap))
            {
                g.Clear(Color.White);
                g.DrawImage(originalImage, 0, 0, width, height);
            }
            return resizedBitmap;
        }

        private Bitmap ConvertToNonIndexedBitmap(Image originalImage)
        {
            // Convierte la imagen a formato no indexado (Format32bppArgb)
            Bitmap newBitmap = new Bitmap(originalImage.Width, originalImage.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.DrawImage(originalImage, 0, 0);
            }
            return newBitmap;
        }

        private string ConvertBitmapToZpl(Bitmap bitmap)
        {
            StringBuilder zpl = new StringBuilder();

            // Configuración de tamaño del papel (4x4 pulgadas en puntos)
            zpl.AppendLine("^XA");
            zpl.AppendLine("^PW812");  // Ancho del papel: 812 puntos
            zpl.AppendLine("^LL812");  // Largo del papel: 812 puntos
            zpl.AppendLine("^FO0,0");  // Posición inicial en (0,0)

            // Tamaño de la imagen
            int widthBytes = (bitmap.Width + 7) / 8;
            int totalBytes = widthBytes * bitmap.Height;
            byte[] imageData = new byte[totalBytes];
            int byteIndex = 0;

            // Convertir la imagen a bytes ZPL
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x += 8)
                {
                    byte pixelByte = 0;
                    for (int bit = 0; bit < 8; bit++)
                    {
                        if (x + bit < bitmap.Width && bitmap.GetPixel(x + bit, y).R < 128) // Negro es 1
                        {
                            pixelByte |= (byte)(1 << (7 - bit));
                        }
                    }
                    imageData[byteIndex++] = pixelByte;
                }
            }

            // Convertir los datos a formato hexadecimal
            string hexData = BitConverter.ToString(imageData).Replace("-", "");

            // Generar el comando GFA
            zpl.AppendLine($"^GFA,{totalBytes},{totalBytes},{widthBytes},{hexData}");

            // Finalizar ZPL
            zpl.AppendLine("^XZ");

            return zpl.ToString();
        }





        public string NormalizeText(string input)
        {
            // Normalización para eliminar acentos y convertir a minúsculas
            string normalized = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (char c in normalized)
            {
                // Filtrar caracteres no combinados (acentos)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Convertir a minúsculas
            return stringBuilder.ToString().ToLowerInvariant();
        }

        //Generar metodo Post enviando el XML
        public static async Task<string> SendPostRequestAsync(string url, string xmlData)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(xmlData, Encoding.UTF8, "text/xml");
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public static string SerializeXml<T>(T data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, data);
                return writer.ToString();
            }
        }

        public static T DeserializeXml<T>(string xmlData)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xmlData))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
