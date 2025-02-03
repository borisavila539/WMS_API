using System.Xml.Serialization;

namespace Core.DTOs.CAEX.Poblados
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class PobladoXMLResponse
    {
        [XmlElement(ElementName = "Body")]
        public BodyResponsePoblado Body { get; set; }
    }
    public class BodyResponsePoblado
    {
        [XmlElement(ElementName = "ObtenerListadoPobladosResponse", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ObtenerListadoPobladosResponse ObtenerListadoPobladosResponse { get; set; }
    }

    public class ObtenerListadoPobladosResponse
    {
        [XmlElement(ElementName = "ResultadoObtenerPoblados", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ResultadoObtenerPoblados ResultadoObtenerPoblados { get; set; }
    }

    public class ResultadoObtenerPoblados
    {
        [XmlElement(ElementName = "ResultadoOperacion")]
        public ResultadoOperacion ResultadoOperacion { get; set; }

        [XmlArray(ElementName = "ListadoPoblados")]
        [XmlArrayItem(ElementName = "Poblado")]
        public Poblado[] Poblados { get; set; }
    }

    public class Poblado
    {
        [XmlElement(ElementName = "Codigo")]
        public string Codigo { get; set; }

        [XmlElement(ElementName = "Nombre")]
        public string Nombre { get; set; }

        [XmlElement(ElementName = "CodigoDepto")]
        public string CodigoDepto { get; set; }

        [XmlElement(ElementName = "CodigoMunicipio")]
        public string CodigoMunicipio { get; set; }

        [XmlElement(ElementName = "ZonaRoja")]
        public int ZonaRoja { get; set; }
    }
}
