﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class IM_WMS_Despacho_Tela_Detalle_Rollo
    {
        public string INVENTSERIALID { get; set; }
        public bool Picking { get; set; }
        public bool Packing { get; set; }
        public bool Receive { get; set; }


    }
}
