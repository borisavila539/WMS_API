using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UbiacacionRollos
{
    public class InventarioRolloPorAlmacenDto
    {
        public string Articulo { get; set; }
        public string NombreBusqueda { get; set; }
        public string Color { get; set; }
        public string NombreColor { get; set; }
        public string Ancho { get; set; }
        public string NumeroSerie { get; set; }
        public string Proveedor { get; set; }
        public string NombreProveedor { get; set; }
        public string Almacen { get; set; }
        public string Lote { get; set; }
        public string Ubicacion { get; set; }
        public decimal InventarioFisico { get; set; }
        public decimal FisicoReservada { get; set; }
    }
}
