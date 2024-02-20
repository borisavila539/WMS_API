using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class EtiquetaDTO
    {
        public string IM_CENTRO_DE_COSTOS { get; set; }
        public int Numero_caja { get; set; }
        public string JOURNALNAMEID { get; set; }
        public string ITEMID { get; set; }
        public string INVENTSIZEID { get; set; }
        public string INVENTCOLORID { get; set; }
        public int QTY { get; set; }
        public string Solicitante { get; set; }
        public string Empacador { get; set; }

    }
}
