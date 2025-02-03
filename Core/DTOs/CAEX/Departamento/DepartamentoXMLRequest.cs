using System.Xml.Serialization;

namespace Core.DTOs.CAEX.Departamento
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class DepartamentoXMLRequest
    {
        [XmlElement(ElementName = "Body")]
        public BodyRequestDepartamento Body { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespaces => new XmlSerializerNamespaces(new[]
        {
        new System.Xml.XmlQualifiedName("soap", "http://schemas.xmlsoap.org/soap/envelope/")
        });
    }
    public class BodyRequestDepartamento
    {
        [XmlElement(ElementName = "ObtenerListadoDepartamentos", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ObtenerListadoDepartamentos ObtenerListadoDepartamentos { get; set; }
    }

    public class ObtenerListadoDepartamentos
    {
        [XmlElement(ElementName = "Autenticacion", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public Autenticacion Autenticacion { get; set; }
    }

    
}
