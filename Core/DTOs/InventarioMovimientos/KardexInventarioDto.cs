namespace Core.DTOs.InventarioMovimientos
{
    public class KardexInventarioDto
    {
        public string Sitio { get; set; }
        public string Almacen { get; set; }
        public string Articulo { get; set; }
        public string Descripcion { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal TotalEntradas { get; set; }
        public decimal TotalSalidas { get; set; }
        public decimal SaldoFinal { get; set; }
    }
}
