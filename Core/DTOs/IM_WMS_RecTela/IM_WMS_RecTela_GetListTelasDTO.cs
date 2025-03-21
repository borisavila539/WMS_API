using System.Collections.Generic;

namespace Core.DTOs.IM_WMS_RecTela
{
    public class IM_WMS_RecTela_GetListTelasDTO
    {
        public string JournalId { get; set; }
        public string Description { get; set; }
        public int NumOfLines { get; set; }
        public string JournalNameId { get; set; }
        public int NumOfLinesComplete { get; set; }

        public List<IM_WMS_RecTela_TelaJournalScanCountsDTO> JournalScanCounts { get; set; }
    }
}
