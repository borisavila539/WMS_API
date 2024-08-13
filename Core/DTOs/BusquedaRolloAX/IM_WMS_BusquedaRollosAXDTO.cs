using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.BusquedaRolloAX
{
    public class IM_WMS_BusquedaRollosAXDTO
    {
        public string ITEMID { get; set; }
        public string INVENTLOCATIONID { get; set; }
        public string INVENTSERIALID { get; set; }
        public string APVENDROLL { get; set; }
        public string INVENTCOLORID { get; set; }
        public string COLORNAME { get; set; }
        public string INVENTBATCHID { get; set; }
        public decimal PHYSICALINVENT { get; set; }
        public string WMSLOCATIONID { get; set; }
        public string REFERENCE { get; set; }
        public string CONFIGID { get; set; }

    }
}
