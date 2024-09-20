using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    public class IM_WMS_DetalleImpresionEtiquetasPrecio
    {
        public string Nombre { get; set; }
        public string CodigoBarra { get; set; }
        public string Articulo { get; set; }
        public string Descripcion { get; set; }
        public string Estilo { get; set; }
        public string Talla { get; set; }
        public string IDColor { get; set; }
        public decimal Precio { get; set; }
        public int QTY { get; set; }
        public string Moneda { get; set; }
        public Boolean Decimal { get; set; }
        public string IMIB_BOXCODE { get; set; }
    }
}
