using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UbiacacionRollos
{
    public class DiarioMovimientoPendienteDto
    {
        public string JournalId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NumOfLines { get; set; }
        public bool IsPosted { get; set; }
    }
}
