﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MB
{
    public class IM_WMS_MB_PACKING
    {
        public int ID { get; set; }
        public string Lote { get; set; }
        public string Orden { get; set; }
        public string Articulo { get; set; }
        public int NumeroCaja { get; set; }
        public string Talla { get; set; }
        public int Cantidad { get; set; }
        public string Color { get; set; }
        public string NombreColor { get; set; }
        public int IDConsolidado { get; set; }
        public Boolean Packing { get; set; }
        public string Pallet { get; set; }
    }
}
