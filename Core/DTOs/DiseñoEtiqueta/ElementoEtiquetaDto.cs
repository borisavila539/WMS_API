using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DiseñoEtiqueta
{
    public class ElementoEtiquetaDto
    {
        public string Id { get; set; } 
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Texto { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int FontSize { get; set; }
        public bool Bold { get; set; }
    }
}
