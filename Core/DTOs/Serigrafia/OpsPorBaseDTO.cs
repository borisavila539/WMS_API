using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia
{
    public class OpPorBaseDTO
    {
        public string ProdMasterId { get; set; }
        public string ItemIdEstilo { get; set; }
        public string INVENTCOLORID { get; set; }
        public int EstadoOp {  get; set; }
        public List<TallaOpDTO> Tallas { get; set; }
    }
}
