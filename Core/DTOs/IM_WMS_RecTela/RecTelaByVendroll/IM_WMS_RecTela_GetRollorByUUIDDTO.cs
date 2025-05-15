using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela.RecTelaByVendroll
{
    public class IM_WMS_RecTela_GetRolloByUUIDDTO
    {
        public int TelaPickingByVendrollId { get; set; }
        public string VendRoll { get; set; } = string.Empty;
        public string ProveedorId { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string ActivityUUID { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string NameProveedor { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int TelaPickingTypeId { get; set; }
    }
}
