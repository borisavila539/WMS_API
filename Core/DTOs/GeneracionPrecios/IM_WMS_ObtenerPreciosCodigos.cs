using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    public class IM_WMS_ObtenerPreciosCodigos
    {
        public int ID { get; set; }
        public string CuentaCliente { get; set; }
        public string Estilo { get; set; }
        public string IDColor { get; set; }
        public string Talla { get; set; }
        public decimal Costo { get; set; }
        public decimal Precio { get; set; }
    }
}
