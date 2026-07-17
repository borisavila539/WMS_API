using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UbiacacionRollos
{
    public class RespuestaConsultarRollo
    {
        public string NumeroRollo { get; set; }
        public string NumeroRolloProveedor { get; set; }
        public string ItemId { get; set; }
        public string Sitio { get; set; }
        public string Almacen { get; set; }
        public string Ubicacion { get; set; }
        public string Cantidad { get; set; }
    }
}
