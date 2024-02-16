using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.DTOs
{
    public class MOVIMIENTOHEADER
    {
        [XmlElement("MOVIMIENTOLINE", typeof(MOVIMIENTOLINE))]
        public MOVIMIENTOLINE[] LINES { get; set; }
    }

    public class MOVIMIENTOLINE
    {
        [XmlElement]
        public string JOURNALID { get; set; }
        [XmlElement]
        public string ITEMBARCODE { get; set; }
        [XmlElement]
        public string PROCESO { get; set; }
        [XmlElement]
        public string IMBOXCODE { get; set; }
    }
}
