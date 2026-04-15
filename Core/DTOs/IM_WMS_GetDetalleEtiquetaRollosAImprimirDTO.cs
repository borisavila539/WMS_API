using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class IM_WMS_GetDetalleEtiquetaRollosAImprimirDTO
    {
        public string Tela { get; set; }
        public string NombreBusqueda { get; set; }
        public string Color { get; set; }
        public string NombreColor { get; set; }
        public string Configuracion { get; set; }
        public string NumeroRollo { get; set; }
        public string NumeroRolloProveedor { get; set; }
        public string Proveedor { get; set; }
        public string NombreProveedor { get; set; }
        public string Almacen { get; set; }
        public string Lote { get; set; }
        public string Ubicacion { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
    }
}
