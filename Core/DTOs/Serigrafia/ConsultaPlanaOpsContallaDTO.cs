using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia
{
    public class ConsultaPlanaOpsContallaDTO
    {
        public string ProdMasterId { get; set; }
        public string ItemIdEstilo { get; set; }
        public string INVENTCOLORID { get; set; }
        public int EstadoOp {  get; set; }
        public string Talla {  get; set; }
        public int CantidadSolicitada { get; set; }
        public int CantidadPreparada { get; set; }
    }
}
