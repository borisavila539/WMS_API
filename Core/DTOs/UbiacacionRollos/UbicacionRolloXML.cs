
using System.Xml.Serialization;

namespace Core.DTOs.UbiacacionRollos
{

 
    [XmlRoot("INTEGRATION")]
    public class DIARIO_MOVIMIENTO_ROLLO_HEADER
    {
        [XmlElement("COMPANY")]
        public MovimientoRolloHeaderData COMPANY { get; set; }
    }

    public class MovimientoRolloHeaderData
    {
        [XmlElement("CODE")]
        public string CODE { get; set; } = "IMHN";

        [XmlElement("USER")]
        public string USER { get; set; } = "servicio_ax";

        [XmlElement("JOURNALDESCRIPTION")]
        public string JOURNALDESCRIPTION { get; set; }
    }

    // --- ESTRUCTURA PARA LAS LÍNEAS DE MOVIMIENTO ---
    [XmlRoot("INTEGRATION")]
    public class DIARIO_MOVIMIENTO_ROLLO_LINE
    {
        [XmlElement("COMPANY")]
        public MovimientoRolloLineData COMPANY { get; set; }
    }

    public class MovimientoRolloLineData
    {
        [XmlElement("CODE")]
        public string CODE { get; set; } = "IMHN";

        [XmlElement("USER")]
        public string USER { get; set; } = "servicio_ax";

        [XmlElement("JOURNALID")]
        public string JOURNALID { get; set; }

        [XmlElement("BARCODE")]
        public string BARCODE { get; set; }

        [XmlElement("QTY")]
        public string QTY { get; set; }

        // Dimensiones Origen
        [XmlElement("FROMINVENTSITEID")]
        public string FROMINVENTSITEID { get; set; }

        [XmlElement("FROMINVENTLOCATIONID")]
        public string FROMINVENTLOCATIONID { get; set; }

        [XmlElement("FROMWMSLOCATIONID")]
        public string FROMWMSLOCATIONID { get; set; }

        // Dimensiones Destino
        [XmlElement("TOINVENTSITEID")]
        public string TOINVENTSITEID { get; set; }

        [XmlElement("TOINVENTLOCATIONID")]
        public string TOINVENTLOCATIONID { get; set; }

        [XmlElement("TOWMSLOCATIONID")]
        public string TOWMSLOCATIONID { get; set; }

        // Campos adicionales vacíos para cumplir con la firma del método 2 de AX si los requiere
        [XmlElement("FROMINVENTBATCHID")]
        public string FROMINVENTBATCHID { get; set; } = "";

        [XmlElement("TOINVENTBATCHID")]
        public string TOINVENTBATCHID { get; set; } = "";
    }
}
