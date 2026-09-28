namespace Core.DTOs.RecepcionFacturasCD
{
    // Resultado por cuenta del alta múltiple.
    public class ResultadoCuentaExcluidaRecepcionCDDto
    {
        public string CuentaCliente { get; set; }
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public int Id { get; set; }
    }
}
