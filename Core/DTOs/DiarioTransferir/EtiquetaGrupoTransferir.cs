using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DiarioTransferir
{
    public class EtiquetaGrupoTransferir
    {
        public string key { get; set; }
        public List<EtiquetaTransferir> items { get; set; }
    }
}
