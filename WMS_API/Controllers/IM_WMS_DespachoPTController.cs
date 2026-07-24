using Core.DTOs.Despacho_PT;
using Core.Interfaces;
using DevExpress.Utils.Serializing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WMS_API.Features.Repositories;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IM_WMS_DespachoPTController : ControllerBase
    {
        private readonly IIM_WMS_DespachoPTRepository _Repository;
        public IM_WMS_DespachoPTController(IIM_WMS_DespachoPTRepository iM_WMS_DespachoPTRepository)
        {
            _Repository = iM_WMS_DespachoPTRepository;
        }
        [HttpPost("importar-excel")]
        public async Task<IActionResult> ImportarDespachoExcel(IFormFile file, [FromQuery] int userId)
        {
            var resultado = await _Repository.ProcesarYGuardarDespachoExcel(file, userId);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }
        [HttpGet("ObtenerPorId/{despachoID}")]
        public async Task<IActionResult> ObtenerDespachoPorId(int despachoID)
        {
            var resultado = await _Repository.ObtenerDespachoPorId(despachoID);
            if (resultado == null || resultado.Cabecera == null)
            {
                return NotFound(new { success = false, message = $"No se encontró el despacho #{despachoID}" });
            }
            return Ok(resultado);
        }

    }
}
