using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Cajasrecicladas
{
    public class IM_WMS_InsertCajasRecicladashistorico
    {
        public int Id { get; set; }
        public string Camion { get; set; }
        public string Chofer { get; set; }
        public string CentroCostos { get; set; }
        public int QTY { get; set; }
        public DateTime Fecha { get; set; }
        public string usuario { get; set; }
        public string Diario { get; set; }

    }
}
