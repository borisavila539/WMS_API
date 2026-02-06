using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.DTOs.Serigrafia
{

    public class DAILYMOVEMENTLINESHEADER
    {
        [XmlElement("JOURNALID")]
        public string JOURNALID { get; set; }
        [XmlElement("LINES")]
        public DAILYMOVEMENTLINES LINES { get; set; }
    }

    public class DAILYMOVEMENTLINES
    {
        [XmlElement("LINE")]
        public DAILYMOVEMENTLINEPARAMS[] DAILYMOVEMENTLINEPARAMS { get; set; }
    }
    public class DAILYMOVEMENTLINEPARAMS
    {
        [XmlElement("ITEMID")]
        public string ITEMID { get; set; }

        [XmlElement("SITE")]
        public string SITE { get; set; }

        [XmlElement("WAREHOUSE")]
        public string WAREHOUSE { get; set; }

        // Opcionales (según tu X++)
        [XmlElement("COLOR")]
        public string COLOR { get; set; }

        [XmlElement("BATCH")]
        public string BATCH { get; set; }

        [XmlElement("WMSLOCATION")]
        public string WMSLOCATION { get; set; }

        // Tu X++ hace str2Date(text, 123). Para evitar líos de formato,
        // mándalo como string ya formateado "yyyy-MM-dd" o lo que estés usando.
        [XmlElement("TRANSDATE")]
        public string TRANSDATE { get; set; }

        [XmlElement("SIZE")]
        public string SIZE { get; set; }

        [XmlElement("QTY")]
        public string QTY { get; set; }



    }

}
