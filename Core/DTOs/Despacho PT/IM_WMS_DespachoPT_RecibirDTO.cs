using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT
{
    public class IM_WMS_DespachoPT_RecibirDTO
    {
        public string PRODID { get; set; }
        public int BOX { get; set; }
        public bool Receive { get; set; }
        public string UserReceive { get; set; }
        public DateTime FechaReceive { get; set; }
        public int DespachoID { get; set; }
    }
}
