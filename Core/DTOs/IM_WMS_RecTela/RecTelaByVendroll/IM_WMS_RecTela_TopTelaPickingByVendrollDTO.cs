using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela.RecTelaByVendroll
{
    public class IM_WMS_RecTela_TopTelaPickingByVendrollDTO
    {
        public string ProveedorId { get; set; }
        public string ActivityUUID { get; set; } = string.Empty;
        public int CantidadEscaneados { get; set; }
        public string NombreProveedor { get; set; } = string.Empty;
        public DateTime FechaUltimoEscaneo { get; set; }
        public DateTime FechaInicioEscaneo { get; set; }
    }
}
