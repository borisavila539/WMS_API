using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela
{
    public class IM_WMS_RecTela_GetTelaPickingRuleDTO
    {
        public int TelaPickingRuleId { get; set; }
        public string StartWith { get; set; }
        public int MaxCount { get; set; }
        public bool IsActive { get; set; }

    }
}
