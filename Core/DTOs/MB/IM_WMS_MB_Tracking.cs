using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MB
{
    public  class IM_WMS_MB_Tracking
    {
        public string Articulo { get; set; }
        public string DescripcioMB { get; set; }
        public string Talla { get; set; }
        public string Color { get; set; }
        public int Cantidad { get; set; }
        public int QTYPicking { get; set; }
        public int QTYPacking { get; set; }


    }
}
