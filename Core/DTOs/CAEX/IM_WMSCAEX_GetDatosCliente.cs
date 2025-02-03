using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.CAEX
{
    public class IM_WMSCAEX_GetDatosCliente
    {
        public string Departamento { get; set; }
        public string Municipio { get; set; }
        public string Direccion { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
    }
}
