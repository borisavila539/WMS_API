using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ControCajasEtiquetado
{
    public class IM_WMS_Control_Cajas_Etiquetado_Detalle
    {
        public string Pedido { get; set; }
        public string Ruta { get; set; }
        public string  CodigoCaja { get; set; }
        public string NumeroCaja { get; set; }
        public int unidades { get; set; }
        public string BFPLINEID { get; set; }
        public string Temporada { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public DateTime Tiempo { get; set; }
        public string Empleado { get; set; }
        public int Paginas { get; set; }


    }
}
