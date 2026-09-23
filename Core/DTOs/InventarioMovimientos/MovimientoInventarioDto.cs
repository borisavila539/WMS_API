using System;

namespace Core.DTOs.InventarioMovimientos
{
    public class MovimientoInventarioDto
    {
        public string Sitio { get; set; }
        public string Almacen { get; set; }
        public string Articulo { get; set; }
        public string Descripcion { get; set; }
        public string Movimiento { get; set; }
        public string TipoReferencia { get; set; }
        public string Documento { get; set; }
        public string UsuarioCrea { get; set; }
        public string UsuarioRegistra { get; set; }
        public DateTime FechaFisica { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Entrada { get; set; }
        public decimal Salida { get; set; }
        public string Albaran { get; set; }
        public string Factura { get; set; }
        public string Comprobante { get; set; }
        public string Color { get; set; }
        public string Talla { get; set; }
        public string Lote { get; set; }
        public string NumSerie { get; set; }
        public string Ubicacion { get; set; }
        public int EstadoSalida { get; set; }
        public int EstadoEntrada { get; set; }
    }
}
