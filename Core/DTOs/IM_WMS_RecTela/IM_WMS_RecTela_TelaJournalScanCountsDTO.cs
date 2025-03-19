using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela
{
    public class IM_WMS_RecTela_TelaJournalScanCountsDTO
    {
        public string JOURNALID { get; set; }
        public string GroupByColumn { get; set; }
        public int ScannedCount { get; set; }
        public int NotScannedCount { get; set; }
        public int TotalCount { get; set; }
    }
}
