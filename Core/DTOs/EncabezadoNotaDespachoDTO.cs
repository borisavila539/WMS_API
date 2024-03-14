using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class EncabezadoNotaDespachoDTO
    {
        public DateTime fecha { get; set; }
        public string Motorista { get; set; }
        public string TRANSFERIDFROM { get; set; }
        public string TRANSFERIDTO { get; set; }
        public string Destino { get; set; }
    }
}
