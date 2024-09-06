using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ControCajasEtiquetado
{
    public class IM_WMS_Control_Cajas_Etiquetado_Detalle_Filtro
    {
        public string Pedido { get; set; }
        public string Ruta { get; set; }
        public string BoxNum { get; set; }
        public string Lote { get; set; }
        public string Empleado { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
