using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_PrepEnvOp
{
    public class IM_PrepEnvOp_UpdateOpPreByEstiloAndOpDTO : IM_PrepEnvOp_ListadoDeOpDTO
    {
    }

    public class UpdateOpPreByEstiloAndOpRequestDTO
    {
        public string Estilo { get; set; }
        public string OrdenTrabajo { get; set; }
        public int? IdDetalleOpEnviada { get; set; }
    }
}
