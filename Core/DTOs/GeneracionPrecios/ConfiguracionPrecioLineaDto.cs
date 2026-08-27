using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    public class ConfiguracionPrecioLineaDto
    {
        public bool ExisteConfiguracion { get; set; }

        public string TipoTalla { get; set; }

        public decimal Costo { get; set; }

        public decimal Precio { get; set; }
    }
}
