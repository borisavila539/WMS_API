using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Devoluciones
{
    public class IM_WMS_Devolucion_Busqueda
    {
        public int ID { get; set; }
        public string NumDevolucion { get; set; } 
        public DateTime FechaCrea { get; set; }
        public string NumeroRMA { get; set; } 
        public DateTime FechaCreacionAX { get; set; } 
        public string Asesor { get; set; } 
        public string Descricpcion { get; set; }
        public int TotalUnidades { get; set; }
        public string Camion { get; set; }
        public int IDConsolidado { get; set; }
    }
}
