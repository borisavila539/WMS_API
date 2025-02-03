using System.Xml.Serialization;

namespace Core.DTOs.CAEX
{
    public class Autenticacion
    {        
        [XmlElement(ElementName = "Login")]
        public string Login { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }        
    }
}
