using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Devoluciones
{
    public class IM_WMS_UpdateDetalleDefectoDevolucion
    {
        public int Id { get; set; }
        public int IdDevolucionDetalle { get; set; }
        public int IdDefecto { get; set; }
        public string Tipo { get; set; }
        public int Reparacion { get; set; }
        public int IdOperacion { get; set; }
    }
}
