using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class IM_WMS_EstadoTrasladosDTO
    {
        public string TRANSFERID { get; set; }
        public string Estado { get; set; }
        public int QTY  { get; set; }
        public int Enviado { get; set; }
        public int Recibido { get; set; }

    }
}
