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
        public int Total { get; set; }
        public int Completados { get; set; }
        public int NoCompletados { get; set; }
        public int Enviados { get; set; }
        public int NoEnviados { get; set; }
    }

}
