using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.RecepcionUbicacionCajas
{
    public class SP_GetAllBoxesReceived
    {
        public string Lote { get; set; }
        public string OP { get; set; }
        public string Articulo { get; set; }
        public string NumeroDeCaja { get; set; }
        public string Talla { get; set; }
        public int CantidadEnCaja { get; set; }
        public DateTime FechaDeEnvio { get; set; }
        public DateTime FechaDeRecepcion { get; set; }
        public string Color { get; set; }
        public string ubicacion { get; set; }
        public int Paginas { get; set; }
    }
}
