using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia
{
    public class DiarioLineasDTO
    {
        public string JournalId { get; set; }
        public List<LineasDiarioDTO> Lineas { get; set; } = new List<LineasDiarioDTO>();
    }

    public class LineasDiarioDTO
    {
        public string ItemId { get; set; }
        public string Site { get; set; }
        public string WareHouse { get; set; }
        public string Color { get; set; }
        public string Batch { get; set; }
        public string WMSLocation { get; set; }
        public string TransDate { get; set; }
        public string Size { get; set; }
        public string Qty { get; set; }
    }
}
