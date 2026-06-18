using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.RecepcionYUbicacionAX
{
    public class TrasladoAxDto
    {
        public string TransferId { get; set; }
        public DateTime Fecha { get; set; }
        public string InventLocationIdFrom { get; set; }
        public string InventLocationIdTo { get; set; }
        public string ItemId { get; set; }
        public string ImDatosTecnicos2 { get; set; }
        public decimal MontoTraslado { get; set; }
        public int StatusId { get; set; }
    }
}
