using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class IM_WMS_TrasladosAbiertos
    {
        public string TRANSFERIDFROM { get; set; }
        public string TRANSFERIDTO { get; set; }
        public string INVENTLOCATIONIDTO { get; set; }
        public string DESCRIPTION { get; set; }
    }
}
