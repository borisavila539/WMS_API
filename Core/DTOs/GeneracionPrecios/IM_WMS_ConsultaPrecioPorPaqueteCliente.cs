using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    public class IM_WMS_ConsultaPrecioPorPaqueteCliente
    {
        public string codigo { get; set; }
        public string Talla    { get; set; }
        public string Color    { get; set; }
        public int precio { get; set; }

    }
}
