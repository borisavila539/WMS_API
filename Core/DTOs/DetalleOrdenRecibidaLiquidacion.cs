using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class DetalleOrdenRecibidaLiquidacion
    {
        public int Numero { get; set; }
        public string OpPadre { get; set; }
        public string OpHija { get; set; }
        public string Size { get; set; }
        public decimal CantidadEstimada { get; set; }
        public decimal CantidadLiberada { get; set; }
        public int HayDiferencia { get; set; }
    }
}
