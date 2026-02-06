using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia
{
    public class PackingRequestDTO
    {
        public int DespachoId { get; set; }
        public string ProdMasterId { get; set; }
        public int Box { get; set; }
        public string UserPacking { get; set; }
    }
}
