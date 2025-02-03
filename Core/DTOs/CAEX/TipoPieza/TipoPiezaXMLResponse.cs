using System.Xml.Serialization;

namespace Core.DTOs.CAEX.TipoPieza
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]

    public class TipoPiezaXMLResponse
    {
        [XmlElement(ElementName = "Body")]
        public BodyResponsePieza Body { get; set; }
    }

    public class BodyResponsePieza
    {
        [XmlElement(ElementName = "ObtenerTiposPiezasResponse", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ObtenerListadoPiezasResponse ObtenerTiposPiezasResponse { get; set; }
    }

    public class ObtenerListadoPiezasResponse
    {
        [XmlElement(ElementName = "ResultadoObtenerPiezas", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ResultadoObtenerPiezas ResultadoObtenerPiezas { get; set; }
    }

    public class ResultadoObtenerPiezas
    {
        [XmlElement(ElementName = "ResultadoOperacion")]
        public ResultadoOperacion ResultadoOperacion { get; set; }

        [XmlArray(ElementName = "ListadoPiezas")]
        [XmlArrayItem(ElementName = "Pieza")]
        public Pieza[] Piezas { get; set; }
    }

    public class Pieza
    {
        [XmlElement(ElementName = "Codigo")]
        public string Codigo { get; set; }

        [XmlElement(ElementName = "Descripcion")]
        public string Descripcion { get; set; }

        
    }
}
