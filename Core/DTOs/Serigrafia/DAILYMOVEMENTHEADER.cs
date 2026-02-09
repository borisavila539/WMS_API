using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.DTOs.Serigrafia
{
    public class DAILYMOVEMENTHEADER
    {
        [XmlElement("HEADER", typeof(DAILYMOVEMENPARAMETRS))]
        public DAILYMOVEMENPARAMETRS PARAM { get; set; }
    }

    public class DAILYMOVEMENPARAMETRS
    {
        [XmlElement]
        public string PERSONNELNUMBER { get; set; }

        [XmlElement]
        public string JOURNALNAME { get; set; }  

        [XmlElement]
        public string DESCRIPTION { get; set; }    
    }
}
