using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DiarioTransferir
{
    public class IM_WMS_InsertTransferirCajaDetalle
    {
        public string BoxCode { get; set; }
        public string JournalID { get; set; }
        public string ItembarCode { get; set; }
        public int QTY { get; set; }
        public string Exito { get; set; }
    }
}
