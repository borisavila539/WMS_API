using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela
{
    public class IM_WMS_RecTela_DatosRollosProveedorDTO
    {
        public string JournalId { get; set; }
        public string ItemId { get; set; }
        public string InventBatchId { get; set; }
        public int CantidadDeRollos { get; set; }
        public string NombreProveedor { get; set; }
    }
}
