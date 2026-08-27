using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    public class ConfiguracionPrecioImportResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = "";

        public List<ConfiguracionPrecioDto> Data { get; set; }
            = new List<ConfiguracionPrecioDto>();
    }
}
