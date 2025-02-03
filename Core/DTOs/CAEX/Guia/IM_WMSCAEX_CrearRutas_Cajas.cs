using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.CAEX.Guia
{
    public class IM_WMSCAEX_CrearRutas_Cajas
    {
        public int idCaex_Rutas { get; set; }
        public int NumeroPieza { get; set; }
        
        public string NumeroGuia { get; set; }
        public string URLConsulta { get; set; }
    }
}
