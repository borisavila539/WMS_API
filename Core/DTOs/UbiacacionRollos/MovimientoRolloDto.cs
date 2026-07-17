using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UbiacacionRollos
{
    public class MovimientoRolloDto
    {
        public string CodigoBarraRollo { get; set; }
        public string SitioOrigen { get; set; }
        public string SitioDestino { get; set; }
        public string AlmacenOrigen { get; set; }
        public string AlmacenDestino { get; set; }
        public string UbicacionOrigen { get; set; }
        public string UbicacionDestino { get; set; }
        public string Cantidad { get; set; }
    }
}
