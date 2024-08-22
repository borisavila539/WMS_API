using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Contabilidad
{
    public class ReporteProveedoreDTO
    {
        public DateTime Fecha { get; set; }
        public string Proveedor { get; set; }
        public string Nombre { get; set; }
        public string Grupo { get; set; }
        public decimal SaldoReal { get; set; }
        public decimal Anticipos { get; set; }
    }
}
