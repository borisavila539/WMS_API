using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT.Liquidacion
{
    public class IM_WMS_DespachoPT_OrdenesRecibidasDepachoDTO
    {
        public string  NumeroOPPakingList { get; set; }
        public string ProdCutSheetID { get; set; }
        public string VendAccount { get; set; }
        public string PurchId { get; set; }
        public string TieneDiarioRecepcion { get; set; }
    }
}
