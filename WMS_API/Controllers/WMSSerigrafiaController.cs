using Core.DTOs.Serigrafia;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WMSSerigrafiaController : ControllerBase
    {
        private readonly IWMSSerigrafiaRepository _repository;
        public WMSSerigrafiaController(IWMSSerigrafiaRepository repository) 
        {
            _repository = repository;
        }
        [HttpGet("GetConsolitadoOpsPorBase")]
        public async Task<ActionResult<IEnumerable<MateriaPrimaPorOpDTO>>> GetDiariosAbiertos()
        {
            var resp = await _repository.GetMateriaPrimaPorOpAsync();
            return Ok(resp);
        }

        [HttpGet("GetConsolidadoOpsPorColor/{ItemId}")]
        public async Task<ActionResult<IEnumerable<OpPorBaseDTO>>> GetConsolidadoOpsPorColor(string ItemId)
        {
            var resp = await _repository.GetConsolidadoOpsPorColorAsync(ItemId);
            return Ok(resp);
        }

        [HttpGet("GetOpsPorBase/{ItemId}")]
        public async Task<ActionResult<IEnumerable<OpPorBaseDTO>>> GetOpsPorBaseAsync(string ItemId)
        {
            var resp= await _repository.GetOpsPorBaseAsync(ItemId);
            return Ok(resp);
        }

        [HttpPost("CreaOpsPreparadasAsync/{ItemId}")]
        public async Task<ActionResult<List<OpPorBaseDTO>>> CreaOpsPreparadasAsync(string ItemId, [FromBody] ConsolidadoOpsPorColorDTO consolidadoPreparado)
        {
            var resp = await _repository.CreaOpsPreparadasAsync(ItemId, consolidadoPreparado);
            return Ok(resp);
        }

        [HttpGet("GetOpsPrepardas/{ItemId}")]
        public async Task<ActionResult<IEnumerable<OpPorBaseDTO>>> GetOpsPrepardas(string ItemId)
        {
            var resp = await _repository.GetOpsPrepardasAsync(ItemId);
            return Ok(resp);
        }

    }
}
