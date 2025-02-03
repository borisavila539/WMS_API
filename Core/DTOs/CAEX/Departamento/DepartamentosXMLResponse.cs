using System.Xml.Serialization;

namespace Core.DTOs.CAEX.Departamento
{

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class DepartamentosXMLResponse
    {
        [XmlElement(ElementName = "Body")]
        public BodyResponseDepartamento Body { get; set; }
    }

    public class BodyResponseDepartamento
    {
        [XmlElement(ElementName = "ObtenerListadoDepartamentosResponse", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ObtenerListadoDepartamentosResponse ObtenerListadoDepartamentosResponse { get; set; }
    }

    public class ObtenerListadoDepartamentosResponse
    {
        [XmlElement(ElementName = "ResultadoObtenerDepartamentos", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ResultadoObtenerDepartamentos ResultadoObtenerDepartamentos { get; set; }
    }

    public class ResultadoObtenerDepartamentos
    {
        [XmlElement(ElementName = "ResultadoOperacion")]
        public ResultadoOperacion ResultadoOperacion { get; set; }

        [XmlArray(ElementName = "ListadoDepartamentos")]
        [XmlArrayItem(ElementName = "Departamento")]
        public Departamento[] Departamentos { get; set; }
    }   

    public class Departamento
    {
        [XmlElement(ElementName = "Codigo")]
        public string Codigo { get; set; }

        [XmlElement(ElementName = "Nombre")]
        public string Nombre { get; set; }
    }
}
