using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela
{
    public class IM_WMS_RecTela_GetListTelasFilterDTO
    {
        public string journalid { get; set; }
        public string itemId { get; set; }
        public string nameColor { get; set; }
        public string inventBatchId { get; set; }
        public decimal QTY { get; set; }
        public string inventserialid { get; set; }
        public string vendroll { get; set; }
        public string configid { get; set; }
        public string inventserialid_picking { get; set; }

        public bool is_scanning { get; set; }
        public DateTime created_date { get; set; }
        public DateTime update_date { get; set; }
        public string USER { get; set; }
        public string tela_picking_id { get; set; }
        public string vendroll_picking { get; set; }
        public string REFERENCE { get; set; }
        public string? TelaPickingDefectoId { get; set; }
        public string? location { get; set; }
        public string? descriptionDefecto { get; set; }
        public string? nameProveedor { get; set; } 
        public int rowNum { get; set; }
        public int totalRecords { get; set; }
    }
}
