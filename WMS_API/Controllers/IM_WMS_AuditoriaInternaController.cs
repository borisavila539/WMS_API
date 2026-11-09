using Core.DTOs.AuditoriaInterna;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IM_WMS_AuditoriaInternaController : ControllerBase
    {
        private readonly IIM_WMS_AuditoriaInternaRepository _repository;

        public IM_WMS_AuditoriaInternaController(IIM_WMS_AuditoriaInternaRepository repository)
        {
            _repository = repository;
        }

        // ---------------------------------------------------------------- App

        [HttpPost("AbrirCaja")]
        public async Task<IActionResult> AbrirCaja([FromBody] AbrirCajaAuditoriaRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.QRCaja))
                    return BadRequest(new { Success = false, Message = "Escanee el QR de la caja." });
                if (string.IsNullOrWhiteSpace(request.Usuario))
                    return BadRequest(new { Success = false, Message = "El usuario es obligatorio." });

                var response = await _repository.AbrirCaja(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("EscanearPieza")]
        public async Task<IActionResult> EscanearPieza([FromBody] EscanearPiezaAuditoriaRequest request)
        {
            try
            {
                if (request == null || request.AuditoriaId <= 0)
                    return BadRequest(new { Success = false, Message = "La auditoria es obligatoria." });
                if (string.IsNullOrWhiteSpace(request.Codigo))
                    return BadRequest(new { Success = false, Message = "Escanee el QR de la pieza." });
                if (string.IsNullOrWhiteSpace(request.Usuario))
                    return BadRequest(new { Success = false, Message = "El usuario es obligatorio." });

                var response = await _repository.EscanearPieza(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("CerrarCaja")]
        public async Task<IActionResult> CerrarCaja([FromBody] CerrarCajaAuditoriaRequest request)
        {
            try
            {
                if (request == null || request.AuditoriaId <= 0)
                    return BadRequest(new { Success = false, Message = "La auditoria es obligatoria." });
                if (string.IsNullOrWhiteSpace(request.Usuario))
                    return BadRequest(new { Success = false, Message = "El usuario es obligatorio." });
                if (request.Observaciones?.Length > 500)
                    return BadRequest(new { Success = false, Message = "Las observaciones admiten maximo 500 caracteres." });

                var response = await _repository.CerrarCaja(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("Caja/{auditoriaId}")]
        public async Task<IActionResult> ObtenerCaja(int auditoriaId)
        {
            try
            {
                var response = await _repository.ObtenerCaja(auditoriaId);
                if (response.Caja == null)
                    return NotFound(new { Success = false, Message = "La auditoria no existe." });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("Pendientes")]
        public async Task<IActionResult> ObtenerPendientes()
        {
            try
            {
                return Ok(await _repository.ObtenerPendientes());
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("Faltantes/{auditoriaId}")]
        public async Task<IActionResult> ObtenerFaltantes(int auditoriaId)
        {
            try
            {
                return Ok(await _repository.ObtenerFaltantes(auditoriaId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("Escaneos/{auditoriaId}")]
        public async Task<IActionResult> ObtenerEscaneos(int auditoriaId)
        {
            try
            {
                return Ok(await _repository.ObtenerEscaneos(auditoriaId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // ---------------------------------------------------------------- Reapertura (web)

        [HttpGet("PermisoReabrir/{usuario}")]
        public async Task<IActionResult> PermisoReabrir(string usuario)
        {
            try
            {
                return Ok(await _repository.PermisoReabrir(usuario));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("ReabrirCaja")]
        public async Task<IActionResult> ReabrirCaja([FromBody] ReabrirCajaAuditoriaRequest request)
        {
            try
            {
                if (request == null || request.AuditoriaId <= 0)
                    return BadRequest(new { Success = false, Message = "La auditoria es obligatoria." });
                if (string.IsNullOrWhiteSpace(request.Usuario))
                    return BadRequest(new { Success = false, Message = "El usuario es obligatorio." });
                if (string.IsNullOrWhiteSpace(request.Motivo))
                    return BadRequest(new { Success = false, Message = "El motivo de la reapertura es obligatorio." });
                if (request.Motivo.Length > 500)
                    return BadRequest(new { Success = false, Message = "El motivo admite maximo 500 caracteres." });

                // El permiso se valida en el SP; aqui solo se traduce la respuesta
                return Ok(await _repository.ReabrirCaja(request));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // Borra escaneos y rechazos y deja la caja abierta en 0 (mismo permiso que reabrir)
        [HttpPost("ReiniciarCaja")]
        public async Task<IActionResult> ReiniciarCaja([FromBody] ReabrirCajaAuditoriaRequest request)
        {
            try
            {
                if (request == null || request.AuditoriaId <= 0)
                    return BadRequest(new { Success = false, Message = "La auditoria es obligatoria." });
                if (string.IsNullOrWhiteSpace(request.Usuario))
                    return BadRequest(new { Success = false, Message = "El usuario es obligatorio." });
                if (string.IsNullOrWhiteSpace(request.Motivo))
                    return BadRequest(new { Success = false, Message = "El motivo del reinicio es obligatorio." });
                if (request.Motivo.Length > 500)
                    return BadRequest(new { Success = false, Message = "El motivo admite maximo 500 caracteres." });

                // El permiso se valida en el SP
                return Ok(await _repository.ReiniciarCaja(request));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // Borra la auditoria completa (queda en la bitacora de eliminadas); la caja vuelve a quedar pendiente.
        // Mismo permiso que reabrir.
        [HttpPost("EliminarCaja")]
        public async Task<IActionResult> EliminarCaja([FromBody] ReabrirCajaAuditoriaRequest request)
        {
            try
            {
                if (request == null || request.AuditoriaId <= 0)
                    return BadRequest(new { Success = false, Message = "La auditoria es obligatoria." });
                if (string.IsNullOrWhiteSpace(request.Usuario))
                    return BadRequest(new { Success = false, Message = "El usuario es obligatorio." });
                if (string.IsNullOrWhiteSpace(request.Motivo))
                    return BadRequest(new { Success = false, Message = "El motivo de la eliminacion es obligatorio." });
                if (request.Motivo.Length > 500)
                    return BadRequest(new { Success = false, Message = "El motivo admite maximo 500 caracteres." });

                // El permiso se valida en el SP
                return Ok(await _repository.EliminarCaja(request));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("Historial/{auditoriaId}")]
        public async Task<IActionResult> ObtenerHistorial(int auditoriaId)
        {
            try
            {
                return Ok(await _repository.ObtenerHistorial(auditoriaId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // ---------------------------------------------------------------- Dashboard

        [HttpGet("DashboardResumen")]
        public async Task<IActionResult> DashboardResumen([FromQuery] DateTime fechaIni, [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaIni.Date > fechaFin.Date)
                    return BadRequest(new { Success = false, Message = "La fecha inicial no puede ser mayor que la fecha final." });

                return Ok(await _repository.DashboardResumen(fechaIni, fechaFin));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("DashboardPorContratista")]
        public async Task<IActionResult> DashboardPorContratista([FromQuery] DateTime fechaIni, [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaIni.Date > fechaFin.Date)
                    return BadRequest(new { Success = false, Message = "La fecha inicial no puede ser mayor que la fecha final." });

                return Ok(await _repository.DashboardPorContratista(fechaIni, fechaFin));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("DashboardPorDia")]
        public async Task<IActionResult> DashboardPorDia([FromQuery] DateTime fechaIni, [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaIni.Date > fechaFin.Date)
                    return BadRequest(new { Success = false, Message = "La fecha inicial no puede ser mayor que la fecha final." });

                return Ok(await _repository.DashboardPorDia(fechaIni, fechaFin));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("DashboardCajas")]
        public async Task<IActionResult> DashboardCajas([FromQuery] DateTime fechaIni, [FromQuery] DateTime fechaFin, [FromQuery] string porFecha = "RECEPCION")
        {
            try
            {
                if (fechaIni.Date > fechaFin.Date)
                    return BadRequest(new { Success = false, Message = "La fecha inicial no puede ser mayor que la fecha final." });

                // RECEPCION (defecto): cajas recibidas en el periodo; AUDITORIA: auditadas en el periodo
                return Ok(await _repository.DashboardCajas(fechaIni, fechaFin, porFecha?.Trim().ToUpperInvariant()));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("DashboardPendientes")]
        public async Task<IActionResult> DashboardPendientes([FromQuery] DateTime fechaIni, [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaIni.Date > fechaFin.Date)
                    return BadRequest(new { Success = false, Message = "La fecha inicial no puede ser mayor que la fecha final." });

                return Ok(await _repository.DashboardPendientes(fechaIni, fechaFin));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
