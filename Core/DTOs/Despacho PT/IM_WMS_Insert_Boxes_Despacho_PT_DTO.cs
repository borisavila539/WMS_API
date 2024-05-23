using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT
{
    public class IM_WMS_Insert_Boxes_Despacho_PT_DTO
    {
        public int ID { get; set; }
        public string ProdID { get; set; }
        public string ProdCutSheetID { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string UserPicking { get; set;}
        public string ItemID { get; set; }
        public int Box { get; set; }
        public int QTY { get; set; }

    }
}
