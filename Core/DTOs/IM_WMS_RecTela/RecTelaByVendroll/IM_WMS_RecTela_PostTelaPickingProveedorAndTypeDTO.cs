using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela.RecTelaByVendroll
{
    public class IM_WMS_RecTela_PostTelaPickingProveedorAndTypeDTO
    {
        public string ProveedorId { get; set; }
        public int TelaPickingTypeId { get; set; }
    }


    public class ProveedorByTypeDTO
    {
        public int TelaPickingTypeId { get; set; }
        public string ProveedorId { get; set; }
    }

}
