using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.InventarioCiclicoTela
{
    public class IM_WMS_InventarioCilicoTelaDiario
    {
        public string JournalID { get; set; }
        public string ItemID { get; set; }
        public string InventLocationID { get; set; }
        public string InventSerialID { get; set; }
        public string ApvendRoll { get; set; }
        public string InventColorID { get; set; }
        public string ColorName { get; set; }
        public string InventBatchID { get; set; }
        public decimal InventOnHand { get; set; }
        public string WMSLocationID { get; set; }
        public string Reference { get; set; }
        public string ConfigID { get; set; }
        public bool Exist { get; set; }
        public bool New { get; set; }
        public string CreatedBy { get; set; }
        public string ScanBy { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ScanDateTime { get; set; }

    }
}
