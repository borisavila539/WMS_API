using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.TrackingPedidos
{
    public class TrackingPedidosFiltro
    {
        public string CuentaCliente {get;set;}
        public string Pedido { get; set; }
        public string ListaEmpaque { get; set; }
        public string Albaran { get; set; }
        public string Factura { get; set; }
        public int page { get; set; }
        public int  size { get; set; }

    }
}
