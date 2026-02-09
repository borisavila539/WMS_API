using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia
{
    public class ArticulosDisponiblesTraslado
    {
        public string ItemId { get; set; }
        public string Color { get; set; }
        public string LoteId { get; set; }
        public int CantidadDisponible { get; set; }
        public string LocationId { get; set; }
        public string ProductType { get; set; }
    }
}
