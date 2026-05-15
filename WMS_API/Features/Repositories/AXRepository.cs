using Core.DTOs;
using Core.DTOs.Cajasrecicladas;
using Core.DTOs.IM_PrepEnvOp;
using Core.DTOs.InventarioCiclicoTela;
using Core.DTOs.Serigrafia;
using Core.DTOs.Serigrafia.ClaseRespuesta;
using Core.Interfaces;
using IM_PreparacionDeOpGP;
using IM_WMS_CajaRecladasGP;
using IM_WMS_MoviminetoWS;
using IM_WMS_ReduccionCajas;
using IM_WMS_SRG_ChangeOpEST;
using IM_WMS_SRG_ProductDispatchGP;
using IM_WMS_SRG_ReporteAsFinishedOPGP;
using IM_WMS_Traslado_Enviar_Recibir;
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
