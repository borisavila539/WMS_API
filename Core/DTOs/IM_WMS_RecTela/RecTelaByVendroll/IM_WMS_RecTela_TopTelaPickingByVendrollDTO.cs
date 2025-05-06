using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela.RecTelaByVendroll
{
    public class IM_WMS_RecTela_TopTelaPickingByVendrollDTO
    {
        public long ProveedorId { get; set; }
        public string ActivityUUID { get; set; } = string.Empty;
        public int CantidadEscaneados { get; set; }
        public string NameProveedor { get; set; } = string.Empty;
    }
}
