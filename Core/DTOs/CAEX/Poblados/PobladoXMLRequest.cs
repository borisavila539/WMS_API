using System.Xml.Serialization;

namespace Core.DTOs.CAEX.Poblados
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]

    public class PobladoXMLRequest
    {
        [XmlElement(ElementName = "Body")]
        public BodyRequestPoblado Body { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespaces => new XmlSerializerNamespaces(new[]
        {
    new System.Xml.XmlQualifiedName("soap", "http://schemas.xmlsoap.org/soap/envelope/")
    });
    }
    public class BodyRequestPoblado
    {
        [XmlElement(ElementName = "ObtenerListadoPoblados", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ObtenerListadoPoblados ObtenerListadoPoblados { get; set; }
    }

    public class ObtenerListadoPoblados
    {
        [XmlElement(ElementName = "Autenticacion", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public Autenticacion Autenticacion { get; set; }
    }
}
