using Core.DTOs.IM_PrepEnvOp;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/PrepEnvOp")]
    [ApiController]
    public class IM_PrepEnvOpController : Controller
    {
        private readonly IIM_PrepEnvOpRepository _IIM_PrepEnvOpRepository;
        private readonly IAX _AX;

        public IM_PrepEnvOpController(IIM_PrepEnvOpRepository IM_PrepEnvOpRepository, IAX AX)
        {
            _IIM_PrepEnvOpRepository = IM_PrepEnvOpRepository;
            _AX = AX;
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

        [HttpGet("HistoricoOpPreparada")]
        public async Task<ActionResult<dynamic>>
            HistoricoOpPreparada(
            [FromQuery] DateTime fechaInicioSemana,
            [FromQuery] DateTime fechaFinSemana,
            [FromQuery] string? area)
        {
            var resp = await _IIM_PrepEnvOpRepository.HistoricoOpPreparada(fechaInicioSemana, fechaFinSemana, area);

            var agrupado = resp
               .GroupBy(x => new { x.OrdenTrabajo, x.NoTraslado })
               .Select(g => new HistoricoOpAgrupadoDTO
               {
                   OrdenTrabajo = g.Key.OrdenTrabajo,
                   NoTraslado = g.Key.NoTraslado,
                  
                   NombreRecibidaPor = g.First().NombreRecibidaPor,
                   FechaDeEntrega = g.First().FechaDeEntrega,
                   IdDetalleOpEnviada = g.First().IdDetalleOpEnviada,
                   EntregadoPor = g.First().EntregadoPor,

                   Articulos = g.Select(a => new ArticuloHistoricoDTO
                   {
                       IdOpPreparada = a.IdOpPreparada,
                       Estilo = a.Estilo,
                       Lote = a.Lote,
                       Base = a.Base,
                       CodigoArticulo = a.CodigoArticulo,
                       NombreArticulo = a.NombreArticulo,
                       Color = a.Color,
                       CantidadTransferida = a.CantidadTransferida,
                       Area = a.Area,
                       Semana = a.Semana,
                       Year = a.Year,
                       Empacado = a.Empacado,
                       Enviado = a.Enviado,
                       IsComplete = a.IsComplete,
                       IsEmpaquetada = a.IsEmpaquetada,
                       IdDetalleOpEnviada = a.IdDetalleOpEnviada
                   }).ToList()
               })
               .ToList();
            return Ok(agrupado);
        }

        [HttpGet("HistoricoByOp")]
        public async Task<ActionResult<dynamic>>
            HistoricoByOp(
            [FromQuery] string ordenDeProduccion)
        {


            var resp = await _IIM_PrepEnvOpRepository.HistoricoByOp(ordenDeProduccion);

            var agrupado = resp
               .GroupBy(x => new { x.OrdenTrabajo, x.NoTraslado })
               .Select(g => new HistoricoOpAgrupadoDTO
               {
                   OrdenTrabajo = g.Key.OrdenTrabajo,
                   NoTraslado = g.Key.NoTraslado,

                   NombreRecibidaPor = g.First().NombreRecibidaPor,
                   FechaDeEntrega = g.First().FechaDeEntrega,
                   IdDetalleOpEnviada = g.First().IdDetalleOpEnviada,
                   EntregadoPor = g.First().EntregadoPor,

                   Articulos = g.Select(a => new ArticuloHistoricoDTO
                   {
                       IdOpPreparada = a.IdOpPreparada,
                       Estilo = a.Estilo,
                       Lote = a.Lote,
                       Base = a.Base,
                       CodigoArticulo = a.CodigoArticulo,
                       NombreArticulo = a.NombreArticulo,
                       Color = a.Color,
                       CantidadTransferida = a.CantidadTransferida,
                       Area = a.Area,
                       Semana = a.Semana,
                       Year = a.Year,
                       Empacado = a.Empacado,
                       Enviado = a.Enviado,
                       IsComplete = a.IsComplete,
                       IsEmpaquetada = a.IsEmpaquetada,
                       IdDetalleOpEnviada = a.IdDetalleOpEnviada
                   }).ToList()
               })
               .ToList();
            return Ok(agrupado);
        }

        [HttpGet("SyncListadoDeOp")]
        public async Task<ActionResult<ListadoDeOpResponseDTO>> SyncListadoDeOp(
            [FromQuery] DateTime fechaInicioSemana,
            [FromQuery] DateTime fechaFinSemana)
        {
            try
            {
                var resp = await _IIM_PrepEnvOpRepository.SyncListadoDeOp(fechaInicioSemana, fechaFinSemana);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al syncronizar: {ex.Message}");
            }
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


        [HttpGet("FirmaByIdDetalle/{idDetalleOpEnviada}")]
        public async Task<ActionResult<dynamic>> FirmaByIdDetalle(int idDetalleOpEnviada)
        {
            try
            {
                var resp = await _IIM_PrepEnvOpRepository.FirmaByIdDetalle(idDetalleOpEnviada);
                return Ok(resp.FirmaBase64);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener la imagen: {ex.Message}");
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
        public async Task<ActionResult<string>> PostPrintEtiquetasEnvio([FromBody] List<IM_PrepEnvOp_OpEtiqueta> dataOp, [FromQuery] string? ipImpresora)
        {
            try
            {
                if(dataOp.Count <= 0)
                {
                    return Ok("No se recibio la lista llena.");

                }
                else
                {

                    var data = new List<IM_PrepEnvOp_OpEtiqueta>();

                    foreach(var item in dataOp)
                    {
                        int cantidadEstimada = await _IIM_PrepEnvOpRepository.CantidadEstimadaPorOP(item.OrdenTrabajo);
                        data.Add( new IM_PrepEnvOp_OpEtiqueta
                        {
                            OrdenTrabajo = item.OrdenTrabajo,
                            CantidadEstimada = cantidadEstimada,
                            Area = item.Area,
                            Semana = item.Semana,
                            Year = item.Year

                        });
                    }

                    var resp = await _IIM_PrepEnvOpRepository.PostPrintEtiquetasEnvio(data, ipImpresora);
                    return Ok(resp);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al imprimir los traslados enviados: {ex.Message}");
            }
        }

        [HttpPost("PostTrasladoComoRecibido")]
        public async Task<ActionResult<List<IM_PrepEnvOp_TrasladoResultDTO>>> PostTrasladoComoRecibido(PostDetalleOpEnviadaResponseDTO response)
        {
            var results = new List<IM_PrepEnvOp_TrasladoResultDTO>();




            foreach (var item in response.ListaOpPorEnviar)
            {
                var result = await _AX.MarcarTrasladoComoRecibido(item.NoTraslado, "Enviar");
                results.Add(result);
            };

            return Ok(results);

        }


        [HttpPost("PostTrasladoComoEmpacado/{userCode}")]
        public async Task<ActionResult<dynamic>> PostTrasladoComoEmpacado([FromBody] ListaDeTrasladosPorOpReponseDTO data, string userCode)
        {
            var listaDeTraslados = new List<string>();
            var resultsAx = new List<IM_PrepEnvOp_TrasladoResultDTO>();
            var resultsUpdateOp = new List<IM_PrepEnvOp_UpdateOpPreparadaLikeEmpaquetadaDTO>();

            var listaDeOpPorTraslado = new List<dynamic>();

            foreach (var item in data.listaOp) // item = OP
            {
                var trasladosPorOp = await _IIM_PrepEnvOpRepository.ListaDeTrasladosPorOp(item, data.listaArea);

                // Verificamos si todos los traslados están completos
                bool todosCompletos = true;
                foreach (var t in trasladosPorOp)
                {
                    if (!t.IsComplete)
                    {
                        todosCompletos = false;
                        break;
                    }
                }

                if (todosCompletos)
                {
                    // Agregamos todos los traslados de la OP a listaDeTraslados
                    foreach (var t in trasladosPorOp)
                    {
                        if (!listaDeTraslados.Contains(t.NoTraslado))
                        {
                            listaDeTraslados.Add(t.NoTraslado);

                        }

                        listaDeOpPorTraslado.Add(new
                        {
                            NoTraslado = t.NoTraslado,
                            OrdenTrabajo = item
                        });

                    }
                }
                else
                {
                    // Agrupamos manualmente por NoTraslado para concatenar los items
                    var dicTraslados = new Dictionary<string, List<dynamic>>(); 
                    foreach (var t in trasladosPorOp)
                    {
                        if (!t.IsComplete)
                        {
                            if (!dicTraslados.ContainsKey(t.NoTraslado))
                                dicTraslados[t.NoTraslado] = new List<dynamic>();

                            dicTraslados[t.NoTraslado].Add(t);
                        }
                    }

                    // Generamos un solo mensaje por traslado
                    foreach (var kvp in dicTraslados)
                    {
                        var traslado = kvp.Key;
                        var articulosFaltantes = kvp.Value;
                        var mensajeArticulos = string.Join("\n", articulosFaltantes.Select(a => $"{a.NombreArticulo} - {a.CodigoArticulo}"));

                        resultsAx.Add(new IM_PrepEnvOp_TrasladoResultDTO
                        {
                            NoTraslado = $"{traslado} {item}",
                            IsComplete = false,
                            Message = $"WAR: Aun no puedes completar el traslado porque faltan empacar:\n{mensajeArticulos}"
                        });
                    }
                }
            }

            // Marcamos los traslados completos
            for (int i = 0; i < listaDeTraslados.Count; i++)
            {
                var traslado = listaDeTraslados[i];
                // En AX
                var result = await _AX.MarcarTrasladoComoRecibido(traslado, "Empacar");
                resultsAx.Add(result);
            }

            for (int i = 0; i < resultsAx.Count; i++)
            {
                var opGroup = resultsAx[i];

                var ordenDeTrabajoCorrespondiente = listaDeOpPorTraslado.Find(x => x.NoTraslado == opGroup.NoTraslado);

                if (opGroup.IsComplete)
                {
                    var result = await _IIM_PrepEnvOpRepository
                       .UpdateOpPreparadaEmpaquetada(ordenDeTrabajoCorrespondiente.OrdenTrabajo, opGroup.NoTraslado,  userCode);
                     resultsUpdateOp.AddRange(result);
                }
                if (ordenDeTrabajoCorrespondiente != null)
                {
                    opGroup.NoTraslado = $"{opGroup.NoTraslado} {ordenDeTrabajoCorrespondiente.OrdenTrabajo}";
                }
                

            }



            return Ok(new {
                resultsAx,
                resultsUpdateOp
            });
        }


    }
}
