using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Devoluciones
{
    public class IM_WMS_ObtenerEstructuraDefectosDevolucion
    {
        public int ID { get; set; }
        public string Estructura { get; set; }
        public string Defecto { get; set; }
        public bool Activo { get; set; }

    }
}
