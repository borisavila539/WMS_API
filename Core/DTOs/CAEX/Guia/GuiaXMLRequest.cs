using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.DTOs.CAEX.Guia
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]

    public class GuiaXMLRequest
    {
        [XmlElement(ElementName = "Body")]
        public BodyRequestGenerarGuia Body { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespaces => new XmlSerializerNamespaces(new[]
        {
        new System.Xml.XmlQualifiedName("soap", "http://schemas.xmlsoap.org/soap/envelope/")
        });
    }
    public class BodyRequestGenerarGuia
    {
        [XmlElement(ElementName = "GenerarGuia", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public GenerarGuia GenerarGuia { get; set; }
    }
    public class GenerarGuia
    {
        [XmlElement(ElementName = "Autenticacion", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public Autenticacion Autenticacion { get; set; }

        [XmlElement(ElementName = "ListaRecolecciones", Namespace = "http://www.caexlogistics.com/ServiceBus")]
        public ListaRecolecciones ListaRecolecciones { get; set; }
    }

    public class ListaRecolecciones
    {
        [XmlElement(ElementName = "DatosRecoleccion")]
        public List<DatosRecoleccion> DatosRecoleccion { get; set; }
    }

    public class DatosRecoleccion
    {
        [XmlElement(ElementName = "RecoleccionID")]
        public string RecoleccionID { get; set; }

        [XmlElement(ElementName = "RemitenteNombre")]
        public string RemitenteNombre { get; set; }

        [XmlElement(ElementName = "RemitenteDireccion")]
        public string RemitenteDireccion { get; set; }

        [XmlElement(ElementName = "RemitenteTelefono")]
        public string RemitenteTelefono { get; set; }

        [XmlElement(ElementName = "DestinatarioNombre")]
        public string DestinatarioNombre { get; set; }

        [XmlElement(ElementName = "DestinatarioDireccion")]
        public string DestinatarioDireccion { get; set; }

        [XmlElement(ElementName = "DestinatarioTelefono")]
        public string DestinatarioTelefono { get; set; }

        [XmlElement(ElementName = "DestinatarioContacto")]
        public string DestinatarioContacto { get; set; }

        [XmlElement(ElementName = "DestinatarioNIT")]
        public string DestinatarioNIT { get; set; }

        [XmlElement(ElementName = "ReferenciaCliente1")]
        public string ReferenciaCliente1 { get; set; }

        [XmlElement(ElementName = "ReferenciaCliente2")]
        public string ReferenciaCliente2 { get; set; }

        [XmlElement(ElementName = "CodigoPobladoDestino")]
        public string CodigoPobladoDestino { get; set; }

        [XmlElement(ElementName = "CodigoPobladoOrigen")]
        public string CodigoPobladoOrigen { get; set; }

        [XmlElement(ElementName = "TipoServicio")]
        public string TipoServicio { get; set; }

        [XmlElement(ElementName = "MontoCOD")]
        public decimal MontoCOD { get; set; }

        [XmlElement(ElementName = "FormatoImpresion")]
        public string FormatoImpresion { get; set; }

        [XmlElement(ElementName = "CodigoCredito")]
        public string CodigoCredito { get; set; }

        [XmlElement(ElementName = "MontoAsegurado")]
        public decimal MontoAsegurado { get; set; }

        [XmlElement(ElementName = "Observaciones")]
        public string Observaciones { get; set; }

        [XmlElement(ElementName = "CodigoReferencia")]
        public int CodigoReferencia { get; set; }

        [XmlElement(ElementName = "FechaRecoleccion")]
        public string FechaRecoleccion { get; set; }

        [XmlElement(ElementName = "TipoEntrega")]
        public int TipoEntrega { get; set; }

        [XmlArray(ElementName = "Piezas")]
        [XmlArrayItem(ElementName = "Pieza")]
        public List<PiezaGuia> Piezas { get; set; }
    }
    public class PiezaGuia
    {
        [XmlElement(ElementName = "NumeroPieza")]
        public int NumeroPieza { get; set; }

        [XmlElement(ElementName = "TipoPieza")]
        public string TipoPieza { get; set; }

        [XmlElement(ElementName = "PesoPieza")]
        public string PesoPieza { get; set; }

        [XmlElement(ElementName = "MontoCOD")]
        public string MontoCOD { get; set; }

    }
}
