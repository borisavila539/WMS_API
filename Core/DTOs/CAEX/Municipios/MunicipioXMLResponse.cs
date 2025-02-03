using System.Xml.Serialization;

namespace Core.DTOs.CAEX.Municipios
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]

    public class MunicipioXMLResponse
    {
        [XmlElement(ElementName = "Body")]
        public BodyResponseMunicipio Body { get; set; }
    }

    public class BodyResponseMunicipio
    {
        [XmlElement(ElementName = "ObtenerListadoMunicipiosResponse", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ObtenerListadoMunicipiosResponse ObtenerListadoMunicipiosResponse { get; set; }
    }

    public class ObtenerListadoMunicipiosResponse
    {
        [XmlElement(ElementName = "ResultadoObtenerMunicipios", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ResultadoObtenerMunicipios ResultadoObtenerMunicipios { get; set; }
    }

    public class ResultadoObtenerMunicipios
    {
        [XmlElement(ElementName = "ResultadoOperacion")]
        public ResultadoOperacion ResultadoOperacion { get; set; }

        [XmlArray(ElementName = "ListadoMunicipios")]
        [XmlArrayItem(ElementName = "Municipio")]
        public Municipio[] Municipios { get; set; }
    }

    public class Municipio
    {
        [XmlElement(ElementName = "Codigo")]
        public string Codigo { get; set; }

        [XmlElement(ElementName = "Nombre")]
        public string Nombre { get; set; }

        [XmlElement(ElementName = "CodigoDepto")]
        public string CodigoDepto { get; set; }

        [XmlElement(ElementName = "CodigoCabecera")]
        public string CodigoCabecera { get; set; }
    }
}
