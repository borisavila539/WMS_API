using System.Xml.Serialization;

namespace Core.DTOs.InventarioCiclicoTela
{
    public class INVENTARIOCICLICOTELAHEADER
    {
       [XmlElement("INVENTARIOCICLICOTELALINE", typeof(INVENTARIOCICLICOTELALINE))]
       public INVENTARIOCICLICOTELALINE[] LINES { get; set; }

    }
    public class INVENTARIOCICLICOTELALINE
    {

        [XmlElement]
        public string JOURNALID { get; set; }

        [XmlElement]
        public string INVENTSERIALID { get; set; }
        [XmlElement]
        public string WMSLOCATIONID { get; set; }
        [XmlElement]
        public string INVENTLOCATIONID { get; set; }
        [XmlElement]
        public string QTY { get; set; }

        [XmlElement]
        public string PROCESO { get; set; }
    }
}
