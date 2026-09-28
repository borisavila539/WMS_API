using System.Collections.Generic;

namespace Core.DTOs.RecepcionFacturasCD
{
    // Alta de varias cuentas a la vez, con el mismo motivo.
    public class CuentasExcluidasRecepcionCDRequest
    {
        public List<string> Cuentas { get; set; }
        public string Motivo { get; set; }
        public string Usuario { get; set; }
    }
}
