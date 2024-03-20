using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Reduccion_Cajas
{
    public class EtiquetaReduccionDTO
    {
        public string NameAlias { get; set; }

        public string ITEMID { get; set; }
        public string INVENTCOLORID { get; set; }
        public string INVENTSIZEID { get; set; }
        public int QTY { get; set; }
    }
}
