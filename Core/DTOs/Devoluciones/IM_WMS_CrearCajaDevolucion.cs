using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Devoluciones
{
    public class IM_WMS_CrearCajaDevolucion
    {
        public int Id { get; set; }
        public int IDDevolucion { get; set; }
        public int Caja { get; set; }
        public string Tipo { get; set; }
        public bool Packing { get; set; }
        public string UserPacking { get; set; }
        public bool Recibir { get; set; }
        public string UserRecibir { get; set; }
    }
}
