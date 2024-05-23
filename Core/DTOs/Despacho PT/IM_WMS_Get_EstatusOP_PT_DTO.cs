using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT
{
    public class IM_WMS_Get_EstatusOP_PT_DTO
    {
        public string UserPicking { get; set; }
        public string Prodcutsheetid { get; set; }
        public string prodid { get; set; }
        public string Size { get; set; }
        public int Escaneado { get; set; }
        public int Cortado { get; set; }
    }
}
