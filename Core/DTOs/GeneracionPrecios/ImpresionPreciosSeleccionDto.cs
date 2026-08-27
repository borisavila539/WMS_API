using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    public class ImpresionPreciosSeleccionDto
    {
        public string Impresora {  get; set; }
        public string Fecha { get; set; }
        public List<IM_WMS_DetalleImpresionEtiquetasPrecio> Lineas { get; set; }
    }
}
