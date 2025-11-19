using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MB
{
    public class IM_WMS_MB_ResumenDespachoPallet
    {
        public int ID { get; set; }
        public int IDConsolidado { get; set; }
        public int NumeroCaja { get; set; }
        public string Lote { get; set; }
        public string Orden { get; set; }
        public string Articulo { get; set; }
        public string DescripcioMB { get; set; }
        public string talla { get; set; }
        public string Color { get; set; }
        public int Cantidad { get; set; }
        public string UbicacionRecepcion { get; set; }
        public string Picking { get; set; }
        public string Packing { get; set; }
        public string Pallet { get; set; }

    }
}
