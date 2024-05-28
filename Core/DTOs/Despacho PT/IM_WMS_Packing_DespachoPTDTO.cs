using System;

namespace Core.DTOs.Despacho_PT
{
    public class IM_WMS_Packing_DespachoPTDTO
    {
        public string PRODID { get; set; } 
        public int BOX { get; set; } 
        public bool Packing { get; set; }
        public string UserPacking { get; set; }
        public DateTime FechaPacking { get; set; }
        public int DespachoID { get; set; }
    }
}
