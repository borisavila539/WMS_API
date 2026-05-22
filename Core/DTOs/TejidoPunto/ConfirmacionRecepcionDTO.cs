using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.TejidoPunto
{
    public class ConfirmacionRecepcionDTO
    {
        public string Action { get; set; }
        public string ProdmasterId { get; set; }

        public string PurchId { get; set; }

        public string PackingSlipId { get; set; }

        public decimal QtyReceive { get; set; }
    }
}
