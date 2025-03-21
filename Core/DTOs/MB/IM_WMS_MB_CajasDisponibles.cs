﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MB
{
    public class IM_WMS_MB_CajasDisponibles
    {
        public int ID { get; set; }
        public int IDConsolidado { get; set; }
        public string Lote { get; set; }
        public string Orden { get; set; }
        public string Articulo { get; set; }
        public string DescripcioMB { get; set; }
        public int NumeroCaja { get; set; }
        public string Talla { get; set; }
        public int Cantidad { get; set; }
        public string Color { get; set; }
        public string NombreColor { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadCajas { get; set; }
        public Boolean PickToDespacho { get; set; }
    }

    public class IM_WMS_MB_CajasDisponibles2
    {
        public int ID { get; set; }
        public int IDConsolidado { get; set; }
        public string Lote { get; set; }
        public string Orden { get; set; }
        public string Articulo { get; set; }
        public string DescripcioMB { get; set; }
        public int NumeroCaja { get; set; }
        public string Talla { get; set; }
        public int Cantidad { get; set; }
        public string Color { get; set; }
        public string NombreColor { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadCajas { get; set; }
        public Boolean PickToDespacho { get; set; }
        public List<IM_WMS_MB_CajasDisponibles2> subRows { get; set; }
    }
}
