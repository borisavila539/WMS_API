using System;

namespace Core.DTOs.Despacho_PT
{
    public class IM_WMS_Get_Despachos_PT_DTO
    {
        public int id { get; set; }
        public string  Driver { get; set; }
        public string truck { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
