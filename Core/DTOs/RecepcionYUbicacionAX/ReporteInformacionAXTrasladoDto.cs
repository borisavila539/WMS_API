using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.RecepcionYUbicacionAX
{
    public class ReporteInformacionAXTrasladoDto
    {
        public int EstadoTraslado { get; set; }
        public int CantidadCajas { get; set; }
        public int CantidadOP { get; set; }

        public decimal CantidadUnidades { get; set; }
        public string Propuesta { get; set; }
        public string OP { get; set; }
        public string NumeroCaja { get; set; }
        public string Talla { get; set; }
        public decimal Cantidad { get; set; }
        public string Color { get; set; }
        public string Lote { get; set; }
        public string CodigoArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public string DesdeAlmacen { get; set; }
        public string HastaAlmacen { get; set; }
        public string CategoriaCaja { get; set; }
        public string LoteDescripcion { get; set; }
        public string TrasladoPrimerasAX { get; set; }
        public string TrasladoSegundasAX { get; set; }
        public string Conductor { get; set; }
        public string CodigoRecibido { get; set; }
        public string Camion { get; set; }
        public int Recibido { get; set; }
        public DateTime? FechaValidado { get; set; }
    }
}
