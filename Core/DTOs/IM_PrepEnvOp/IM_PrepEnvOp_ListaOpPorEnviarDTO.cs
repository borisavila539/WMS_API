using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_PrepEnvOp
{
    public class IM_PrepEnvOp_ListaOpPorEnviarDTO
    {
        public string Estilo { get; set; }
        public string OrdenTrabajo { get; set; }
        public string Estado { get; set; }
        public string Area { get; set; }
        public string NoTraslado { get; set; }
        public string Semana { get; set; }
        public string Year { get; set; }
        public int Total { get; set; }
        public int Completados { get; set; }
        public int NoCompletados { get; set; }
        public int Enviados { get; set; }
        public int MetalicosCount { get; set; }
        public int EmpaqueCount { get; set; }
        public int SumCantidadTransferida { get; set; }
        public int NoEnviados { get; set; }
    }

}
