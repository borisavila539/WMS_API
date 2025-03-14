using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela
{
    public class IM_WMS_RecTela_PostTelaPickingMergeDTO
    {
        public string ItemId { get; set; }
        public string InventColorId { get; set; }
        public string? NameColor { get; set; }
        public string InventBatchId { get; set; }
        public decimal Qty { get; set; }
        public int tela_picking_id { get; set; }
        public string JournalId { get; set; }
        public string InventSerialId { get; set; }
        public string VendRoll { get; set; }
        public string? User { get; set; }
        public DateTime created_date { get; set; }
        public bool is_scanning { get; set; }
        public DateTime update_date { get; set; }
        public string vendroll_picking { get; set; }
        public string? Reference { get; set; }
        public int TelaPickingDefectoId { get; set; }
        public string? DescriptionDefecto { get; set; }
        public string? Location { get; set; }
    }

}
