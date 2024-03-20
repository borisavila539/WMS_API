
using System.Xml.Serialization;

namespace Core.DTOs
{
    public class REDUCCIONHEADER
    {
        [XmlElement("REDUCCIONLINE", typeof(REDUCCIONLINE))]
        public REDUCCIONLINE[] LINES { get; set; }
    }
    public class REDUCCIONLINE
    {
       
        [XmlElement]
        public string ITEMBARCODE { get; set; }
        [XmlElement]
        public string PROCESO { get; set; }
        [XmlElement]
        public string IMBOXCODE { get; set; }
    }
}
