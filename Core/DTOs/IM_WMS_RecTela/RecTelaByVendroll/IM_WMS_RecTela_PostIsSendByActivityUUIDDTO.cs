using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela.RecTelaByVendroll
{
    public class IM_WMS_RecTela_PostIsSendByActivityUUIDDTO
    {
        public string TelaPickingByVendrollId { get; set; }
        public string VendRoll { get; set; }
        public string ProveedorId { get; set; }
        public string Location { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Guid ActivityUUID { get; set; }
        public int TelaPickingTypeId { get; set; }
        public bool IsSend { get; set; }
    }
}
