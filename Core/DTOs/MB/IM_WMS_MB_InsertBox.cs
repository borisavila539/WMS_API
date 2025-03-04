using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MB
{
    public class IM_WMS_MB_InsertBox
    {
        public int ID { get; set; }
        public string Lote { get; set; }
        public string Orden { get; set; }
        public string Articulo { get; set; }
        public int NumeroCaja { get; set; }
        public int Talla { get; set; }
        public int Cantidad { get; set; }
        public string Color { get; set; }
        public string NombreColor { get; set; }
        public string UbicacionRecepcion { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public int IDConsolidado { get; set; }
        public string Camion { get; set; }
        public string usuarioRecepcion { get; set; }

    }
}
