using Core.DTOs;
using Core.DTOs.ClaseRespuesta;
using Core.DTOs.UbiacacionRollos;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class WMS_UbicacionRollosController : ControllerBase
    {
        private readonly IWMS_UbiacionRollosRepository _ubiacionRollosRepository;
        private readonly IAX _AX;

        public WMS_UbicacionRollosController(IWMS_UbiacionRollosRepository ubiacionRollosRepository, IAX AX)
        {
            _ubiacionRollosRepository = ubiacionRollosRepository;
            _AX = AX;
        }

        [HttpGet("ExistenciaUbicacion/{ubicacion}/{almacen}")]
        public async Task<ActionResult<RespuestaExistenciaUbicacion>> GetExistenciaUbiacion(string ubicacion, string almacen)
        {
            var resp = await _ubiacionRollosRepository.GetExistenciaUbiacion(ubicacion, almacen);
            return Ok(resp);
        }

        [HttpPost("AgregarUbicacionRollos/{Empresa}/{Ubicacion}/{Almacen}/{Pasillo}")]
        public async Task<ActionResult<Respuesta<string>>> AgregarNuevaUbicacion(string Empresa, string Ubicacion, string Almacen, string Pasillo)
        {
            var resp = await _AX.AgregarNuevaUbicacion(Empresa, Ubicacion, Almacen, Pasillo);
            return Ok(resp);
        }
        [HttpPost("CambiarUbiacionRollos")]
        public async Task<ActionResult<Respuesta<string>>> CambiarUbiacionRollos([FromBody] List<MovimientoRolloDto> rollosAMover)
        {
            var resp = await _AX.RegistrarMovimientoRollosEnDiario(rollosAMover);
            return Ok(resp);
        }
        [HttpGet("ConsultarRolloCambioUbicacion/{rolloId}")]
        public async Task<ActionResult<RespuestaConsultarRollo>> ConsultarRolloCambioUbiacion(string rolloId)
        {
            var resp = await _ubiacionRollosRepository.GetRolloParaCambioDeUbiacion(rolloId);
            return Ok(resp);
        }

        [HttpPost("validar-rollo-repetido")]
        public async Task<IActionResult> ValidarRolloRepetido([FromBody] ValidarRolloRepetidoRequestDto request)
        {
            try
            {
                List<ValidarRolloRepetidoDto> diariosDuplicados =
                    await _ubiacionRollosRepository.ValidarRolloRepetidoEnInventario(request);

                if (diariosDuplicados.Any())
                {
                    return Ok(new
                    {
                        esDuplicado = true,
                        mensaje = "El rollo ya está registrado en uno o más diarios abiertos de transferencia.",
                        diarios = diariosDuplicados
                    });
                }

                return Ok(new
                {
                    esDuplicado = false,
                    mensaje = "El rollo está disponible para ser procesado.",
                    diarios = new List<ValidarRolloRepetidoDto>()
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error interno al validar el rollo en el inventario.", detalle = ex.Message });
            }
        }
        [HttpPost("RegistrarCambioUbicacionRollos")]
        public async Task<ActionResult<Respuesta<string>>> RegistrarCambioUbicacionRollos([FromBody] List<MovimientoRolloDto> rollosAMover)
        {
            var resp = await _AX.RegistrarMovimientoRollosEnDiario(rollosAMover);
            return Ok(resp);
        }

        [HttpGet("GetConsultarRollosPorUbicacion/{almacen}/{ubicacion}")]
        public async Task<ActionResult<RespuestaExistenciaUbicacion>> GetConsultarRollosPorUbicacion(string almacen, string ubicacion)
        {
            var resp = await _ubiacionRollosRepository.GetConsultarRollosPorUbicacion(almacen, ubicacion);
            return Ok(resp);
        }

        [HttpGet("ConsultarInventarioRollosPorAlmacen/{almacen}")]
        public async Task<ActionResult<List<InventarioRolloPorAlmacenDto>>> ConsultarInventarioRollosPorAlmacen(string almacen)
        {
            var resp = await _ubiacionRollosRepository.ConsultarInventarioRollosPorAlmacen(almacen);
            return Ok(resp);
        }

        [HttpGet("GetDiariosMovimientoPendientes")]
        public async Task<ActionResult<List<DiarioMovimientoPendienteDto>>> GetDiariosMovimientoPendientes()
        {
            var resp = await _ubiacionRollosRepository.ObtenerDiariosMovimientoPendientes();
            return Ok(resp);
        }

        [HttpGet("GetDetalleDiarioMovimiento/{journalId}")]
        public async Task<ActionResult<List<DetalleDiarioMovimientoDto>>> GetDetalleDiarioMovimiento(string journalId)
        {
            var resp = await _ubiacionRollosRepository.ObtenerDetalleDiarioMovimiento(journalId);
            return Ok(resp);
        }
    }
}
