using System.Xml.Serialization;

namespace Core.DTOs.CAEX
{
    public class ResultadoOperacion
    {
        [XmlElement(ElementName = "ResultadoExitoso")]
        public bool ResultadoExitoso { get; set; }

        [XmlElement(ElementName = "MensajeError")]
        public string MensajeError { get; set; }

        [XmlElement(ElementName = "CodigoRespuesta")]
        public int CodigoRespuesta { get; set; }
    }
}
