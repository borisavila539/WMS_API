using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    public class ConfiguracionPrecioDto
    {
        public int Id { get; set; }
        public string Categoria { get; set; }
        public string Cuenta { get; set; }
        public string Coleccion { get; set; }
        public string Subcategoria { get; set; }
        public string Base { get; set; }
        public string IdColor { get; set; }
        public string Talla { get; set; }
        public decimal Costo { get; set; }
        public decimal Precio { get; set; }
        public decimal Margen { get; set; }
    }
}
