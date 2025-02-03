using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.CAEX.Guia
{
    public class IM_WMS_CAEX_CrearRutas
    {
        public int ID { get; set; }
        public string Ruta { get; set; }
        public int? RutaIDConsolidado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Usuario { get; set; }
    }
}
