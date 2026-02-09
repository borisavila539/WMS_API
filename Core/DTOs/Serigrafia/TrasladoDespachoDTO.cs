using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia
{
    public class TrasladoDespachoDTO
    {
        public int DespachoId { get; set; }
        public string TransferId { get; set; }
        public string InventLocationIdFrom { get; set; }
        public string InventLocationIdTo {  get; set; }
        public string ItemId { get; set; }
        public int MontoTraslado { get; set; }
        public int StatusId { get; set; }
    }
}
