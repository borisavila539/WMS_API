using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT
{
    public class IM_WMS_Detalle_Auditoria_CajaDTO
    {
        public string ItemID { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int QTY { get; set; }
        public int Auditada { get; set; }
    }
}
