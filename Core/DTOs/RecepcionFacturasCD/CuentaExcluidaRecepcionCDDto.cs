using System;

namespace Core.DTOs.RecepcionFacturasCD
{
    // Columnas que devuelve IM_WMS_RecepcionFacturasCD_CuentasExcluidas_Obtener.
    public class CuentaExcluidaRecepcionCDDto
    {
        public int Id { get; set; }
        public string CuentaCliente { get; set; }
        public string NombreCliente { get; set; }
        public string Motivo { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
