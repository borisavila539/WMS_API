using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class DespachoTelasDetalleDTO
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
        public bool Picking { get; set; }
        public bool Packing { get; set; }
        public bool receive { get; set; }

    }
}
