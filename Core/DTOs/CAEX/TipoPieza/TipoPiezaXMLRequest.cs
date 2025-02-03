using System.Xml.Serialization;

namespace Core.DTOs.CAEX.TipoPieza
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]

    public class TipoPiezaXMLRequest
    {
        [XmlElement(ElementName = "Body")]
        public BodyRequestPieza Body { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespaces => new XmlSerializerNamespaces(new[]
        {
    new System.Xml.XmlQualifiedName("soap", "http://schemas.xmlsoap.org/soap/envelope/")
    });
    }

    public class BodyRequestPieza
    {
        [XmlElement(ElementName = "ObtenerTiposPiezas", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ObtenerListadoPiezas ObtenerTiposPiezas { get; set; }
    }

    public class ObtenerListadoPiezas
    {
        [XmlElement(ElementName = "Autenticacion", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public Autenticacion Autenticacion { get; set; }
    }
}
