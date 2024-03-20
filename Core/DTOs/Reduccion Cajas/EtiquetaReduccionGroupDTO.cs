using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Reduccion_Cajas
{
    public class EtiquetaReduccionGroupDTO
    {
        public string key { get; set; }
        public List<EtiquetaReduccionDTO> items { get; set; }
    }
}
