using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UbiacacionRollos
{
    public class DIARIO_MOVIMIENTO_POST
    {
        public PostJournalData COMPANY { get; set; }
    }

    public class PostJournalData
    {
        public string CODE { get; set; }
        public string USER { get; set; }
        public string JOURNALID { get; set; }
    }
}
