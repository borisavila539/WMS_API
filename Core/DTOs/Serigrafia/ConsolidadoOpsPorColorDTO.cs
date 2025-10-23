using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia
{
    public class ConsolidadoOpsPorColorDTO
    {
        public string INVENTCOLORID { get; set; }
        public List<string> OpsIds { get; set; }
        public List<TallaOpDTO> Tallas { get; set; }
    }
}
