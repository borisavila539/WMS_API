using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
        public class IM_WMS_HistoricoGeneracionPrecioCodigoDto
        {
            public int Id { get; set; }
            public bool TieneIrregularidad { get; set; }
            public string CuentaCliente { get; set; } = string.Empty;
            public string PedidoVenta { get; set; } = string.Empty;
            public string CodigoBarra { get; set; } = string.Empty;
            public string Articulo { get; set; } = string.Empty;
            public string Base { get; set; }
            public string Estilo { get; set; }
            public string IdColor { get; set; }
            public string Referencia { get; set; }
            public string Descripcion { get; set; }
            public string ColorDescripcion { get; set; }
            public string Talla { get; set; }
            public string Descripcion2 { get; set; }
            public string Categoria { get; set; }
            public int Cantidad { get; set; }
            public decimal CostoAX { get; set; }
            public decimal CostoConfiguracionPrecio { get; set; }
            public decimal Precio { get; set; }
            public string Departamento { get; set; }
            public string SubCategoria { get; set; }
            public string Coleccion { get; set; }

            // Control de Estado y Auditoría de Confirmación
            public bool Confirmado { get; set; }
            public bool EsConfirmacionMasiva { get; set; }
            public string UsuarioConfirmacion { get; set; }
            public string DeliveryName { get; set; }
            public DateTime FechaConfirmacion { get; set; }
            public string HostNameConfirmacion { get; set; }
        }
}
