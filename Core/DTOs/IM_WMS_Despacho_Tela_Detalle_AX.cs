using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class IM_WMS_Despacho_Tela_Detalle_AX
    {
        public string TRANSFERID { get; set; }
        public string INVENTSERIALID { get; set; }
        public string APVENDROLL { get; set; }
        public decimal QTYTRANSFER { get; set; }
        public string NAME { get; set; }
        public string CONFIGID { get; set; }
        public string INVENTBATCHID { get; set; }
        public string ITEMID { get; set; }
        public string BFPITEMNAME { get; set; }
    }
}
