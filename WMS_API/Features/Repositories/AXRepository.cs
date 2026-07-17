using Core.DTOs;
using Core.DTOs.Cajasrecicladas;
using Core.DTOs.IM_PrepEnvOp;
using Core.DTOs.InventarioCiclicoTela;
using Core.DTOs.RecepcionYUbicacionAX;
using Core.DTOs.Serigrafia;
using Core.DTOs.Serigrafia.ClaseRespuesta;
using Core.DTOs.TejidoPunto;
using Core.DTOs.UbiacacionRollos;
using Core.Interfaces;
using IM_PreparacionDeOpGP;
using IM_WMS_CajaRecladasGP;
using IM_WMS_CrearNuevaUbicacion;
using IM_WMS_MoviminetoWS;
using IM_WMS_RecepcionSubcontratacionGP;
using IM_WMS_RecepcionTrasladoPepeMBGPService;
using IM_WMS_ReduccionCajas;
using IM_WMS_SRG_ChangeOpEST;
using IM_WMS_SRG_ProductDispatchGP;
using IM_WMS_SRG_ReporteAsFinishedOPGP;
using IM_WMS_Traslado_Enviar_Recibir;
using IMGetTransferJournalAsgGPService;
using OfficeOpenXml.Utils;
using ServiceReference1;
using ServiceReferenceIM_WMS_InventarioCiclicoTela;
using ServiceReferenceIM_WMS_Trasferir_Inventario;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WMS_API.Features.Repositories
{
    public class AXRepository : IAX
    {
        public async Task<string> RegistrarMovimientoRollosEnDiario(List<MovimientoRolloDto> rollosAMover)
        {
            if (rollosAMover == null || rollosAMover.Count == 0)
            {
                return "E Error: No se proporcionaron rollos para registrar el movimiento.";
            }

            // Instancia del servicio con tu referencia específica
            var context = new IMGetTransferJournalAsgGPService.CallContext { Company = "IMHN" };
            var serviceClient = new MGetTransferJournalAsgClient(
                GetBindingGeneric("NetTcpBinding_IMGetTransferJournalAsg"),
                GetEndpointGeneric("net.tcp://gim-pro3-aos:8201/DynamicsAx/Services/IMGetTransferJournalAsgGP")
            );

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";

            string diarioIdCreado = string.Empty;

            try
            {
                // ====================================================================
                // FASE 1: CREAR EL ENCABEZADO DEL DIARIO DE MOVIMIENTO AUTOMÁTICO
                // ====================================================================
                var datosHeader = new DIARIO_MOVIMIENTO_ROLLO_HEADER
                {
                    COMPANY = new MovimientoRolloHeaderData
                    {
                        JOURNALDESCRIPTION = $"Diario automático movimiento a ubicación {rollosAMover[0].UbicacionDestino}"
                    }
                };

                // Serialización con tu SerializationService
                string xmlHeader = SerializationService.Serialize(datosHeader);
                var respHeader = await serviceClient.getTransferJournalHeaderAsync(context, xmlHeader);
                string resultadoHeader = respHeader.response.ToString().Trim();
                XDocument xmlDoc = XDocument.Parse(resultadoHeader);
                var respuestaNode = xmlDoc.Descendants("Respuesta").FirstOrDefault();
                string stringContenido = respuestaNode?.Value?.Trim() ?? string.Empty;

                if (stringContenido.StartsWith("S "))
                {
                    // Extrae de forma segura el ID del diario generado por AX (ej. de "S DJ-001" obtiene "DJ-001")
                    diarioIdCreado = stringContenido.Substring(2).Trim();
                }
                else
                {
                    return $"E Error al inicializar el diario en AX. Detalle: {stringContenido}";
                }

                // ====================================================================
                // FASE 2: REGISTRAR LAS LÍNEAS DE MOVIMIENTO POR CADA ROLLO ESCANEADO
                // ====================================================================
                int registrosExitosos = 0;
                System.Text.StringBuilder historialErrores = new System.Text.StringBuilder();

                foreach (var rollo in rollosAMover)
                {
                    var datosMovimiento = new DIARIO_MOVIMIENTO_ROLLO_LINE
                    {
                        COMPANY = new MovimientoRolloLineData
                        {
                            JOURNALID = diarioIdCreado,
                            BARCODE = rollo.CodigoBarraRollo,
                            QTY = rollo.Cantidad,

                            // El sitio y almacén suelen mantenerse fijos en movimientos de ubicación directa
                            FROMINVENTSITEID = rollo.SitioOrigen,
                            FROMINVENTLOCATIONID = rollo.AlmacenOrigen,
                            FROMWMSLOCATIONID = rollo.UbicacionOrigen,

                            TOINVENTSITEID = rollo.SitioDestino,
                            TOINVENTLOCATIONID = rollo.AlmacenDestino,
                            TOWMSLOCATIONID = rollo.UbicacionDestino
                        }
                    };

                    string xmlLinea = SerializationService.Serialize(datosMovimiento);
                    var respLinea = await serviceClient.getTransferJournalLine2Async(context, xmlLinea);
                    string resultadoLinea = respLinea.response.ToString().Trim();
                    XDocument xmlDocline = XDocument.Parse(resultadoLinea);
                    var respuestaNodeline = xmlDocline.Descendants("Respuesta").FirstOrDefault();
                    string stringContenidoline = respuestaNodeline?.Value?.Trim() ?? string.Empty;

                    // AX retorna "OK" si pasó todas tus validaciones internas (Availability, Counting, etc.)
                    if (stringContenidoline.Trim().ToUpper() == "OK")
                    {
                        registrosExitosos++;
                    }
                    else
                    {
                        historialErrores.AppendLine($"Rollo {rollo.CodigoBarraRollo}: {stringContenidoline}");
                    }
                }

                // --- CONSOLIDACIÓN DE RESPUESTA Y POSTEO EN AX ---

                // Si hubo errores al insertar líneas, es mejor NO registrar (postear) el diario para evitar inconsistencias
                if (historialErrores.Length > 0)
                {
                    return $"W Proceso terminado con observaciones en Diario {diarioIdCreado} ({registrosExitosos} exitosos). No se procedió con el registro por errores en líneas:\n{historialErrores.ToString()}";
                }

                // ====================================================================
                // FASE 3: REGISTRAR / POSTEAR EL DIARIO (Llamada al nuevo método de AX)
                // ====================================================================
                try
                {
                    var datosPost = new DIARIO_MOVIMIENTO_POST
                    {
                        COMPANY = new PostJournalData
                        {
                            CODE = context.Company,          // "IMHN"
                            USER = "servicio_ax",            // Usuario del sistema que ejecuta la acción
                            JOURNALID = diarioIdCreado       // El ID que rescatamos en la Fase 1
                        }
                    };

                    string xmlPost = SerializationService.Serialize(datosPost);
                    var respPost = await serviceClient.getPostTransferJournalAsync(context, xmlPost);
                    string resultadoPost = respPost.response.ToString().Trim();

                    XDocument xmlDocPost = XDocument.Parse(resultadoPost);
                    var respuestaNodePost = xmlDocPost.Descendants("Respuesta").FirstOrDefault();
                    string stringContenidoPost = respuestaNodePost?.Value?.Trim() ?? string.Empty;

                    // AX usualmente devuelve "OK" si el posteo fue exitoso según su lógica interna
                    if (stringContenidoPost.Trim().ToUpper() == "OK")
                    {
                        return $"S Movimiento finalizado y POSTEADO con éxito. Diario AX: {diarioIdCreado}. Se registraron {registrosExitosos} rollos.";
                    }
                    else
                    {
                        return $"E El diario {diarioIdCreado} fue creado con {registrosExitosos} líneas, pero falló al registrarse (postear) en AX. Detalle: {stringContenidoPost}";
                    }
                }
                catch (Exception exPost)
                {
                    return $"E El diario {diarioIdCreado} fue creado con {registrosExitosos} líneas, pero ocurrió un error crítico al intentar postearlo: {exPost.Message}";
                }
            }
            catch (Exception ex)
            {
                return "E Excepción crítica en el servicio de movimientos: " + ex.Message;
            }
        }
        public async Task<Respuesta<string>> AgregarNuevaUbicacion(string empresa, string ubicacion, string almacen, string pasillo)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            IM_WMS_CrearNuevaUbicacion.CallContext context = new IM_WMS_CrearNuevaUbicacion.CallContext{Company = "IMHN"};
            var serviceClient = new M_WMS_AgregarNuevaUbiacionClient(
                GetBindingGeneric("NetTcpBinding_IM_WMS_AgregarNuevaUbiacion"),
                GetEndpointGeneric("net.tcp://gim-pro3-aos:8201/DynamicsAx/Services/IM_WMS_AgregarNuevaUbicacionGP")
            );

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";

            try
            {
                var xml = new StringBuilder();
                xml.Append("<DATA>");
                xml.Append("<COMPANY>");
                xml.AppendFormat("<CODE>{0}</CODE>", empresa);
                xml.AppendFormat("<WMSLOCATIONID>{0}</WMSLOCATIONID>", ubicacion);
                xml.AppendFormat("<INVENTLOCATIONID>{0}</INVENTLOCATIONID>", almacen);
                xml.AppendFormat("<AISLEID>{0}</AISLEID>", pasillo);
                xml.Append("</COMPANY>");
                xml.Append("</DATA>");
                string xmlInput = xml.ToString();
                var resp = await serviceClient.createNewLocationXMLAsync(context, xmlInput);
                string rawResponse = resp.response?.ToString()?.Trim();

                if (string.IsNullOrEmpty(rawResponse))
                {
                    return new Respuesta<string> { Exito = false, Mensaje = "El servicio de AX devolvió una respuesta vacía." };
                }
                if (!rawResponse.StartsWith("<"))
                {
                    return new Respuesta<string>
                    {
                        Exito = false,
                        Mensaje = rawResponse.StartsWith("E ") ? rawResponse.Substring(2) : rawResponse
                    };
                }
                XDocument xmlDoc = XDocument.Parse(rawResponse);
                var respuestaNode = xmlDoc.Descendants("Respuesta").FirstOrDefault();
                string stringContenido = respuestaNode?.Value?.Trim() ?? string.Empty;

                if (stringContenido == "OK")
                {
                    respuesta = new Respuesta<string>
                    {
                        Exito = true,
                        Mensaje = "Se creó correctamente la ubicación.",
                        Datos = stringContenido
                    };
                }
                else 
                {
                    string mensajeError = stringContenido.StartsWith("E ") ? stringContenido.Substring(2) : stringContenido;

                    respuesta = new Respuesta<string>
                    {
                        Exito = false,
                        Mensaje = string.IsNullOrEmpty(mensajeError) ? "Error desconocido en AX." : mensajeError
                    };
                }
            }
            catch (Exception ex)
            {
                respuesta = new Respuesta<string>
                {
                    Exito = false,
                    Mensaje = $"Error de comunicación o parseo: {ex.Message}",
                    Datos = null
                };
            }

            return respuesta;
        }
        public string EnviarRecibirTraslados(string TRANSFERID, string ESTADO)
        {
            TRASLADOHEADER HEADER = new TRASLADOHEADER();
            List<TRASLADOLINE> LIST = new List<TRASLADOLINE>();

            TRASLADOLINE LINE = new TRASLADOLINE();
            LINE.TRANSFERID = TRANSFERID;
            LINE.ESTADO = ESTADO;
            LIST.Add(LINE);

            HEADER.LINES = LIST.ToArray();

            string trasladosLine = SerializationService.Serialize(HEADER);
            IM_WMS_Traslado_Enviar_Recibir.CallContext context = new IM_WMS_Traslado_Enviar_Recibir.CallContext { Company = "IMHN" };
            var serviceClient = new M_WMS_Traslados_Enviar_RecibirClient(GetBinding(), GetEndpointAddrT());

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";


            try
            {
                string dataValidation = string.Format("<INTEGRATION><COMPANY><CODE>{0}</CODE><USER>{1}</USER></COMPANY></INTEGRATION>", context.Company, "servicio_ax");
                var resp = serviceClient.initAsync(context, dataValidation, trasladosLine);

                return resp.Result.response;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public async Task<Respuesta<string>> RecibirTrasladoYCambioUbiacion(string trasladoId,InformacionEmpresa informacion)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            IM_WMS_RecepcionTrasladoPepeMBGPService.CallContext context = new IM_WMS_RecepcionTrasladoPepeMBGPService.CallContext
            {
                Company = "IMHN"
            };

            var serviceClient = new M_WMS_RecepcionTrasladoPepeMBClient(
                GetBindingGeneric("NetTcpBinding_IM_WMS_RecepcionTrasladoPepeMB"),
                GetEndpointGeneric("net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_RecepcionTrasladoPepeMBGP")
            );

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";

            try
            {
                var xml = new StringBuilder();

                xml.Append("<INTEGRATION>");
                xml.AppendFormat("<TRANSFERID>{0}</TRANSFERID>", trasladoId);
                xml.AppendFormat("<UBICACION>{0}</UBICACION>", informacion.Ubicacion);
                xml.AppendFormat("<UBICACIONIRREGULAR>{0}</UBICACIONIRREGULAR>", informacion.UbicacionIrregular);
                xml.AppendFormat("<UBICACIONTERCERA>{0}</UBICACIONTERCERA>", informacion.UbicacionTercera);
                xml.AppendFormat("<LOTEIRREGULAR>{0}</LOTEIRREGULAR>", informacion.LOTEIRREGULAR);
                xml.AppendFormat("<LOTETERCERA>{0}</LOTETERCERA>", informacion.LOTETERCERA);
                xml.AppendFormat("<ALMACEN>{0}</ALMACEN>", informacion.ALMACEN);

                xml.Append("</INTEGRATION>");

                string xmlInput = xml.ToString();

                var resp = await serviceClient.initRecepcionAsync(context, xmlInput);
                string xmlResponse = resp.response?.ToString();
                if (!string.IsNullOrEmpty(xmlResponse))
                {
                    XDocument xmlDoc = XDocument.Parse(xmlResponse);
                    var respuestaXml = xmlDoc.Descendants("RESPUESTA").FirstOrDefault()?.Value;
                    var estado = respuestaXml.Split('|')[0].Replace("Estado:", "").Trim();
                    var mensaje = respuestaXml.Split('|')[1].Replace("Mensaje:", "").Trim();
                    if (estado == "Éxito")
                    {
                        respuesta = new Respuesta<string>
                        {
                            Exito = true,
                            Mensaje = mensaje,
                            Datos = null,
                        };
                    }
                    else
                    {
                        respuesta = new Respuesta<string>
                        {
                            Exito = false,
                            Mensaje = mensaje,
                            Datos = null,
                        };
                    }
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                respuesta = new Respuesta<string>
                {
                    Exito = false,
                    Mensaje = ex.ToString(),
                    Datos = null,
                };
            }
           
            return respuesta;
        }

        public string InsertDeleteMovimientoLine(string JOURNALID, string ITEMBARCODE, string PROCESO, string IMBOXCODE)
        {
            MOVIMIENTOHEADER HEADER = new MOVIMIENTOHEADER();
            List<MOVIMIENTOLINE> LIST = new List<MOVIMIENTOLINE>();

            MOVIMIENTOLINE LINE = new MOVIMIENTOLINE();
            LINE.JOURNALID = JOURNALID;
            LINE.ITEMBARCODE = ITEMBARCODE;
            LINE.PROCESO = PROCESO;
            LINE.IMBOXCODE = IMBOXCODE;
            LIST.Add(LINE);

            HEADER.LINES = LIST.ToArray();
            string movimientosLines = SerializationService.Serialize(HEADER);
            IM_WMS_MoviminetoWS.CallContext context = new IM_WMS_MoviminetoWS.CallContext { Company = "IMHN" };
            var serviceClient = new M_WMS_MovimientoClient(GetBinding(), GetEndpointAddr());

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";


            try
            {
                string dataValidation = string.Format("<INTEGRATION><COMPANY><CODE>{0}</CODE><USER>{1}</USER></COMPANY></INTEGRATION>", context.Company, "servicio_ax");
                var resp = serviceClient.initAsync(context, dataValidation, movimientosLines);

                return resp.Result.response;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string INsertDeleteReduccionCajas(string ITEMBARCODE, string PROCESO, string IMBOXCODE)
        {
            REDUCCIONHEADER HEADER = new REDUCCIONHEADER();
            List<REDUCCIONLINE> LIST = new List<REDUCCIONLINE>();

            REDUCCIONLINE LINE = new REDUCCIONLINE();
        
            LINE.ITEMBARCODE = ITEMBARCODE;
            LINE.PROCESO = PROCESO;
            LINE.IMBOXCODE = IMBOXCODE;
            LIST.Add(LINE);

            HEADER.LINES = LIST.ToArray();
            string reduccionLines = SerializationService.Serialize(HEADER);
            IM_WMS_ReduccionCajas.CallContext context = new IM_WMS_ReduccionCajas.CallContext { Company = "IMHN" };
            var serviceClient = new M_WMS_Reduccion_CajasClient(GetBinding(), GetEndpointAddrR());

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";


            try
            {
                string dataValidation = string.Format("<INTEGRATION><COMPANY><CODE>{0}</CODE><USER>{1}</USER></COMPANY></INTEGRATION>", context.Company, "servicio_ax");
                var resp = serviceClient.initAsync(context, dataValidation, reduccionLines);

                return resp.Result.response;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        //Entrada en diarios de movimiento
        public string InsertDeleteEntradaMovimientoLine (string JOURNALID, string ITEMBARCODE, string PROCESO )
        {
            MOVIMIENTOHEADER HEADER = new MOVIMIENTOHEADER();
            List<MOVIMIENTOLINE> LIST = new List<MOVIMIENTOLINE>();

            MOVIMIENTOLINE LINE = new MOVIMIENTOLINE();
            LINE.JOURNALID = JOURNALID;
            LINE.ITEMBARCODE = ITEMBARCODE;
            LINE.PROCESO = PROCESO;
            LIST.Add(LINE);

            HEADER.LINES = LIST.ToArray();
            string movimientosLines = SerializationService.Serialize(HEADER);
            ServiceReference1.CallContext context = new ServiceReference1.CallContext { Company = "IMHN" };
            var serviceClient = new M_WMS_Entrada_MovimientoClient(GetBinding(), GetEndpointEntradaMovimiento());

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";


            try
            {
                string dataValidation = string.Format("<INTEGRATION><COMPANY><CODE>{0}</CODE><USER>{1}</USER></COMPANY></INTEGRATION>", context.Company, "servicio_ax");
                var resp = serviceClient.initAsync(context, dataValidation, movimientosLines);

                return resp.Result.response;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        //transferir inventario de un almacen a otro
        //InsertDeleteTransferirMovimientoLine
        public string InsertDeleteTransferirMovimientoLine(string JOURNALID, string ITEMBARCODE, string PROCESO)
        {
            MOVIMIENTOHEADER HEADER = new MOVIMIENTOHEADER();
            List<MOVIMIENTOLINE> LIST = new List<MOVIMIENTOLINE>();

            MOVIMIENTOLINE LINE = new MOVIMIENTOLINE();
            LINE.JOURNALID = JOURNALID;
            LINE.ITEMBARCODE = ITEMBARCODE;
            LINE.PROCESO = PROCESO;
            LIST.Add(LINE);

            HEADER.LINES = LIST.ToArray();
            string movimientosLines = SerializationService.Serialize(HEADER);
            ServiceReferenceIM_WMS_Trasferir_Inventario.CallContext context = new ServiceReferenceIM_WMS_Trasferir_Inventario.CallContext { Company = JOURNALID.StartsWith("DIA-")? "IMHN":"IMGT" };
            var serviceClient = new M_WMS_Trasferir_InventarioClient(GetBinding(), GetEndpointTransferir());

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";


            try
            {
                string dataValidation = string.Format("<INTEGRATION><COMPANY><CODE>{0}</CODE><USER>{1}</USER></COMPANY></INTEGRATION>", context.Company, "servicio_ax");
                var resp = serviceClient.initAsync(context, dataValidation, movimientosLines);

                return resp.Result.response;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string InsertAddInventarioCiclicoTelaLine(List<INVENTARIOCICLICOTELALINE> LIST)
        {
            INVENTARIOCICLICOTELAHEADER HEADER = new INVENTARIOCICLICOTELAHEADER();

            HEADER.LINES = LIST.ToArray();
            string InventarioCiclicoTelaLine = SerializationService.Serialize(HEADER);
            ServiceReferenceIM_WMS_InventarioCiclicoTela.CallContext context = new ServiceReferenceIM_WMS_InventarioCiclicoTela.CallContext { Company = "IMHN" };
            var serviceClient = new M_WMS_InventarioCiclicoTelaClient(GetBinding(), GetEndpointInventarioCiclicotela());
            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";

            try
            {
                string dataValidation = string.Format("<INTEGRATION><COMPANY><CODE>{0}</CODE><USER>{1}</USER></COMPANY></INTEGRATION>", context.Company, "servicio_ax");
                var resp = serviceClient.initAsync(context, dataValidation, InventarioCiclicoTelaLine);

                return resp.Result.response;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public string InsertCajasRecicladas(string qty, string CentroCosto,string diario)
        {
            CAJASRECICLADAS HEADER = new CAJASRECICLADAS();
            List<CAJARECICLADALINE> LIST = new List<CAJARECICLADALINE>();

            CAJARECICLADALINE LINE = new CAJARECICLADALINE();
            LINE.DIARIO = diario;
            LINE.CENTROCOSTOS = CentroCosto;
            LINE.QTY = qty;
            LIST.Add(LINE);

            HEADER.LINES = LIST.ToArray();

            string trasladosLine = SerializationService.Serialize(HEADER);
            IM_WMS_CajaRecladasGP.CallContext context = new IM_WMS_CajaRecladasGP.CallContext { Company = "IMHN" };
            var serviceClient = new M_WMS_CajaRecicladasClient(GetBinding(), GetEndpointAddCaja());

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";


            try
            {
                string dataValidation = string.Format("<INTEGRATION><COMPANY><CODE>{0}</CODE><USER>{1}</USER></COMPANY></INTEGRATION>", context.Company, "servicio_ax");
                var resp = serviceClient.initAsync(context, dataValidation, trasladosLine);

                return resp.Result.response;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<IM_PrepEnvOp_TrasladoResultDTO> MarcarTrasladoComoRecibido(string inventTransferId, string tipo)
        {
            var result = new IM_PrepEnvOp_TrasladoResultDTO
            {
                NoTraslado = inventTransferId,
                IsComplete = true, 
                Message = ""
            };

            IM_PreparacionDeOpGP.CallContext context = new IM_PreparacionDeOpGP.CallContext { Company = "IMHN" };
            var serviceClient = new M_PreparacionDeOpClient(GetBindingPreOp(), GetEndpointAddrPreOp());

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";

            try
            {
                string dataValidation = string.Format("<INTEGRATION><COMPANY><CODE>{0}</CODE><USER>{1}</USER></COMPANY></INTEGRATION>", context.Company, "servicio_ax");
                //Empacar
                //Enviar
                var resp = await serviceClient.initRunAsync(context, dataValidation, inventTransferId, tipo);

                result.IsComplete = resp.response.Contains("OK:");
                result.Message = resp.response;
            }
            catch (FaultException<IM_PreparacionDeOpGP.AifFault> faultEx)
            {
                result.IsComplete = false;
                var fault = faultEx.Detail;

                if (fault.InfologMessageList != null)
                {
                    var mensajes = new List<string>();

                    foreach (var mensaje in fault.InfologMessageList)
                    {
                        string texto = mensaje.Message
                            .Replace("OK:", "\n")
                            .Replace("\t", " ")
                            .Trim();

                        mensajes.Add(texto);
                    }

                    result.Message = string.Join("\n", mensajes);
                }
                else
                {
                    result.Message = faultEx.ToString();
                }
            }

            return result;
        }

        public string CrearDiario(DiarioHeaderDTO headerDTO, DiarioLineasDTO lineasDTO,string accíon)
        {
            DAILYMOVEMENTHEADER HEADER = new DAILYMOVEMENTHEADER();
            DAILYMOVEMENPARAMETRS HEADERPARM = new DAILYMOVEMENPARAMETRS();
         
            
            HEADERPARM.PERSONNELNUMBER = headerDTO.PersonnelNumber;
            HEADERPARM.JOURNALNAME = headerDTO.JournalName;
            HEADERPARM.DESCRIPTION = headerDTO.Description;

            HEADER.PARAM = HEADERPARM;

            DAILYMOVEMENTLINESHEADER LINEHEADER = new DAILYMOVEMENTLINESHEADER();
            DAILYMOVEMENTLINES LINES = new DAILYMOVEMENTLINES();
            List<DAILYMOVEMENTLINEPARAMS> LIST = new List<DAILYMOVEMENTLINEPARAMS>();
            LINEHEADER.JOURNALID = lineasDTO.JournalId;

            foreach (var linea in lineasDTO.Lineas)
            {
                DAILYMOVEMENTLINEPARAMS LINE = new DAILYMOVEMENTLINEPARAMS();
                LINE.ITEMID = linea.ItemId;
                LINE.SITE = linea.Site;
                LINE.WAREHOUSE = linea.WareHouse;
                LINE.COLOR = linea.Color;
                LINE.BATCH = linea.Batch;
                LINE.WMSLOCATION = linea.WMSLocation;
                LINE.TRANSDATE = linea.TransDate;
                LINE.SIZE = linea.Size;
                LINE.QTY = linea.Qty;
                LIST.Add(LINE);
            };

            LINES.DAILYMOVEMENTLINEPARAMS = LIST.ToArray();

            LINEHEADER.LINES = LINES;


            string XML = "";
            if (accíon == "HEADER")
            {
                XML = SerializationService.Serialize(HEADER);
            }
            else if (accíon == "LINES")
            {
                XML = SerializationService.Serialize(LINEHEADER);
            }

            IM_WMS_SRG_ProductDispatchGP.CallContext context = new IM_WMS_SRG_ProductDispatchGP.CallContext { Company = "IMHN" };

            var serviceClient = new M_WMS_SRG_ProductDispatchClient(
                GetBindingGeneric("NetTcpBinding_IM_WMS_SRG_ProductDispatch"),
                GetEndpointGeneric("net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_SRG_ProductDispatchGP")
            );

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";


            try
            {
                string dataValidation = string.Format("<INTEGRATION><COMPANY><CODE>{0}</CODE><USER>{1}</USER></COMPANY></INTEGRATION>", context.Company, "servicio_ax");
              
                var resp = serviceClient.initDiariosAsync(context, accíon,dataValidation,XML);

                return resp.Result.response;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<string> CrearTrasladosPorArticulo(TrasladoDTO trasladoDTO)
        {
            IM_WMS_SRG_ProductDispatchGP.CallContext context = new IM_WMS_SRG_ProductDispatchGP.CallContext
            {
                Company = "IMHN"
            };

            var serviceClient = new M_WMS_SRG_ProductDispatchClient(
                GetBindingGeneric("NetTcpBinding_IM_WMS_SRG_ProductDispatch"),
                GetEndpointGeneric("net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_SRG_ProductDispatchGP")
            ); 

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";

            try
            {
                var xml = new StringBuilder();

               // xml.Append("<TransferOrders>");

                  xml.Append("<TransferOrder>");

                    // HEADER
                    xml.Append("<Header>");
                    xml.AppendFormat("<FromWarehouse>{0}</FromWarehouse>", trasladoDTO.AlmacenDeSalida);
                    xml.AppendFormat("<ToWarehouse>{0}</ToWarehouse>", trasladoDTO.AlmacenDeEntrada);
                    xml.Append("</Header>");

                    // LINES
                    xml.Append("<Lines>");

                    foreach (var linea in trasladoDTO.Lineas)
                    {
                        xml.Append("<Line>");
                        xml.AppendFormat("<ItemId>{0}</ItemId>", linea.ItemId);
                        xml.AppendFormat("<ColorId>{0}</ColorId>", linea.Color);
                        xml.AppendFormat("<BatchNumber>{0}</BatchNumber>", linea.LoteId);
                        xml.AppendFormat("<LocationId>{0}</LocationId>", linea.LocationId);
                        xml.AppendFormat("<Qty>{0}</Qty>", linea.CantidadEnviar);
                        xml.AppendFormat("<sizeId>{0}</sizeId>", linea.SizeId);
                        xml.Append("</Line>");
                    }

                    xml.Append("</Lines>");
                 xml.Append("</TransferOrder>");
                

                //xml.Append("</TransferOrders>");

                string xmlInput = xml.ToString();

                // LLAMADA AL SERVICIO
                var resp = await serviceClient.createTransferAsync(context, xmlInput);

                return resp.response;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<string> CambioIniciadoEstadoOpSerigrafia(OpPorBaseDTO orden)
        {
            IM_WMS_SRG_ChangeOpEST.CallContext context = new IM_WMS_SRG_ChangeOpEST.CallContext
            {
                Company = "IMHN"
            };

            var serviceClient = new M_WMS_SRG_ChangeEstOpClient(
                GetBindingGeneric("NetTcpBinding_IMChangeEstadoOP"),
                GetEndpointGeneric("net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_SRG_ChangEstOP")
            );

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";

            try
            {
                // Construcción del XML basado en la estructura solicitada
                var xml = new StringBuilder();
                xml.Append("<INTEGRATION>");
                xml.Append("<COMPANIES>");

                foreach (var talla in orden.Tallas)
                {
                    xml.Append("<COMPANY>");
                    xml.AppendFormat("<CODE>{0}</CODE>", "IMHN");
                    xml.AppendFormat("<PRODUCTIONORDER>{0}</PRODUCTIONORDER>", orden.ProdMasterId);
                    xml.AppendFormat("<ITEMID>{0}</ITEMID>", orden.ItemIdEstilo);
                    xml.AppendFormat("<SIZEID>{0}</SIZEID>", talla.Talla);
                    xml.Append("</COMPANY>");
                }

                xml.Append("</COMPANIES>");
                xml.Append("</INTEGRATION>");

                string xmlInput = xml.ToString();

                // Llamada al servicio
                var resp = await serviceClient.changeOpStateToStartedUpAsync(context, xmlInput);

                return resp.response.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<List<Respuesta<string>>> CambioANotificadoEstadoOPSerigrafia(List<IM_WMS_SRG_DatosParaNotificarRespuesta> datos)
        {
             List<Respuesta<string>> respuestas = new List<Respuesta<string>>();
             IM_WMS_SRG_ReporteAsFinishedOPGP.CallContext context = new IM_WMS_SRG_ReporteAsFinishedOPGP.CallContext
            {
                Company = "IMHN"
            }
            ;

            var serviceClient = new MReportAsFinishedPRODClient(
                GetBindingGeneric("NetTcpBinding_IMChangeEstadoOP"),
                GetEndpointGeneric("net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IMReportAsFinishedOPGP")
            );

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";

            try
            {
                foreach (var dato in datos)
                {
                    var xml = new StringBuilder();
                    xml.Append("<INTEGRATION>");
                    xml.Append("<COMPANY>");
                    xml.AppendFormat("<PRODID>{0}</PRODID>", dato.PRODID);
                    xml.AppendFormat("<CANTIDADPRIMERAS>{0}</CANTIDADPRIMERAS>", dato.CANTIDADPRIMERAS);
                    xml.AppendFormat("<CANTIDADIRREGULARES>{0}</CANTIDADIRREGULARES>", dato.CANTIDADIRREGULARES);
                    xml.AppendFormat("<DESCRIPCIONDIARIO>{0}</DESCRIPCIONDIARIO>", dato.DESCRIPCIONDIARIO);
                    xml.AppendFormat("<BOXNUM>{0}</BOXNUM>", dato.BOXNUM);
                    xml.AppendFormat("<ACEPTARERROR>{0}</ACEPTARERROR>", dato.ACEPTARERROR);
                    xml.Append("</COMPANY>");
                    xml.Append("</INTEGRATION>");

                    string xmlInput = xml.ToString();

                    // Llamada al servicio
                    var resp = await serviceClient.changeOpStateToNotifyAsFinishedAsync(context, xmlInput);
                    string xmlResponse = resp.response?.ToString();
                    if (!string.IsNullOrEmpty(xmlResponse))
                    {
                        XDocument xmlDoc = XDocument.Parse(xmlResponse);
                        var respuesta = xmlDoc.Descendants("Respuesta").FirstOrDefault()?.Value;
                        var codigo = respuesta.Split('|')[0].Replace("Código:", "").Trim();
                        var estado = respuesta.Split('|')[1].Replace("Estado:", "").Trim();
                        var mensaje = respuesta.Split('|')[3].Replace("Mensaje:", "").Trim();
                        if (estado == "Éxito")
                        {
                            respuestas.Add(new Respuesta<string>
                            {
                                Exito = true,
                                Mensaje = mensaje,
                                Datos = codigo.ToString()
                            });
                        }
                        else
                        {
                            respuestas.Add(new Respuesta<string>
                            {
                                Exito = false,
                                Mensaje = mensaje,
                                Datos = codigo.ToString()
                            });
                        }

                    }
                }
                return respuestas;
            }
            catch (Exception ex)
            {
                respuestas.Add(new Respuesta<string>
                {
                    Exito = false,
                    Mensaje = ex.ToString(),
                });
                return respuestas;  
            }
        }
        public async Task<string> AjustarCantidadPorOP(OpPorBaseDTO orden)
        {
            IM_WMS_SRG_ChangeOpEST.CallContext context = new IM_WMS_SRG_ChangeOpEST.CallContext
            {
                Company = "IMHN"
            };

            var serviceClient = new M_WMS_SRG_ChangeEstOpClient(
                GetBindingGeneric("NetTcpBinding_IMChangeEstadoOP"),
                GetEndpointGeneric("net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_SRG_ChangEstOP")
            );

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";

            try
            {
                // Construcción del XML basado en la estructura solicitada
                var xml = new StringBuilder();
                xml.Append("<INTEGRATION>");
                xml.Append("<COMPANIES>");

                foreach (var talla in orden.Tallas)
                {
                    xml.Append("<COMPANY>");
                    xml.AppendFormat("<CODE>{0}</CODE>", "IMHN");
                    xml.AppendFormat("<PRODUCTIONORDER>{0}</PRODUCTIONORDER>", orden.ProdMasterId);
                    xml.AppendFormat("<ITEMID>{0}</ITEMID>", orden.ItemIdEstilo);
                    xml.AppendFormat("<SIZEID>{0}</SIZEID>", talla.Talla);
                    xml.AppendFormat("<QTYSCHED>{0}</QTYSCHED>", talla.CantidadPreparada);
                    xml.Append("</COMPANY>");
                }

                xml.Append("</COMPANIES>");
                xml.Append("</INTEGRATION>");

                string xmlInput = xml.ToString();

                // Llamada al servicio
                var resp = await serviceClient.quantityAdjustmenByOrderAsync(context, xmlInput);

                return resp.response.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<List<Respuesta<string>>> NotificacionSubcontratacionTejidoPunto(List<IM_WMS_NOTIFICARSUBCONTRATACIONTEJIDOPUNTO> datos)
        {
            List<Respuesta<string>> respuestas = new List<Respuesta<string>>();
            IM_WMS_RecepcionSubcontratacionGP.CallContext context = new IM_WMS_RecepcionSubcontratacionGP.CallContext
            {
                Company = "IMHN"
            }
           ;

            var serviceClient = new M_WMS_RecepcionSubcontratacionClient(
                 GetBindingGeneric("NetTcpBinding_IM_WMS_RecepcionSubcontratacion"),
                 GetEndpointGeneric("net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_RecepcionSubcontratacionGP")
             );

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";


            try
            {
                foreach (var dato in datos)
                {
                    var xml = new StringBuilder();
                    xml.Append("<INTEGRATION>");
                    xml.Append("<COMPANY>");
                    xml.AppendFormat("<PRODID>{0}</PRODID>", dato.PRODID);
                    xml.AppendFormat("<CANTIDADPRIMERAS>{0}</CANTIDADPRIMERAS>", dato.CANTIDADPRIMERAS);
                    xml.AppendFormat("<CANTIDADIRREGULARES>{0}</CANTIDADIRREGULARES>", dato.CANTIDADIRREGULARES);
                    xml.AppendFormat("<DESCRIPCIONDIARIO>{0}</DESCRIPCIONDIARIO>", dato.DESCRIPCIONDIARIO);
                    xml.AppendFormat("<ACEPTARERROR>{0}</ACEPTARERROR>", dato.ACEPTARERROR);
                    xml.AppendFormat("<UBICACION>{0}</UBICACION>", dato.UBICACION);
                    xml.Append("</COMPANY>");
                    xml.Append("</INTEGRATION>");

                    string xmlInput = xml.ToString();

                    // Llamada al servicio
                    var resp = await serviceClient.initReportAsFinishedXmlAsync(context, xmlInput);
                    string xmlResponse = resp.response?.ToString();
                    if (!string.IsNullOrEmpty(xmlResponse))
                    {
                        XDocument xmlDoc = XDocument.Parse(xmlResponse);
                        var respuesta = xmlDoc.Descendants("Respuesta").FirstOrDefault()?.Value;
                        var codigo = respuesta.Split('|')[0].Replace("Código:", "").Trim();
                        var estado = respuesta.Split('|')[1].Replace("Estado:", "").Trim();
                        var mensaje = respuesta.Split('|')[2].Replace("Mensaje:", "").Trim();
                        if (estado == "Éxito")
                        {
                            respuestas.Add(new Respuesta<string>
                            {
                                Exito = true,
                                Mensaje = mensaje,
                                Datos = codigo.ToString()
                            });
                        }
                        else
                        {
                            respuestas.Add(new Respuesta<string>
                            {
                                Exito = false,
                                Mensaje = mensaje,
                                Datos = codigo.ToString()
                            });
                        }

                    }
                }
                return respuestas;
            }
            catch (Exception ex)
            {
                respuestas.Add(new Respuesta<string>
                {
                    Exito = false,
                    Mensaje = ex.ToString(),
                });
                return respuestas;
            }
        }
        public async Task<Respuesta<string>> ConfirmacionRecepcionDePedidoDeCompra(ConfirmacionRecepcionDTO confirmacionRecepcion)
        {
            Respuesta<string> respuestas = new Respuesta<string>();
            ConfirmacionRecepcionXML confirmacionXml = new ConfirmacionRecepcionXML();
            bool esconfirmacion = confirmacionRecepcion.Action == "CONFIRM_PC";
            confirmacionXml .Action = confirmacionRecepcion.Action;
            confirmacionXml.PurchId = confirmacionRecepcion.PurchId;
            confirmacionXml.PackingSlipId = confirmacionRecepcion.PackingSlipId;
            confirmacionXml.QtyReceive = confirmacionRecepcion.QtyReceive;

            string XML = SerializationService.Serialize(confirmacionXml);

            IM_WMS_RecepcionSubcontratacionGP.CallContext context = new IM_WMS_RecepcionSubcontratacionGP.CallContext
            {
                Company = "IMHN"
            };

            var serviceClient = new M_WMS_RecepcionSubcontratacionClient(
                GetBindingGeneric("NetTcpBinding_IM_WMS_RecepcionSubcontratacion"),
                GetEndpointGeneric("net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_RecepcionSubcontratacionGP")
            );

            serviceClient.ClientCredentials.Windows.ClientCredential.UserName = "servicio_ax";
            serviceClient.ClientCredentials.Windows.ClientCredential.Password = "Int3r-M0d@.aX$3Rv";

            try
            {
                var resp = await serviceClient.initConfirmacionRecepcionXMLAsync(context, XML);
                string xmlResponse = resp.response?.ToString();
                if (!string.IsNullOrEmpty(xmlResponse))
                {
                    XDocument xmlDoc = XDocument.Parse(xmlResponse);
                    var respuestaxml = xmlDoc.Descendants("RESPUESTA").FirstOrDefault()?.Value;
                    var estado = respuestaxml.Split('|')[0].Replace("Estado:", "").Trim();
                    var mensaje = respuestaxml.Split('|')[1].Replace("Detalle:", "").Trim();
                    var validacionMensaje = mensaje.Contains("Se ha cancelado la actualización");
                    if (estado == "Éxito" && !validacionMensaje)
                    {
                        respuestas = new Respuesta<string>
                        {
                            Exito = true,
                            Mensaje = esconfirmacion ? "Confirmada exitosamente." : "Recepción realizada exitosamente."
                        };
                    }
                    else
                    {
                        respuestas = new Respuesta<string>
                        {
                            Exito = false,
                            Mensaje = mensaje,
                        };
                    }
                }
                return respuestas;
            }
            catch (Exception ex)
            {
                respuestas = new Respuesta<string>
                {
                    Exito = false,
                    Mensaje = ex.ToString()
                };
                return respuestas;
            }

        }

        private NetTcpBinding GetBinding()
        {
            var netTcpBinding = new NetTcpBinding();
            netTcpBinding.Name = "NetTcpBinding_IM_WMSCreateJournalServices";
            netTcpBinding.MaxBufferSize = int.MaxValue;
            netTcpBinding.MaxReceivedMessageSize = int.MaxValue;

            return netTcpBinding;
        }
        private EndpointAddress GetEndpointAddr()
        {

            string url = "net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_MoviminetoGP";
            string user = "sqladmin@intermoda.com.hn";

            var uri = new Uri(url);
            var epid = new UpnEndpointIdentity(user);
            var addrHdrs = new AddressHeader[0];
            var endpointAddr = new EndpointAddress(uri, addrHdrs); //, epid, addrHdrs);
            return endpointAddr;
        }

        private NetTcpBinding GetBindingPreOp()
        {
            var netTcpBinding = new NetTcpBinding();
            netTcpBinding.Name = "NetTcpBinding_IM_PreparacionDeOp";
            netTcpBinding.MaxBufferSize = int.MaxValue;
            netTcpBinding.MaxReceivedMessageSize = int.MaxValue;

            netTcpBinding.SendTimeout = TimeSpan.FromMinutes(10);
            netTcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(10);

            return netTcpBinding;
        }

        private EndpointAddress GetEndpointAddrPreOp()
        {

            string url = "net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_PreparacionDeOpGP";
            string user = "sqladmin@intermoda.com.hn";

            var uri = new Uri(url);
            var epid = new UpnEndpointIdentity(user);
           
            var endpointAddr = new EndpointAddress(uri, epid);
            return endpointAddr;
        }

        private EndpointAddress GetEndpointAddrT()
        {

             string url = "net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_Traslado_Enviar_RecibirGP";
            string user = "sqladmin@intermoda.com.hn";

            var uri = new Uri(url);
            var epid = new UpnEndpointIdentity(user);
            var addrHdrs = new AddressHeader[0];
            var endpointAddr = new EndpointAddress(uri, addrHdrs); //, epid, addrHdrs);
            return endpointAddr;
        }

        private EndpointAddress GetEndpointAddrR()
        {

            string url = "net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_Reduccion_CajasGP";
            string user = "sqladmin@intermoda.com.hn";

            var uri = new Uri(url);
            var epid = new UpnEndpointIdentity(user);
            var addrHdrs = new AddressHeader[0];
            var endpointAddr = new EndpointAddress(uri, addrHdrs); //, epid, addrHdrs);
            return endpointAddr;
        }

        private EndpointAddress GetEndpointEntradaMovimiento()
        {

            string url = "net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_Entrada_MovimientoGP";
            string user = "sqladmin@intermoda.com.hn";

            var uri = new Uri(url);
            var epid = new UpnEndpointIdentity(user);
            var addrHdrs = new AddressHeader[0];
            var endpointAddr = new EndpointAddress(uri, addrHdrs); //, epid, addrHdrs);
            return endpointAddr;
        }
        private EndpointAddress GetEndpointTransferir()
        {

            string url = "net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_Trasferir_InventarioGP";
            string user = "sqladmin@intermoda.com.hn";

            var uri = new Uri(url);
            var epid = new UpnEndpointIdentity(user);
            var addrHdrs = new AddressHeader[0];
            var endpointAddr = new EndpointAddress(uri, addrHdrs); //, epid, addrHdrs);
            return endpointAddr;
        }

        private EndpointAddress GetEndpointInventarioCiclicotela()
        {

            string url = "net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_InventarioCiclicoTelaGP";
            string user = "sqladmin@intermoda.com.hn";

            var uri = new Uri(url);
            var epid = new UpnEndpointIdentity(user);
            var addrHdrs = new AddressHeader[0];
            var endpointAddr = new EndpointAddress(uri, addrHdrs); //, epid, addrHdrs);
            return endpointAddr;
        }

        private EndpointAddress GetEndpointAddCaja()
        {

            string url = "net.tcp://gim-pro3-AOS:8201/DynamicsAx/Services/IM_WMS_CajaRecladasGP";
            string user = "sqladmin@intermoda.com.hn";

            var uri = new Uri(url);
            var epid = new UpnEndpointIdentity(user);
            var addrHdrs = new AddressHeader[0];
            var endpointAddr = new EndpointAddress(uri, addrHdrs); //, epid, addrHdrs);
            return endpointAddr;
        }

        private NetTcpBinding GetBindingGeneric(string name)
        {
            var netTcpBinding = new NetTcpBinding();
            netTcpBinding.Name = name;
            netTcpBinding.MaxBufferSize = int.MaxValue;
            netTcpBinding.MaxReceivedMessageSize = int.MaxValue;
            netTcpBinding.SendTimeout = TimeSpan.FromMinutes(10);
            netTcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            return netTcpBinding;


        }
        private EndpointAddress GetEndpointGeneric(string url)
        {
            string user = "sqladmin@intermoda.com.hn";

            var uri = new Uri(url);
            var epid = new UpnEndpointIdentity(user);
            var addrHdrs = new AddressHeader[0];
            var endpointAddr = new EndpointAddress(uri, addrHdrs); //, epid, addrHdrs);
            return endpointAddr;
        }


    }
    public static class SerializationService
    {
        public static string Serialize<T>(this T toSerialize)
        {
            var serializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static T DeSerialize<T>(string datos)
        {
            T type;
            var serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(datos))
            {
                type = (T)serializer.Deserialize(reader);
            }

            return type;
        }

    }
}
