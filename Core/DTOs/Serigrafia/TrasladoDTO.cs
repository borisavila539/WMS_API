using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia
{
    public class TrasladoDTO
    {
        public string AlmacenDeSalida { get; set; }
        public string AlmacenDeEntrada { get; set; }
        public List<LineasTrasladoDTO> Lineas { get; set; }
    }
}
