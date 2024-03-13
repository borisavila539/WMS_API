using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class EstadoTrasladoTipoDTO
    {
        public string Tipo { get; set; }
        public int picking { get; set; }
        public int Enviado { get; set; }
        public int Recibido { get; set; }
        public int Total { get; set; }
    }
}
