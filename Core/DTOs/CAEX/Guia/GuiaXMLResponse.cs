using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.DTOs.CAEX.Guia
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class GuiaXMLResponse
    {
        [XmlElement(ElementName = "Body")]
        public BodyResponse Body { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespaces => new XmlSerializerNamespaces(new[]
        {
        new System.Xml.XmlQualifiedName("soap", "http://schemas.xmlsoap.org/soap/envelope/")
    });
    }

    public class BodyResponse
    {
        [XmlElement(ElementName = "GenerarGuiaResponse", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public GenerarGuiaResponse GenerarGuiaResponse { get; set; }
    }

    public class GenerarGuiaResponse
    {
        [XmlElement(ElementName = "ResultadoGenerarGuia", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ResultadoGenerarGuia ResultadoGenerarGuia { get; set; }
    }

    public class ResultadoGenerarGuia
    {
        [XmlElement(ElementName = "ResultadoOperacionMultiple")]
        public ResultadoOperacionMultiple ResultadoOperacionMultiple { get; set; }

        [XmlElement(ElementName = "ListaRecolecciones")]
        public ListaRecoleccionesResponse ListaRecolecciones { get; set; }
    }

    public class ResultadoOperacionMultiple
    {
        [XmlElement(ElementName = "ResultadoExitoso")]
        public bool ResultadoExitoso { get; set; }

        [XmlElement(ElementName = "MensajeError")]
        public string MensajeError { get; set; }

        [XmlElement(ElementName = "CodigoRespuesta")]
        public int CodigoRespuesta { get; set; }
    }

    public class ListaRecoleccionesResponse
    {
        [XmlElement(ElementName = "DatosRecoleccion")]
        public List<DatosRecoleccionResponse> DatosRecoleccion { get; set; }
    }

    public class DatosRecoleccionResponse
    {
        [XmlElement(ElementName = "RecoleccionID")]
        public string RecoleccionID { get; set; }

        [XmlElement(ElementName = "NumeroPieza")]
        public int NumeroPieza { get; set; }

        [XmlElement(ElementName = "NumeroGuia")]
        public string NumeroGuia { get; set; }

        [XmlElement(ElementName = "MontoTarifa")]
        public decimal MontoTarifa { get; set; }

        [XmlElement(ElementName = "URLConsulta")]
        public string URLConsulta { get; set; }

        [XmlElement(ElementName = "URLRecoleccion")]
        public string URLRecoleccion { get; set; }

        [XmlElement(ElementName = "ResultadoOperacion", IsNullable = true)]
        public ResultadoOperacionMultiple ResultadoOperacion { get; set; }

        [XmlElement(ElementName = "DatosGuia", IsNullable = true)]
        public string DatosGuia { get; set; }
    }
}
