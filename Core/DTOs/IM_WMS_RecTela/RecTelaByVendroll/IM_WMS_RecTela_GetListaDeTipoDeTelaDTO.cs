using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela.RecTelaByVendroll
{
    public class IM_WMS_RecTela_GetListaDeTipoDeTelaDTO
    {
        public int TelaPickingTypeId { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string? Description { get; set; } = null;
        public bool IsActive { get; set; }
    }
}
