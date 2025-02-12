using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.CAEX.Guia
{
    public class RequestGenerarGuia
    {
        public string Cliente { get; set; }
        public int Cajas { get; set; }
        public string usuario { get; set; }
        public List<IM_WMSCAEX_ObtenerDetallePickingRouteID> ListasEmpaque { get; set; }
    }
}
