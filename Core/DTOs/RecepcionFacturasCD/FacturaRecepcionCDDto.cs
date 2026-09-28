using System;

namespace Core.DTOs.RecepcionFacturasCD
{
    // Columnas que devuelve dbo.usp_PedidosFacturasRecibidasCD.
    public class FacturaRecepcionCDDto
    {
        public string PV { get; set; }
        public string Ruta { get; set; }
        public string NombreCliente { get; set; }
        public string Cuenta { get; set; }
        public string Factura { get; set; }
        public DateTime FechaGeneracionFactura { get; set; }
        public DateTime? FechaRecibidaEnCD { get; set; }
        public string Ubicacion { get; set; }
        public string Recibido { get; set; }
    }
}
