using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MB
{
    public class FiltroCajasDisponiblesMB
    {
        public string Orden { get; set; }
        public string Articulo { get; set; }
        public string Color { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
