using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.DTOs;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using IM_WMS_MoviminetoWS;

namespace WMS_API.Features.Repositories
{
    public class AXRepository : IAX
    {
        public string InsertMovimientoLine(string JOURNALID, string ITEMBARCODE)
        {
            object obj = new object();
            MOVIMIENTOHEADER HEADER = new MOVIMIENTOHEADER();
            List<MOVIMIENTOLINE> LIST = new List<MOVIMIENTOLINE>();

            MOVIMIENTOLINE LINE = new MOVIMIENTOLINE();
            LINE.JOURNALID = JOURNALID;
            LINE.ITEMBARCODE = ITEMBARCODE;

            LIST.Add(LINE);

            HEADER.LINES = LIST.ToArray();
            string movimientosLines = SerializationService.Serialize(HEADER);
            CallContext context = new CallContext { Company = "IMHN" };
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

            string url = "net.tcp://gim-dev-AOS:8201/DynamicsAx/Services/IM_WMS_MoviminetoGP";
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
