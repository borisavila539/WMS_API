using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.TejidoPunto
{
    public  class IM_WMS_NOTIFICARSUBCONTRATACIONTEJIDOPUNTO
    {
        public string PRODID { get; set; }
        public int CANTIDADPRIMERAS { get; set; }
        public int CANTIDADIRREGULARES { get; set; }
        public string DESCRIPCIONDIARIO { get; set; }
        public string ACEPTARERROR { get; set; }
        public string UBICACION { get; set; }
    }
}
