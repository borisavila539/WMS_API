using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UbiacacionRollos
{
    public class InventarioRolloPorUbiacionAlmacenDto
    {
        public string NumeroRollo { get; set; } = string.Empty;
        public string NumeroRolloProveedor { get; set; } // Puede ser nulo por el LEFT JOIN
        public string ItemId { get; set; } = string.Empty;
        public string Sitio { get; set; } = string.Empty;
        public string Almacen { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public decimal Cantidad { get; set; } // Microsoft Dynamics AX suele usar decimal para inventarios
    }
}
