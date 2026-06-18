using Core.DTOs.RecepcionYUbicacionAX;
using Core.DTOs.Serigrafia;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WMSRecepcionYUbicacionAXController : ControllerBase
    {
        private readonly IWMSRecepcionYUbicacionAX _repository;
        private IAX _ax;

        public WMSRecepcionYUbicacionAXController(IWMSRecepcionYUbicacionAX repository, IAX AX)
        {
            _repository = repository;
            _ax = AX;
        }
        [HttpGet("GetTrasladosAX")]
        public async Task<ActionResult<IEnumerable<TrasladoAxDto>>> GetTrasladosAX()
        {
            var resp = await _repository.GetTrasladosAXAsync();
            if (resp == null || resp.Count == 0)
            {
                return BadRequest("No se encontraron registros");
            }
            return Ok(resp);
        }

        [HttpGet("GetReporteInformacionAXTraslado/{idTraslado}")]
        public async Task<ActionResult<IEnumerable<ReporteInformacionAXTrasladoDto>>> GetReporteInformacionAXTraslado(string idTraslado)
        {
            var resp = await _repository.GetReporteInformacionAXTrasladoAsync(idTraslado);

            if (resp == null || resp.Count == 0)
            {
                return BadRequest("No se encontraron registros");
            }

            return Ok(resp);
        }

        [HttpPost("RecibirTrasladoConCambioUbiacion/{trasladoId}/{empresa}")]
        public async Task<ActionResult<string>> RecibirTrasladoConCambioUbiacion(string trasladoId, string empresa)
        {
            var Informacion = await _repository.GetUbicacionEmpresa(empresa);
            var resp = await _ax.RecibirTrasladoYCambioUbiacion(trasladoId, Informacion);
            if (resp == null)
            {
                return BadRequest("No se pudo procesar la solicitud");
            }
            return Ok(resp);

        }
    }

}
