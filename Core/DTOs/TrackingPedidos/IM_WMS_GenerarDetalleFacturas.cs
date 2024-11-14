using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.TrackingPedidos
{
    public class IM_WMS_GenerarDetalleFacturas
    {
        public string CuentaCliente { get; set; }
        public string NombreCliente { get; set; }
        public DateTime FechaIngresoPedido { get; set; }
        public string PedidoVenta { get; set; }
        public string ListaEmpaque { get; set; }
        public DateTime FechaGeneracionListaEmpaque { get; set; }
        public DateTime FechaListaEmpaqueCompletada { get; set; }
        public string Albaran { get; set; }
        public DateTime FechaAlbaran { get; set; }
        public string Factura { get; set; }
        public DateTime FechaFactura { get; set; }
        public DateTime FechaRececpcionCD { get; set; }
        public DateTime FechaDespacho { get; set; }
        public string Ubicacion { get; set; }
        public int Cajas { get; set; }
        public int QTY { get; set; }
        public string EstadoPedido { get; set; }
        public int Paginas { get; set; }
        public string Responsable { get; set; }
        public string BFPSEASONID { get; set; }
        public string Tienda { get; set; }

    }
}
