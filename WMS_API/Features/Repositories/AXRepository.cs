using Core.DTOs;
using Core.Interfaces;
using IM_WMS_MoviminetoWS;
using IM_WMS_ReduccionCajas;
using IM_WMS_Traslado_Enviar_Recibir;
using ServiceReferenceIM_WMS_Trasferir_Inventario;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
            object obj = new object();
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
            object obj = new object();
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
            object obj = new object();
            MOVIMIENTOHEADER HEADER = new MOVIMIENTOHEADER();
            List<MOVIMIENTOLINE> LIST = new List<MOVIMIENTOLINE>();

            MOVIMIENTOLINE LINE = new MOVIMIENTOLINE();
            LINE.JOURNALID = JOURNALID;
            LINE.ITEMBARCODE = ITEMBARCODE;
            LINE.PROCESO = PROCESO;
            LIST.Add(LINE);

            HEADER.LINES = LIST.ToArray();
            string movimientosLines = SerializationService.Serialize(HEADER);
            ServiceReferenceIM_WMS_Trasferir_Inventario.CallContext context = new ServiceReferenceIM_WMS_Trasferir_Inventario.CallContext { Company = "IMHN" };
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

            string url = "net.tcp://gim-dev-AOS:8201/DynamicsAx/Services/IM_WMS_Entrada_MovimientoGP";
            string user = "sqladmin@intermoda.com.hn";

            var uri = new Uri(url);
            var epid = new UpnEndpointIdentity(user);
            var addrHdrs = new AddressHeader[0];
            var endpointAddr = new EndpointAddress(uri, addrHdrs); //, epid, addrHdrs);
            return endpointAddr;
        }
        private EndpointAddress GetEndpointTransferir()
        {

            string url = "net.tcp://gim-dev-AOS:8201/DynamicsAx/Services/IM_WMS_Trasferir_InventarioGP";
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
