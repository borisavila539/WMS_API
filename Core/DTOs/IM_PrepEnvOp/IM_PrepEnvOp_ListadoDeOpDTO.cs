using System;
using System.Collections.Generic;

namespace Core.DTOs.IM_PrepEnvOp
{
    public class IM_PrepEnvOp_ListadoDeOpDTO
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
        public DateTime? FechaActualizado { get; set; }
        public string? ActualizadoPor { get; set; }
        public int? IdDetalleOpEnviada { get; set; }
    }
    public class ListadoDeOpResponseDTO
    {
        public List<string> ListaDeEstilos { get; set; } = new();
        public List<EstiloAgrupadoDTO> Agrupados { get; set; } = new();
    }
    public class MaterialDTO : IM_PrepEnvOp_ListadoDeOpDTO
    {
        // Hereda todas las propiedades del listado original
    }

    public class OrdenDTO
    {
        public string OrdenTrabajo { get; set; } = string.Empty;
        public decimal sumCantidadTransferida { get; set; }
        public List<MaterialDTO> Materiales { get; set; } = new();
    }

    public class EstiloAgrupadoDTO
    {
        public string Estilo { get; set; } = string.Empty;
        public List<OrdenDTO> Ordenes { get; set; } = new();
    }
}
