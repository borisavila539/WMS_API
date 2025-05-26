using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_PrepEnvOp
{
    public class IM_PrepEnvOp_PostDetalleOpEnviadaDTO
    {
        public int IdDetalleOpEnviada { get; set; }
        public string NombreRecibidaPor { get; set; }
        public DateTime FechaDeEntrega { get; set; }
        public string FirmaBase64 { get; set; }
        public DateTime FechaDeCreacion { get; set; }
        public string CreadoPor { get; set; }
    }

    public class PostDetalleOpEnviadaResponseDTO
    {
        public string NombreRecibidaPor { get; set; }
        public DateTime FechaDeEntrega { get; set; }
        public byte[] FirmaBase64 { get; set; }
        public string CreadoPor { get; set; }
        public List<IM_PrepEnvOp_ListaOpPorEnviarDTO>  ListaOpPorEnviar { get; set; }
    }

}
