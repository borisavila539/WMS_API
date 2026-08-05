using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DespachoTela
{
    public class DetalleDespachoTelaDto
    {
        public long Fila { get; set; }
        public string Transferencia { get; set; } = string.Empty;
        public DateTime? FechaEnvio { get; set; }
        public string DesdeAlmacen { get; set; } = string.Empty;
        public string HastaAlmacen { get; set; } = string.Empty;
        public string CodigoArticulo { get; set; } = string.Empty;
        public string Ancho { get; set; } = string.Empty;
        public string Lote { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string NombreColor { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public string NumeroDeSerie { get; set; } = string.Empty;
        public Decimal Cantidad { get; set; }
        public string Revisado { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }
}

