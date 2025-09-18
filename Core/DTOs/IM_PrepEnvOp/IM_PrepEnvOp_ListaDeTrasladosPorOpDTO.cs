using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_PrepEnvOp
{
    public class IM_PrepEnvOp_ListaDeTrasladosPorOpDTO
    {
        public string OrdenTrabajo { get; set; }
        public string CodigoArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public bool IsComplete { get; set; }
        public string NoTraslado { get; set; }
        public string Area { get; set; }
    }

    public class ListaDeTrasladosPorOpReponseDTO
    {
        public List<string> listaOp { get; set; }
        public List<string> listaArea { get; set; }

    }
}
