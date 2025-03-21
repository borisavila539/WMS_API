using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela
{
    public class IM_WMS_RecTela_UpdateTelaPickingIsScanningDTO
    {
        public int tela_picking_id { get; set; }
        public string journalid { get; set; }
        public string inventserialid { get; set; }
        public string vendroll { get; set; }
        public int user { get; set; }
        public DateTime created_date { get; set; }
        public bool is_scanning { get; set; }
        public DateTime update_date { get; set; }
        public string location { get; set; }
        public string ItemId { get; set; }
        public int TelaPickingDefectoId { get; set; }
    }
}
