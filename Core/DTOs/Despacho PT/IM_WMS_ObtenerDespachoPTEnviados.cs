using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT
{
    public class IM_WMS_ObtenerDespachoPTEnviados
    {
        public int ID { get; set; }
        public string ProdID { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public DateTime fechaPacking { get; set; }
        public string ItemID { get; set; }
        public int Box { get; set; }
        public int QTY { get; set; }
        public bool NeedAudit { get; set; }
        public bool Receive { get; set; }
    }
}
