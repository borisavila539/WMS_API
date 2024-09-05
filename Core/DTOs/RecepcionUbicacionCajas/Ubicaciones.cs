using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.RecepcionUbicacionCajas
{
   public  class Ubicaciones
    {
        public string ubicacion { get; set; }
        public string Camion { get; set; }
        public string Usuario { get; set; }
        public string[] Ordenes { get; set; }

    }
}
