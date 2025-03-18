using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela
{
    public class IM_WMS_RecTela_CorreosActivosDTO
    {
        public int RecTela_CorreosId { get; set; }
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public bool IsActive { get; set; }
    }
}
