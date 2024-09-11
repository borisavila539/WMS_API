using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    public class IM_WMS_ObtenerDetalleGeneracionPrecios
    {
        public string CuentaCliente { get; set; }
        public string CodigoBarra { get; set; }
        public string Articulo { get; set; }
        public string Base { get; set; }
        public string Estilo { get; set; }
        public string IDColor { get; set; }
        public string Referencia { get; set; }
        public string Descripcion { get; set; }
        public string ColorDescripcion { get; set; }
        public string Talla { get; set; }
        public string Descripcion2 { get; set; }
        public string Categoria { get; set; }
        public int Cantidad { get; set; }
        public decimal Costo { get; set; }
        public string Departamento { get; set; }
        public string SubCategoria { get; set; }
        public decimal Precio { get; set; }
    }
}
