using System;

namespace Core.DTOs.AuditoriaInterna
{
    // Columnas de IM_WMS_AuditoriaInterna_DashboardResumen.
    // Cobertura y pendientes son sobre lo RECIBIDO en el CD; enviadas es informativo.
    public class AuditoriaInternaDashboardResumenDto
    {
        public int CajasEnviadas { get; set; }
        public int UnidadesEnviadas { get; set; }
        public int CajasRecibidas { get; set; }
        public int UnidadesRecibidas { get; set; }
        public int CajasSinRecibir { get; set; }
        public int CajasRecibidasAuditadas { get; set; }
        public decimal Cobertura { get; set; }
        public int CajasPendientes { get; set; }
        public int CajasEnProceso { get; set; }
        public int CajasAuditadas { get; set; }
        public int CajasConDiferencia { get; set; }
        public int CajasDiferenciaEtiqueta { get; set; }
        public int UnidadesEsperadas { get; set; }
        public int UnidadesEscaneadas { get; set; }
        public int UnidadesFaltantes { get; set; }
        public int UnidadesSobrantes { get; set; }
        public int UnidadesDiferenciaEtiqueta { get; set; }
        public decimal Exactitud { get; set; }
        public decimal MinutosPromedioCaja { get; set; }
        // Hora del ultimo recalculo del cache de envios y recepciones (las auditorias son en linea)
        public DateTime? UniversoActualizado { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_DashboardPorContratista (una fila por linea:
    // planta propia = Denim PEPE, MB, Serigrafia; contratistas = nombre del contratista).
    public class AuditoriaInternaDashboardContratistaDto
    {
        public string Origen { get; set; }
        public string Linea { get; set; }
        public int CajasEnviadas { get; set; }
        public int UnidadesEnviadas { get; set; }
        public int CajasRecibidas { get; set; }
        public int UnidadesRecibidas { get; set; }
        public int CajasRecibidasAuditadas { get; set; }
        public decimal Cobertura { get; set; }
        public int CajasAuditadas { get; set; }
        public int CajasConDiferencia { get; set; }
        public int CajasDiferenciaEtiqueta { get; set; }
        public int UnidadesEsperadas { get; set; }
        public int UnidadesEscaneadas { get; set; }
        public int UnidadesFaltantes { get; set; }
        public int UnidadesSobrantes { get; set; }
        public int UnidadesDiferenciaEtiqueta { get; set; }
        public decimal Exactitud { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_DashboardPorDia.
    public class AuditoriaInternaDashboardDiaDto
    {
        public DateTime Fecha { get; set; }
        public int CajasAuditadas { get; set; }
        public int CajasConDiferencia { get; set; }
        public int UnidadesEsperadas { get; set; }
        public int UnidadesEscaneadas { get; set; }
        public int UnidadesFaltantes { get; set; }
        public int UnidadesSobrantes { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_DashboardCajas.
    public class AuditoriaInternaDashboardCajaDto
    {
        public int Id { get; set; }
        public string OP { get; set; }
        public int NumeroCaja { get; set; }
        public string Estado { get; set; }
        public string Resultado { get; set; }
        public string ModoValidacion { get; set; }
        public string Origen { get; set; }
        public string Linea { get; set; }
        public string Contratista { get; set; }
        public string Conjunto { get; set; }
        public string CategoriaInterna { get; set; }
        public string Articulo { get; set; }
        public string Estilo { get; set; }
        public string ColorDescripcion { get; set; }
        public string Lote { get; set; }
        public string CategoriaCaja { get; set; }
        public string PLDespacho { get; set; }
        public int CantidadEsperada { get; set; }
        public int CantidadEscaneada { get; set; }
        public int Diferencia { get; set; }
        public int DiferenciaEtiqueta { get; set; }
        public string Observaciones { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public string UsuarioInicio { get; set; }
        public DateTime FechaInicio { get; set; }
        public string UsuarioCierre { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal? Minutos { get; set; }
        public int Rechazos { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_DashboardPendientes.
    public class AuditoriaInternaDashboardPendienteDto
    {
        public string OP { get; set; }
        public int NumeroCaja { get; set; }
        public string Origen { get; set; }
        public string Linea { get; set; }
        public string Contratista { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public int Unidades { get; set; }
        public string Estado { get; set; }
    }
}
