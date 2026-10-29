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

        // Criterio de búsqueda con el que se obtuvieron las líneas. El API vuelve a consultar
        // con él antes de imprimir, para no confiar en RequiereConfirmacion/Precio del cliente.
        public string Pedido { get; set; }
        public string Ruta { get; set; }
        public string Caja { get; set; }

        public List<IM_WMS_DetalleImpresionEtiquetasPrecio> Lineas { get; set; }
    }
}
