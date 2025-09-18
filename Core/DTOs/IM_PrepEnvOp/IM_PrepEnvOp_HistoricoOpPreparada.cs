using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_PrepEnvOp
{
    public class IM_PrepEnvOp_ListadoDeOpConDetalleDTO : IM_PrepEnvOp_ListadoDeOpDTO
    {
        // Campos adicionales del detalle
        public string NombreRecibidaPor { get; set; }
        public DateTime FechaDeEntrega { get; set; }

        public byte[] FirmaBase64 { get; set; }

        public string EntregadoPor { get; set; }
    }


    public class ArticuloHistoricoDTO
    {
        public int IdOpPreparada { get; set; }
        public string CodigoArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public string Color { get; set; }
        public decimal CantidadTransferida { get; set; }
        public string Area { get; set; }
        public string Empacado { get; set; }
        public string Enviado { get; set; }
        public bool IsComplete { get; set; }
        public bool IsEmpaquetada { get; set; }
        public string Estilo { get; set; }
        public string Lote { get; set; }
        public string Base { get; set; }
        public int Semana { get; set; }
        public int Year { get; set; }

        public int? IdDetalleOpEnviada { get; set; }
    }

    public class HistoricoOpAgrupadoDTO
    {
        public string OrdenTrabajo { get; set; }
        public string NoTraslado { get; set; }

        // campos generales de la OP/Traslado
        public string NombreRecibidaPor { get; set; }
        public DateTime FechaDeEntrega { get; set; }
        public string EntregadoPor { get; set; }
        public int IdDetalleOpEnviada { get; set; }

        public List<ArticuloHistoricoDTO> Articulos { get; set; } = new();
    }

    public class IM_PrepEnvOp_FirmaBase64
    {
        public byte[] FirmaBase64 { get; set; }

    }

}
