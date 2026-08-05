using Core.DTOs.DiseñoEtiqueta;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WMS_DiseñoEtiquetaController : ControllerBase
    {
        private readonly IDiseñoEtiquetaRepository _diseñoEtiquetaRepository;
        public WMS_DiseñoEtiquetaController(IDiseñoEtiquetaRepository diseñoEtiquetaRepository)
        {
            _diseñoEtiquetaRepository = diseñoEtiquetaRepository;
        }

        [HttpPost("ImprimirPrueba")]
        public async Task<IActionResult> ImprimirPrueba([FromBody] SolicitudImpresionDto request)
        {
            var response = await _diseñoEtiquetaRepository.ImprimirPrueba(request);

            if (!response.Exito)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("GuardarDiseñoUbicacion")]
        public async Task<IActionResult> GuardarDiseño([FromBody] List<ElementoEtiquetaDto> elementos)
        {
            var resultado = await _diseñoEtiquetaRepository.GuardarDiseño(elementos);

            if (!resultado.Exito)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        [HttpGet("ObtenerDiseñoUbicacion")]
        public async Task<IActionResult> ObtenerDiseño([FromQuery] string codigoDiseño = "UBICACION_RACK")
        {
            var resultado = await _diseñoEtiquetaRepository.ObtenerDiseño(codigoDiseño);

            if (!resultado.Exito)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
    }
}
