using Core.DTOs.RecepcionFacturasCD;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IM_WMS_RecepcionFacturasCDController : ControllerBase
    {
        private readonly IIM_WMS_RecepcionFacturasCDRepository _repository;

        public IM_WMS_RecepcionFacturasCDController(IIM_WMS_RecepcionFacturasCDRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("GetFacturasRecepcionCD")]
        public async Task<IActionResult> GetFacturasRecepcionCD([FromQuery] DateTime fechaIni, [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaIni.Date > fechaFin.Date)
                    return BadRequest(new { Success = false, Message = "La fecha inicial no puede ser mayor que la fecha final." });

                var data = await _repository.GetFacturasRecepcionCD(fechaIni, fechaFin);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("GetCuentasExcluidas")]
        public async Task<IActionResult> GetCuentasExcluidas()
        {
            try
            {
                var data = await _repository.GetCuentasExcluidas();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("InsertCuentaExcluida")]
        public async Task<IActionResult> InsertCuentaExcluida([FromBody] CuentaExcluidaRecepcionCDRequest item)
        {
            try
            {
                if (item == null || string.IsNullOrWhiteSpace(item.CuentaCliente))
                    return BadRequest(new { Success = false, Message = "La cuenta de cliente es obligatoria." });

                var response = await _repository.InsertarCuentaExcluida(item);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("InsertCuentasExcluidas")]
        public async Task<IActionResult> InsertCuentasExcluidas([FromBody] CuentasExcluidasRecepcionCDRequest request)
        {
            try
            {
                if (request == null || request.Cuentas == null || !request.Cuentas.Any(c => !string.IsNullOrWhiteSpace(c)))
                    return BadRequest(new { Success = false, Message = "Seleccione al menos una cuenta de cliente." });

                var response = await _repository.InsertarCuentasExcluidas(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("DeleteCuentaExcluida/{id}")]
        public async Task<IActionResult> DeleteCuentaExcluida(int id, [FromQuery] string usuario = null)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { Success = false, Message = "El Id del registro es obligatorio." });

                var response = await _repository.EliminarCuentaExcluida(id, usuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
