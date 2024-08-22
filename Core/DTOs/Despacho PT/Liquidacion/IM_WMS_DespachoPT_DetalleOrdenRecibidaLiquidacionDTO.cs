using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT.Liquidacion
{
    public class IM_WMS_DespachoPT_DetalleOrdenRecibidaLiquidacionDTO
    {
        public int Numero { get; set; }
        public int secuencia { get; set; }
        public string ProdID { get; set; }
        public string ProdCutSheetID { get; set; }
        public string Size { get; set; }
        public int Enviado { get; set; }
        public int Recibido { get; set; }

    }
}
