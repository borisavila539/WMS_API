using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT.Liquidacion
{
    public class IM_WMS_DespachosRecibidosLiquidacionDTO
    {
        public int ID { get; set; }
        public string Driver { get; set; }
        public string Truck { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int Almacen { get; set; }
    }
}
