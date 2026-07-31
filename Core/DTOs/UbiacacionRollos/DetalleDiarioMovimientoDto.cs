using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UbiacacionRollos
{
    public class DetalleDiarioMovimientoDto
    {
        public string JournalId { get; set; } = string.Empty;
        public string Comprobante { get; set; } = string.Empty;
        public string ItemId { get; set; } = string.Empty;
        public string NumeroRollo { get; set; } = string.Empty;
        public string AlmacenDesde { get; set; } = string.Empty;
        public string UbicacionDesde { get; set; } = string.Empty;
        public string AlmacenPara { get; set; } = string.Empty;
        public string UbicacionPara { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
    }
}
