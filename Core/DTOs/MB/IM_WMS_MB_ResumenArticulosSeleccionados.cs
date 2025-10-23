using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MB
{
    public class IM_WMS_MB_ResumenArticulosSeleccionados
    {
        public string Articulo { get; set; }
        public string descripcioMB { get; set; }
        public string Talla { get; set; }
        public string NombreColor { get; set; }
        public string Color { get; set; }
        public int QTYTotal { get; set; }
        public int QTYCajas { get; set; }
    }
}
