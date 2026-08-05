using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DiseñoEtiqueta
{
    public class SolicitudImpresionDto
    {
        public string Impresora { get; set; } 
        public string Zpl { get; set; }
        public int Cantidad { get; set; }
    }
}
