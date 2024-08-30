using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.RecepcionUbicacionCajas
{
    public class Filtros
    {
        public string Lote { get; set; }
        public string Orden { get; set; }
        public string Articulo { get; set; }
        public string Talla { get; set; }
        public string Color { get; set; }
        public string Ubicacion { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public string Tipo { get; set; }

    }
}
