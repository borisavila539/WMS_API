using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class IM_WMS_FiltroDetalleEtiquetaRolloDTO
    {
        public string InventBatchId { get; set; }          // OBLIGATORIO
        public string TipoEtiqueta { get; set; } = "";     // OBLIGATORIO
        public string InventColorId { get; set; } = "";    // OPCIONAL
        public string ConfigId { get; set; } = "";         // OPCIONAL
        public string Proveedor { get; set; } = "";        // OPCIONAL
        public string RolloProveedor { get; set; } = "";   // OPCIONAL
        public decimal? Cantidad { get; set; }             // OPCIONAL
    }
}
