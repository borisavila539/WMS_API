using Azure.Core;
using Core.DTOs;
using Core.DTOs.Serigrafia;
using Core.DTOs.Serigrafia.Enums;
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
    public class WMSSerigrafiaController : ControllerBase
    {
        private readonly IWMSSerigrafiaRepository _repository;
        private IAX _ax;

        public WMSSerigrafiaController(IWMSSerigrafiaRepository repository, IAX AX)
        {
            _repository = repository;
            _ax = AX;
        }


        [HttpGet("GetLote")]
        public async Task<ActionResult<IEnumerable<ConsultaLote>>> GetLotes()
        {
            var resp = await _repository.GetLotesAsync();
            if (resp == null || resp.Count == 0)
            {
                return BadRequest("No se encontraron registros");
            }
            return Ok(resp);
        }

        [HttpGet("GetConsultaOpsPorBase/{Lote}")]
        public async Task<ActionResult<IEnumerable<MateriaPrimaPorOpDTO>>> GetConsultaOpsPorBase(string Lote)
        {
            var resp = await _repository.GetMateriaPrimaPorOpAsync(Lote);
            if (resp == null || resp.Count == 0)
            {
                return BadRequest("No se encontraron registros");
            }
            return Ok(resp);
        }


        [HttpGet("GetOpsPorBase/{ItemId}/{Lote}")]
        public async Task<ActionResult<IEnumerable<OpPorBaseDTO>>> GetOpsPorBaseAsync(string ItemId, string Lote)
        {
            var resp = await _repository.GetOpsPorBaseAsync(ItemId, Lote);
            if (resp == null || resp.Count == 0)
            {
                return BadRequest("No se encontraron registros");
            }
            return Ok(resp);
        }

        [HttpPost("CreaOpsPreparadasAsync/{ItemId}/{Lote}")]
        public async Task<ActionResult<List<OpPorBaseDTO>>> CreaOpsPreparadasAsync(string ItemId, string Lote, [FromBody] ConsolidadoOpsPorColorDTO consolidadoPreparado)
        {
            var resp = await _repository.CreaOpsPreparadasAsync(ItemId, Lote, consolidadoPreparado);
            return Ok(resp);
        }

        [HttpGet("GetOpsPrepardas/{ItemId}/{Lote}")]
        public async Task<ActionResult<IEnumerable<OpPorBaseDTO>>> GetOpsPrepardas(string ItemId, string Lote)
        {
            var resp = await _repository.GetOpsPorBaseAsync(ItemId, Lote);
            return Ok(resp);

        }

        [HttpPost("CambiarEstadoOpIniciado/{ItemIdBase}")]
        public async Task<ActionResult> CambiarEstadoOpIniciado([FromBody] OpPorBaseDTO opPorBaseDTO, string ItemIdBase)
        {
            var resp = await _ax.CambioIniciadoEstadoOpSerigrafia(opPorBaseDTO);

            var respuestaGeneraciOP = await _repository.GestionarOPBaseLocal(opPorBaseDTO, ItemIdBase, (int)EstadoOp.Iniciado);
            var esCorrecto = resp.Contains("OK");

            if (!esCorrecto)
            {
                return Ok(resp);

            }

            var seIngresoCorrectamente = respuestaGeneraciOP.Contains("OK");
            if (!seIngresoCorrectamente)
            {
                return Ok($"Se Inicio la OP en AX, pero no se pudo guardar registro local. {respuestaGeneraciOP}");
            }

            return Ok(resp);

        }

        [HttpPost("CambiarEstadOpTerminado/{ItemIdBase}")]
        public async Task<ActionResult> CambiarEstadOpTerminado([FromBody] OpPorBaseDTO opPorBaseDTO, string ItemIdBase)
        {
            var resp = await _ax.CambioTerminadoEstadoOPSerigrafia(opPorBaseDTO);

            var respuestaGeneraciOP = await _repository.GestionarOPBaseLocal(opPorBaseDTO, ItemIdBase, (int)EstadoOp.NotificadoT);
            var esCorrecto = resp.Contains("OK");

            if (!esCorrecto)
            {
                return Ok(resp);

            }

            var seIngresoCorrectamente = respuestaGeneraciOP.Contains("OK");
            if (!seIngresoCorrectamente)
            {
                return Ok($"Se Inicio la OP en AX, pero no se pudo guardar registro local. {respuestaGeneraciOP}");
            }

            return Ok(resp);
        }
        [HttpPost("AjustarCantidadPorOP/{ItemIdBase}")]
        public async Task<ActionResult> AjustarCantidadPorOP([FromBody] OpPorBaseDTO opPorBaseDTO, string ItemIdBase)
        {
            var resp = await _ax.AjustarCantidadPorOP(opPorBaseDTO);

            var esCorrecto = resp.Contains("OK");
            if (!esCorrecto)
            {
                return Ok(resp);

            }

            var respuestaGeneraciOP = await _repository.GestionarOPBaseLocal(opPorBaseDTO, ItemIdBase, (int)(opPorBaseDTO.EstadoOp));

            var seIngresoCorrectamente = respuestaGeneraciOP.Contains("OK");
            if (!seIngresoCorrectamente)
            {

                return Ok($"Se Inicio la OP en AX, pero no se pudo guardar registro local. {respuestaGeneraciOP}");
            }

            return Ok("Exito. Se ajusto la Cantidad");
        }


        [HttpPost("AjustarCantidadPorOPEnNotificar/{ItemIdBase}")]
        public async Task<ActionResult> AjustarCantidadPorOPEnNotificar([FromBody] OpPorBaseDTO opPorBaseDTO, string ItemIdBase)
        {
            var resp = await _ax.AjustarCantidadPorOP(opPorBaseDTO);

            var esCorrecto = resp.Contains("OK");
            if (!esCorrecto)
            {
                return Ok(resp);

            }

            var respuestaAlIniciarAx = await _ax.CambioIniciadoEstadoOpSerigrafia(opPorBaseDTO);
            var seInicioCorrectamente = respuestaAlIniciarAx.Contains("OK");

            if (!seInicioCorrectamente)
            {
                return Ok(resp);

            }

            var respuestaGeneraciOP = await _repository.GestionarOPBaseLocal(opPorBaseDTO, ItemIdBase, (int)(opPorBaseDTO.EstadoOp));

            var seIngresoCorrectamente = respuestaGeneraciOP.Contains("OK");
            if (!seIngresoCorrectamente)
            {

                return Ok($"Se Inicio la OP en AX, pero no se pudo guardar registro local. {respuestaGeneraciOP}");
            }

            return Ok("Exito. Se ajusto la Cantidad");
        }

        [HttpGet("GetArticulosDisponibleParaTraslado/{loteId}")]
        public async Task<ActionResult<IEnumerable<ArticulosDisponiblesTraslado>>> GetArticulosDisponibleParaTraslado(string loteId)
        {
            var resp = await _repository.GetArticulosPisponiblesParaTraslado(loteId);
            if (resp == null || resp.Count == 0)
            {
                return BadRequest("No se encontraron registros");
            }
            return Ok(resp);
        }

        [HttpGet("GetLineasDeTraslado/{ItemId}")]
        public async Task<ActionResult<IEnumerable<LineasTrasladoDTO>>> GetLineasDeTraslado(string ItemId)
        {

            var resp = await _repository.GetLineasDeTraslado(ItemId);


            return Ok(resp);
        }

        [HttpPost("CrearTraslado")]
        public async Task<ActionResult> CrearTraslado([FromBody] List<LineasTrasladoDTO> lineasTraslados)
        {
            var NuevoTraslado = new TrasladoDTO
            {
                Lineas = new List<LineasTrasladoDTO>()
            };

            NuevoTraslado.AlmacenDeSalida = "40";
            NuevoTraslado.AlmacenDeEntrada = "22";


            NuevoTraslado.Lineas = lineasTraslados;
            var resp = await _ax.CrearTrasladosPorArticulo(NuevoTraslado);
            var esCorrectaRespuetaAX = resp.Contains("OK");

            if (!esCorrectaRespuetaAX)
                return BadRequest(resp.ToString());

            var transferId = resp.Split(':', StringSplitOptions.RemoveEmptyEntries);

            var respuestaBaseLocal = await _repository.CrearTrasladoBaseLocal(NuevoTraslado, transferId[1]);
            var respuestaCorrectaBaseLocal = respuestaBaseLocal.Contains("OK");

            if (!respuestaCorrectaBaseLocal)
                return BadRequest(respuestaBaseLocal.ToString());

            var respuesta = "OK";

            return Ok(respuesta);
        }


        [HttpGet("GetArticulosGenericoSegundas/{ItemId}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_SRG_ArticulosGenericosSegundas>>> GetArticulosGenericoSegundas(string ItemId)
        {

            var resp = await _repository.GetArticulosGenericoSegundas(ItemId);


            return Ok(resp.Datos);
        }

        [HttpGet("GetTrasladoParaDespachoPorLote/{LoteId}")]
        public async Task<ActionResult<IEnumerable<TrasladoDespachoDTO>>> GetTrasladoParaDespachoPorLote(string LoteId)
        {
            var resp = await _repository.GetTrasladoPorLote(LoteId);
            if (resp == null)
                return BadRequest(resp.ToString());
            return Ok(resp);
        }

        [HttpPost("CrearDiarioHeader")]
        public string CrearDiarioHeader([FromBody] DiarioHeaderDTO headerDTO)
        {
            const string accion = "HEADER";
            DiarioLineasDTO diarioLineasDTO = new DiarioLineasDTO();
            var resp = _ax.CrearDiario(headerDTO,diarioLineasDTO,accion);
            return resp;
        }

        [HttpPost("CrearDiarioLines")]
        public string CrearDiarioLines([FromBody] DiarioLineasDTO diarioLineasDTO)
        {
            const string accion = "LINES";
            DiarioHeaderDTO diarioHeader = new DiarioHeaderDTO();
            var resp = _ax.CrearDiario(diarioHeader, diarioLineasDTO, accion);
            return resp;
        }

        [HttpGet("GetTipoDiairo")]
        public async Task<ActionResult<IEnumerable<IM_WMS_SRG_TipoDiario>>> GetTipoDiairo()
        {
            var resp = await _repository.GetTiposDiario();
            return resp;
        }


        [HttpGet("GetDiariosAbiertos/{UserId}")]
        public async Task<ActionResult<List<DiariosAbiertosDTO>>> GetDiariosAbiertos(string userId)
        {
            var resp = await _repository.GetDiariosAbiertosAsync(userId, null);
            return resp;
        }

        [HttpGet("GetDiariosAbiertosByDiarioId/{UserId}/{DiarioId}")]
        public async Task<ActionResult<List<DiariosAbiertosDTO>>> GetDiariosAbiertos(string userId, string diarioId)
        {
            var resp = await _repository.GetDiariosAbiertosAsync(userId, diarioId);
            return resp;
        }

        [HttpGet("GetDespachosByBatchId/{batchId}/{tipo}")]
        public async Task<ActionResult<List<IM_WMS_SRG_Despacho>>> GetDespachosByBatchId(string batchId, int tipo)
        {
            var resp = await _repository.GetDespachosByBatchId(batchId,tipo);
            return resp;
        }

        [HttpGet("GetDespachoLinesByIdAEnviar/{DespachoId}")]
        public async Task<ActionResult<List<IM_WMS_SRG_Despacho_Lines_Packing>>> GetDespachoLinesById(string DespachoId)
        {
            var resp = await _repository.GetDespachoLinesByIdAEnviar(DespachoId);
            return resp;
        }


        [HttpGet("GetDespachoLinesByIdARecibir/{DespachoId}")]
        public async Task<ActionResult<List<IM_WMS_SRG_Despacho_Lines_Packing>>> GetDespachoLinesByIdARecibir(string DespachoId)
        {
            var resp = await _repository.GetDespachoLinesByIdARecibir(DespachoId);
            return resp;
        }

        [HttpGet("GetDespachoTrasladosById/{DespachoId}")]
        public async Task<ActionResult<List<TrasladoDespachoDTO>>> GetDespachoTrasladosById(string DespachoId)
        {
            var resp = await _repository.GetDespachoTrasladosById(DespachoId);
            return resp;
        }

        [HttpPost("CreateDespacho")]
        public async Task<ActionResult<string>> CrearDespacho([FromBody] IM_WMS_SRG_Despacho despacho)
        {
            var resp = await _repository.CrearDespacho(despacho);

            return Ok(resp);
        }

        [HttpPost("SetPacking")]
        public async Task<ActionResult<string>> SetPacking([FromBody] PackingRequestDTO request)
        {
            if (request == null)
                return BadRequest("Request inválido.");

            try
            {
                var result = await _repository.SetPackingAsync(request);

                var rowsUpdated = result.FirstOrDefault()?.RowsUpdated ?? 0;

                if (rowsUpdated == 0)
                {
                    return NotFound(new
                    {
                        message = "No se encontró información para empacar o la caja ya fue empacada."
                    });
                }

                return Ok(new
                {
                    message = "Caja empacada correctamente.",
                    rowsUpdated
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al empacar la caja.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("SetReceiveAsync")]
        public async Task<ActionResult<string>> SetReceiveAsync([FromBody] PackingRequestDTO request)
        {
            if (request == null)
                return BadRequest("Request inválido.");

            try
            {
                var result = await _repository.SetReceiveAsync(request);

                var rowsUpdated = result.FirstOrDefault()?.RowsUpdated ?? 0;

                if (rowsUpdated == 0)
                {
                    return NotFound(new
                    {
                        message = "No se encontró información para empacar o la caja ya fue empacada."
                    });
                }

                return Ok(new
                {
                    message = "Caja empacada correctamente.",
                    rowsUpdated
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al empacar la caja.",
                    error = ex.Message
                });
            }
        }


        [HttpPost("EnviarDespacho/{despachoId}")]
        public async Task<ActionResult<string>> EnviarTraslado([FromBody] List<TrasladoDespachoDTO> trasladoDespachoDTOs, int despachoId)
        {
            int contadorErroresAx = 0;
            foreach (var traslado in trasladoDespachoDTOs)
            {
                var respAX = _ax.EnviarRecibirTraslados(traslado.TransferId, "ENVIAR");
                var esExitoso = respAX.Contains("OK");
                if (!esExitoso)
                {
                    contadorErroresAx++;
                }
            }
            if (contadorErroresAx > 0)
            {
                return BadRequest("Se presentaron errores al enviar los traslados a AX.");
            }
            int statudId = 1;
            var respBaseLocal = await _repository.ChangeEstadoTraslado(despachoId, statudId);
            var SeActualizoCorrectamente = respBaseLocal.Contains("Ok");
            if (!SeActualizoCorrectamente)
            {
                return BadRequest("No se pudo actualizar el estado del despacho en la base local.");
            } 
            return  "Se envio el despacho";
        }

        [HttpPost("RecibirDespacho/{despachoId}")]
        public async Task<ActionResult<string>> RecibirDespacho([FromBody] List<TrasladoDespachoDTO> trasladoDespachoDTOs, int despachoId)
        {
            int contadorErroresAx = 0;
            foreach (var traslado in trasladoDespachoDTOs)
            {
                var respAX = _ax.EnviarRecibirTraslados(traslado.TransferId, "RECIBIR");
                var esExitoso = respAX.Contains("OK");
                if (!esExitoso)
                {
                    contadorErroresAx++;
                }
            }
            if (contadorErroresAx > 0)
            {
                return BadRequest("Se presentaron errores al enviar los traslados a AX.");
            }
            int statudId = 2;
            var respBaseLocal = await _repository.ChangeEstadoTraslado(despachoId, statudId);
            var SeActualizoCorrectamente = respBaseLocal.Contains("Ok");
            if (!SeActualizoCorrectamente)
            {
                return BadRequest("No se pudo actualizar el estado del despacho en la base local.");
            }
            return "Se envio el despacho";
        }




    }
}
