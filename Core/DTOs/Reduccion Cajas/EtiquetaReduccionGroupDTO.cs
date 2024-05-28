using System.Collections.Generic;

namespace Core.DTOs.Reduccion_Cajas
{
    public class EtiquetaReduccionGroupDTO
    {
        public string key { get; set; }
        public List<EtiquetaReduccionDTO> items { get; set; }
    }
}
