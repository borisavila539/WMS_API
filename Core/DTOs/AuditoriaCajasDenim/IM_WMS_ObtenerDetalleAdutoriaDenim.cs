using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.AuditoriaCajasDenim
{
    public class IM_WMS_ObtenerDetalleAdutoriaDenim
    {
        public int Id { get; set; }
        public string OP { get; set; }
        public string Articulo { get; set; }
        public int NumeroCaja { get; set; }
        public int Cantidad { get; set; }
        public string ubicacion { get; set; }
        public Boolean Enviado { get; set; }
        public string usuario { get; set; }
        public int Auditado { get; set; }
        public string Talla { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Lote { get; set; }
        public string COlor { get; set; }

    }
}
