using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.DTOs.TejidoPunto
{
    [XmlRoot("INTEGRATION")]
    public class ConfirmacionRecepcionXML
    {
        [XmlElement("Action")]
        public string Action { get; set; }

        [XmlElement("PurchId")]
        public string PurchId { get; set; }

        [XmlElement("PackingSlipId")]
        public string PackingSlipId { get; set; }

        [XmlElement("QtyReceive")]
        public decimal QtyReceive { get; set; }
    }
}
