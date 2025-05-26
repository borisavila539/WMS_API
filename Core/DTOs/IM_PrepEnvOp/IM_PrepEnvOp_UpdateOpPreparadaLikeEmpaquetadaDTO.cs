using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_PrepEnvOp
{
    public class IM_PrepEnvOp_UpdateOpPreparadaLikeEmpaquetadaDTO
    {
        public int IdOpPreparada { get; set; }
        public string OrdenTrabajo { get; set; } = string.Empty;
        public string CodigoArticulo { get; set; } = string.Empty;
        public string? NombreArticulo { get; set; }
        public string Color { get; set; } = string.Empty;
        public string EstadoOp { get; set; } = string.Empty;
        public string Articulo { get; set; } = string.Empty;
        public string Lote { get; set; } = string.Empty;
        public string Estilo { get; set; } = string.Empty;
        public string Base { get; set; } = string.Empty;
        public string NoTraslado { get; set; } = string.Empty;
        public decimal CantidadTransferida { get; set; }
        public string NoPropuesta { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public int Transferid { get; set; }
        public DateTime FechaTraslado { get; set; }
        public int Semana { get; set; }
        public int Year { get; set; }
        public string Empacado { get; set; } = string.Empty;
        public int Datoempac { get; set; }
        public DateTime? FechaEmpacado { get; set; }
        public string? EmpacadoPor { get; set; }
        public string Enviado { get; set; } = string.Empty;
        public int DatoEnv { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string? EnviadoPor { get; set; }
        public string DesdeAlmacen { get; set; } = string.Empty;
        public string Almacen { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public bool IsEmpaquetada { get; set; }
        public DateTime FechaActualizado { get; set; }
        public string? ActualizadoPor { get; set; }
        public int? IdDetalleOpEnviada { get; set; }
    }

    public class UpdateOpPreparadaEmpaquetadaRequestDTO
    {
        public string userCode { get; set; }
        public List<ArticuloDTO> materiales { get; set; }
    }

    public class ArticuloDTO
    {
        public string codigoArticulo { get; set; }
        public string nombreArticulo { get; set; }
        public string color { get; set; }
        public string area { get; set; }
        public List<OrdenesDTO> ordenes { get; set; }
    }

    public class OrdenesDTO
    {
        public string ordenTrabajo { get; set; }
        public int idOpPreparada { get; set; }
        public bool isComplete { get; set; }
        public DateTime fechaTraslado { get; set; }
        public int cantidadTransferida { get; set; }
        public string semana { get; set; }
        public string year { get; set; }
        public string estilo { get; set; }
    }

}
