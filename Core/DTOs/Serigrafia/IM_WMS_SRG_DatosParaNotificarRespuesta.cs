using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia
{
    public class IM_WMS_SRG_DatosParaNotificarRespuesta
    {
        public string PRODID { get; set; }
        public int CANTIDADPRIMERAS { get; set; }
        public int CANTIDADIRREGULARES { get; set; }
        public string DESCRIPCIONDIARIO { get; set; }
        public string BOXNUM { get; set; }
        public string ACEPTARERROR { get; set; }
    }
}
