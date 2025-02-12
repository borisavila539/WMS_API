using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.CAEX
{
    public class IM_WMSCAEX_ObtenerDetallePickingRouteID
    {
        public string SalesID { get; set; }
        public string PickingRouteID { get; set; }
        public string CuentaCliente { get; set; }
        public string Cliente { get; set; }
        public string Telefono { get; set; }
        public string County { get; set; }
        public string Codigo { get; set; }
        public string Empacador { get; set; }
        public int Cajas { get; set; }
        public string Embarque { get; set; }
        public string Address { get; set; }
    }
}
