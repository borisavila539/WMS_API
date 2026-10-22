using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    // IdDireccion = RecId de LOGISTICSPOSTALADDRESS en AX (WMSPICKINGROUTE.DELIVERYPOSTALADDRESS).
    // Moneda: LEMPIRAS | DOLAR. Si es DOLAR, la impresión de etiquetas usa PrecioDolar.
    public class TiendaGeneracionPrecioDto
    {
        public int Id { get; set; }
        public string CodigoCliente { get; set; }
        public string Tienda { get; set; }
        public long IdDireccion { get; set; }
        public string Descripcion { get; set; }
        public string Moneda { get; set; }
    }
}
