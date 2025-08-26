using Core.DTOs.IM_PrepEnvOp;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/PrepEnvOp")]
    [ApiController]
    public class IM_PrepEnvOpController : Controller
    {
        private readonly IIM_PrepEnvOpRepository _IIM_PrepEnvOpRepository;
        public IM_PrepEnvOpController(IIM_PrepEnvOpRepository IM_PrepEnvOpRepository)
        {
            _IIM_PrepEnvOpRepository = IM_PrepEnvOpRepository;
        }


        [HttpGet("GetListadoDeOp")]
        public async Task<ActionResult<ListadoDeOpResponseDTO>>
            GetListadoDeOp(
            [FromQuery] DateTime fechaInicioSemana,
            [FromQuery] DateTime fechaFinSemana,
            [FromQuery] string? area)
        {
            var resp = await _IIM_PrepEnvOpRepository.GetListadoDeOp(fechaInicioSemana, fechaFinSemana, area);
            return Ok(resp);
        }


        [HttpPost("UpdateOpPreparada")]
        public async Task<ActionResult<ListadoDeOpResponseDTO>>UpdateOpPreparada([FromBody] UpdateOpPreparadaRequestDTO requestDTO)
        {
            try
            {
                var resp = await _IIM_PrepEnvOpRepository.UpdateOpPreparada( requestDTO.idOpPreparada, requestDTO.userCode);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode( 500, $"Error al actualizar la OP preparada: {ex.Message}");
            }
        }

        [HttpPost("UpdateOpPreparadaEmpaquetada")]
        public async Task<ActionResult<IM_PrepEnvOp_UpdateOpPreparadaLikeEmpaquetadaDTO>> UpdateOpPreparadaEmpaquetada([FromBody] UpdateOpPreparadaEmpaquetadaRequestDTO data)
        {
            try
            {
                var resp = await _IIM_PrepEnvOpRepository.UpdateOpPreparadaEmpaquetada(data);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la OP preparada: {ex.Message}");
            }
        }


        [HttpGet("ListaOpPorEnviar")]
        public async Task<ActionResult<ListadoDeOpResponseDTO>> ListaOpPorEnviar([FromQuery] DateTime fechaInicioSemana, [FromQuery] DateTime fechaFinSemana)
        {
            try
            {
                var resp = await _IIM_PrepEnvOpRepository.ListaOpPorEnviar(fechaInicioSemana, fechaFinSemana);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener la lista por enviar: {ex.Message}");
            }
        }


        [HttpPost("PostDetalleOpEnviada")]
        public async Task<ActionResult<IM_PrepEnvOp_PostDetalleOpEnviadaDTO>> PostDetalleOpEnviada(PostDetalleOpEnviadaResponseDTO response)
        {
            try
            {
                var resp = await _IIM_PrepEnvOpRepository.PostDetalleOpEnviada(response);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar las OP: {ex.Message}");
            }
        }

        [HttpPost("PostPrintEtiquetasMateriales")]
        public async Task<ActionResult<string>> PostPrintEtiquetasMateriales([FromBody] List<ArticuloDTO> data, [FromQuery] string? ipImpresora)
        {
            try
            {
                var resp = await _IIM_PrepEnvOpRepository.PostPrintEtiquetasMateriales(data, ipImpresora);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al imprimr materiales: {ex.Message}");
            }
        }

        [HttpPost("PostPrintEtiquetasEnvio")]
        public async Task<ActionResult<string>> PostPrintEtiquetasEnvio([FromBody] List<IM_PrepEnvOp_ListaOpPorEnviarDTO> data, [FromQuery] string? ipImpresora)
        {
            try
            {
                var resp = await _IIM_PrepEnvOpRepository.PostPrintEtiquetasEnvio(data, ipImpresora);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al imprimir los traslados enviados: {ex.Message}");
            }
        }
    }
}
