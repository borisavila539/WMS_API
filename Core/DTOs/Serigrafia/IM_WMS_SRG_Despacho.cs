using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia
{
    public class IM_WMS_SRG_Despacho
    {
        public int Id { get; set; }
        public string Driver { get; set; }
        public string Truck { get; set; }
        public int StatusId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Store { get; set; }

        public List<TrasladoDespachoDTO> Traslados { get; set; }

    }
}
