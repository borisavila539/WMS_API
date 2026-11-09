using Core.DTOs.AuditoriaInterna;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IIM_WMS_AuditoriaInternaRepository
    {
        // App
        Task<AuditoriaInternaAbrirCajaResponse> AbrirCaja(AbrirCajaAuditoriaRequest request);
        Task<AuditoriaInternaEscanearResponse> EscanearPieza(EscanearPiezaAuditoriaRequest request);
        Task<AuditoriaInternaRespuestaDto> CerrarCaja(CerrarCajaAuditoriaRequest request);
        Task<AuditoriaInternaAbrirCajaResponse> ObtenerCaja(int auditoriaId);
        Task<List<AuditoriaInternaPendienteDto>> ObtenerPendientes();
        Task<List<AuditoriaInternaFaltanteDto>> ObtenerFaltantes(int auditoriaId);
        Task<List<AuditoriaInternaHistorialEscaneoDto>> ObtenerEscaneos(int auditoriaId);

        // Reapertura y reinicio (web, usuario con permiso)
        Task<AuditoriaInternaPermisoDto> PermisoReabrir(string usuario);
        Task<AuditoriaInternaRespuestaDto> ReabrirCaja(ReabrirCajaAuditoriaRequest request);
        Task<AuditoriaInternaRespuestaDto> ReiniciarCaja(ReabrirCajaAuditoriaRequest request);
        Task<AuditoriaInternaRespuestaDto> EliminarCaja(ReabrirCajaAuditoriaRequest request);
        Task<List<AuditoriaInternaHistorialDto>> ObtenerHistorial(int auditoriaId);

        // Dashboard
        Task<AuditoriaInternaDashboardResumenDto> DashboardResumen(DateTime fechaIni, DateTime fechaFin);
        Task<List<AuditoriaInternaDashboardContratistaDto>> DashboardPorContratista(DateTime fechaIni, DateTime fechaFin);
        Task<List<AuditoriaInternaDashboardDiaDto>> DashboardPorDia(DateTime fechaIni, DateTime fechaFin);
        Task<List<AuditoriaInternaDashboardCajaDto>> DashboardCajas(DateTime fechaIni, DateTime fechaFin, string porFecha);
        Task<List<AuditoriaInternaDashboardPendienteDto>> DashboardPendientes(DateTime fechaIni, DateTime fechaFin);
    }
}
