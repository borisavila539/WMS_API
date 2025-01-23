using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Devoluciones
{
    public class DefectosAuditoria
    {
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public selectList[] Operacion { get; set; }
        public selectList[] Defecto { get; set; }

    }

    public class selectList
    {
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }


}
