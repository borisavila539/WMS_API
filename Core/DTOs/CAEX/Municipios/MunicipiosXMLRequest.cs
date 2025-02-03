using System.Xml.Serialization;

namespace Core.DTOs.CAEX.Municipios
{

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class MunicipiosXMLRequest
    {
        [XmlElement(ElementName = "Body")]
        public BodyRequestMunicipio Body { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespaces => new XmlSerializerNamespaces(new[]
        {
    new System.Xml.XmlQualifiedName("soap", "http://schemas.xmlsoap.org/soap/envelope/")
    });
    }
    public class BodyRequestMunicipio
    {
        [XmlElement(ElementName = "ObtenerListadoMunicipios", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ObtenerListadoMunicipios ObtenerListadoMunicipios { get; set; }
    }

    public class ObtenerListadoMunicipios
    {
        [XmlElement(ElementName = "Autenticacion", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public Autenticacion Autenticacion { get; set; }
    }
    
}
