using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia
{
    public class IM_WMS_SRG_Despacho_Lines_Packing
    {
        public int Id { get; set; }
        public int DespachoId { get; set; }
        public string ProdMasterId { get; set; }
        public string ProdId { get; set; }
        public string ItemId { get; set; }
        public int Box { get; set; }
        public string Size { get; set; }
        public string ColorId { get; set; }
        public int Qty { get; set; }
        public bool Packing { get; set; }
        public string UserPacking { get; set; }
        public DateTime PackingDateTime { get; set; }
        public bool Receive { get; set; }
        public string UserReceive { get; set; }
        public DateTime ReceiveDateTime { get; set; }
    }
}
