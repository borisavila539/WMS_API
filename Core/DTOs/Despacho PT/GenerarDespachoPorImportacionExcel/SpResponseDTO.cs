using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT.GenerarDespachoPorImportacionExcel
{
    public class SpResponseDTO
    {
        public int ID { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
