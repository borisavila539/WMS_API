using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.DTOs.Cajasrecicladas
{
    public class CAJASRECICLADAS
    {
        [XmlElement("CAJARECICLADALINE")]
        public CAJARECICLADALINE[] LINES { get; set; }
    }

    public class CAJARECICLADALINE
    {
        [XmlElement]
        public string DIARIO { get; set; }
        [XmlElement]
        public string CENTROCOSTOS { get; set; }
        [XmlElement]
        public string QTY { get; set; }
    }


}
