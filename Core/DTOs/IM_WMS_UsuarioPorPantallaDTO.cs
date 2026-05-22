using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class IM_WMS_UsuarioPorPantallaDTO
    {
        public int ID { get; set; }
        public string NumeroColaborador { get; set; }
        public string NOMBRECOMPLETO { get; set; }
        public string PANTALLA { get; set; }
        public bool PERMISOADMIN { get; set; }
        public bool PERMISOLECTURA { get; set; }
    }
}
