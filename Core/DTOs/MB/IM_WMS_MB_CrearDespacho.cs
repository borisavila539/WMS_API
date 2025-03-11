using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MB
{
    public class IM_WMS_MB_CrearDespacho
    {
        public int ID { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public Boolean Enviado { get; set; }
    }
}
