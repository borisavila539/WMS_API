using Core.DTOs.CAEX;
using Core.DTOs.CAEX.Departamento;
using Core.DTOs.CAEX.Guia;
using Core.DTOs.CAEX.Municipios;
using Core.DTOs.CAEX.Poblados;
using Core.DTOs.CAEX.TipoPieza;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
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
            _url = "https://ws.caexlogistics.com/wsCAEXLogisticsSB/wsCAEXLogisticsSB.asmx";
            //Pruebas
            _url = "http://wsqa.caexlogistics.com:1880/wsCAEXLogisticsSB/wsCAEXLogisticsSB.asmx";

            //Produccion
           // _url = "https://ws.caexlogistics.com/wsCAEXLogisticsSB/wsCAEXLogisticsSB.asmx";


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



        public async Task<ResultadoGenerarGuia> GetGenerarGuia(string Cuentacliente,string ListasEmpaque,int cajas,string usuario)
        {
            string empresa = Cuentacliente.Substring(0,4);
            //obtener las credenciales por empresa
            var credencial = await GetCompanyCredencial(empresa);

            //obtener la data de los departamentos, municipios, poblados y piezas
            //var departamento = await getDepartamentos(credencial.Username,credencial.Password);
            //var municipio = await GetMunicipios(credencial.Username, credencial.Password);
            var poblado = await GetPoblados(credencial.Username, credencial.Password);
            var pieza = await GetPiezas(credencial.Username, credencial.Password);

            //obtener informacion del cliente
            var cliente = await GetDatosCliente(Cuentacliente);//"IMGT-000000704"

            //buscar departamento,municipio y poblado de cliente
            //var codigoDepto = departamento.Find(element => element.Nombre == cliente.Departamento).Codigo;
            // var codigoMuni = municipio.Find(element => element.CodigoDepto == codigoDepto && element.Nombre == cliente.Municipio).Codigo;
            var codigoPobladoCliente = poblado.Find(element => string.Equals(NormalizeText(element.Nombre),NormalizeText(cliente.Municipio),System.StringComparison.OrdinalIgnoreCase)).Codigo;

            //buscar poblado de Empresa
            var CodigoPoblaboEmpresa = poblado.Find(element => element.Nombre == credencial.Poblado).Codigo;

            //crear la lista de piezas a enviar
            List<PiezaGuia> piezas = new List<PiezaGuia>();
            var codigoPieza = pieza.Find(element => element.Descripcion == "PAQUETES").Codigo;

            for (int i = 1; i <= cajas; i++)
            {
                PiezaGuia tmp = new PiezaGuia();

                tmp.NumeroPieza = i;
                tmp.TipoPieza = codigoPieza;
                tmp.PesoPieza = "30.00";
                piezas.Add(tmp);
            }


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
                                    DestinatarioNombre = cliente.Nombre,
                                    DestinatarioDireccion = cliente.Direccion,
                                    DestinatarioTelefono = cliente.Telefono,
                                    DestinatarioContacto = cliente.Nombre,
                                    DestinatarioNIT = "",
                                    ReferenciaCliente1 = "",
                                    ReferenciaCliente2 = ListasEmpaque,
                                    CodigoPobladoDestino = codigoPobladoCliente,
                                    CodigoPobladoOrigen = CodigoPoblaboEmpresa,
                                    TipoServicio = "2",//cambiar a 1
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
                string[] Rutas = ListasEmpaque.Split(",");
                
                foreach(var ruta in Rutas)
                {
                    if(IDConsolidado == 0)
                    {
                        var crear = await getIM_WMS_CAEX_CrearRutas(ruta, usuario, IDConsolidado);
                        IDConsolidado = crear.ID;
                    }
                    else
                    {
                        await getIM_WMS_CAEX_CrearRutas(ruta, usuario, IDConsolidado);
                    }
                    
                }

            }


            //validar si se creo la guia y guardarla
            mappedResponse.Body.GenerarGuiaResponse.ResultadoGenerarGuia.ListaRecolecciones.DatosRecoleccion.ForEach(async(element) =>
            {
                if (element.ResultadoOperacion.ResultadoExitoso)
                {
                    var insertarCaja = await getIM_WMSCAEX_CrearRutas_Cajas(IDConsolidado, element.NumeroPieza, element.NumeroGuia, element.URLConsulta);

                }
            });
           
            return mappedResponse.Body.GenerarGuiaResponse.ResultadoGenerarGuia;
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
