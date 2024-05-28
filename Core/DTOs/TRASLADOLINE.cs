using System.Xml.Serialization;


namespace Core.DTOs
{
    public class TRASLADOHEADER
    {
        [XmlElement("TRASLADOLINE", typeof(TRASLADOLINE))]
        public TRASLADOLINE[] LINES { get; set; }
    }
    public class TRASLADOLINE
    {
        [XmlElement]
        public string TRANSFERID { get; set; }
        [XmlElement]
        public string ESTADO { get; set; }

    }
}
