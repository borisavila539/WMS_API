using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT
{
    public class IM_WMS_Consulta_OP_DetalleDTO
    {
        public string ProdCutSheetID { get; set; }
        public string ProdID { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Cortado { get; set; }
        public int Receive { get; set; }
        public int Segundas { get; set; }
        public int terceras { get; set; }
        public int cajas { get; set; }
        public int DespachoID { get; set; }
    }
}
