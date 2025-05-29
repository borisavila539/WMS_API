using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_PrepEnvOp
{
    public class IM_PrepEnvOp_CorreosHabilitadosDTO
    {
        public int IdCorreoPrepEnvOP { get; set; }
        public string Correo { get; set; }
        public bool IsActive { get; set; }
        public string Area { get; set; }
    }

}
