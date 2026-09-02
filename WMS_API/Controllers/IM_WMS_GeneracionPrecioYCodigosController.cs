using Core.DTOs.GeneracionPrecios;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IM_WMS_GeneracionPrecioYCodigosController : ControllerBase
    {
        private readonly IIM_WMS_GeneracionPrecioYCodigosRepository _repository;

        [HttpGet("ObtenerClientesGeneracionPrecio")]
        public async Task<IEnumerable<IM_WMS_ClientesGeneracionprecios>> GetClientesGeneracionprecios()
        {
            var resp = await _repository.GetClientesGeneracionprecios();
            return resp;
        }
        public IM_WMS_GeneracionPrecioYCodigosController(IIM_WMS_GeneracionPrecioYCodigosRepository repository)
        {
            _repository = repository;
        }
        [HttpPost("ImportarPlantillaPreciosExcel")]
        public async Task<IActionResult> ImportarPlantillaPreciosExcel(IFormFile file)
        {

            var resultado = _repository.ParsearExcel(file);
            if (!resultado.Success)
                return BadRequest(resultado);
            return Ok(resultado);
        }
        [HttpGet("GetConfiguracionPrecio")]
        public async Task<IActionResult> GetConfiguracionPrecio()
        {
            try
            {
                var data = await _repository.GetConfiguracionPrecio();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("InsertConfiguracionPrecio")]
        public async Task<IActionResult> InsertConfiguracionPrecio([FromBody] List<ConfiguracionPrecioDto> items,[FromQuery] int userId)
        {
            try
            {
                if (items == null || !items.Any())
                    return BadRequest("No se recibieron registros.");

                foreach (var item in items)
                {
                    await _repository.InsertConfiguracionPrecio(item, userId);
                }

                return Ok(new
                {
                    Success = true,
                    Message = $"{items.Count} registros insertados correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("InsertSingleConfiguracionPrecio")]
        public async Task<IActionResult> InsertSingleConfiguracionPrecio([FromBody] ConfiguracionPrecioDto item,[FromQuery] int userId)
        {
            try
            {
                if (item == null)
                    return BadRequest(new { Success = false, Message = "No se recibieron datos del registro." });

                int newId = await _repository.InsertConfiguracionPrecio(item, userId);

                return Ok(new
                {
                    Success = true,
                    Message = "Registro insertado correctamente.",
                    Id = newId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("UpdateConfiguracionPrecio")]
        public async Task<IActionResult> UpdateConfiguracionPrecio([FromBody] List<ConfiguracionPrecioDto> items,[FromQuery] int userId)
        {
            try
            {
                if (items == null || !items.Any())
                    return BadRequest("No se recibieron registros.");

                foreach (var item in items)
                {
                    await _repository.UpdateConfiguracionPrecio(item, userId);
                }

                return Ok(new
                {
                    Success = true,
                    Message = $"{items.Count} registros actualizados correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }

        }

        [HttpPost("UpdateSingleConfiguracionPrecio")]
        public async Task<IActionResult> UpdateSingleConfiguracionPrecio([FromBody] ConfiguracionPrecioDto item, [FromQuery] int userId)
        {
            try
            {
                  if (item == null)
                    return BadRequest(new { Success = false, Message = "No se actualiaron los registros" });

                 var reponse =    await _repository.UpdateConfiguracionPrecio(item, userId);


                return Ok(new
                {
                    Success = true,
                    Message = $"{reponse} registros actualizado correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }

        }

        [HttpPost("DeleteConfiguracionPrecio/{id}")]
        public async Task<IActionResult> DeleteConfiguracionPrecio(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("No se recibieron registros.");

                var response =    await _repository.DeleteConfiguracionPrecio(id);


                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("ObtenerDetalleGeneracionPrecios/{pedido}/{empresa}")]
        public async Task<IEnumerable<IM_WMS_HistoricoGeneracionPrecioCodigoDto>> GetObtenerDetalleGeneracionPrecios(string pedido, string empresa)
        {
            var resp = await _repository.GenerarHistoricoPrecios(pedido, empresa);
            
            return resp;
        }

        [HttpGet("ObtenerPedidoConPrecioConfigurado/{pedido}")]
        public async Task<IEnumerable<IM_WMS_HistoricoGeneracionPrecioCodigoDto>> ObtenerPedidoConPrecioConfigurado(string pedido, string empresa)
        {
            var resp = await _repository.ObtenerHistoricoPedidos(pedido);

            return resp;
        }

        [HttpPost("ConfirmarGeneracionPrecioLinea/{id}/{usuario}")]
        public async Task<ConfirmacionGeneracionPrecioDto>ConfirmarGeneracionPrecioLinea(int id,string usuario)
        {
            var resp = await _repository.ConfirmarGeneracionPrecioLinea(id,usuario,Environment.MachineName);

            return resp;
        }

        [HttpGet("GetPrecioCodigos")]
        public async Task<IEnumerable<IM_WMS_DetalleImpresionEtiquetasPrecio>> getDatosPrecioCodigos([FromQuery] ImpresionEtiqueta impresionEtiquetaParm)
        {
            var data = await _repository.GetDetalleImpresionEtiquetasPrecio(impresionEtiquetaParm);
            return data;
        }

        [HttpPost("ImpresionPrecioCodigosSeleccionados")]
        public async Task<string> PostImpresionPrecioCodigosSeleccionados([FromBody] ImpresionPreciosSeleccionDto request)
        {
            if (request?.Lineas == null || request.Lineas.Count == 0)
            {
                return "No se recibieron líneas para imprimir";
            }

            bool hayInvalidas = request.Lineas.Any(x => x.RequiereConfirmacion || x.Precio == 0);
            if (hayInvalidas)
            {
                return "Existen articulos sin precio o pendientes de confirmación";
            }

            var data = request.Lineas
                .OrderBy(x => x.IMIB_BOXCODE)
                .ThenBy(x => x.Articulo)
                .ThenBy(x => x.IDColor)
                .ThenBy(x => x.Talla)
                .ToList();

            string resp = "";
            string resp2 = "";
            string cajas = "";
            List<IM_WMS_DetalleImpresionEtiquetasPrecio> listado = new List<IM_WMS_DetalleImpresionEtiquetasPrecio>();

            data.ForEach(element =>
            {
                if (cajas == "" || cajas != element.IMIB_BOXCODE)
                {
                    if (listado.Count > 0)
                    {
                        // imprimir
                        resp2 = _repository.imprimirEtiquetaprecios2(listado, request.Fecha, request.Impresora);
                        if (resp2 != "OK")
                        {
                            resp += "Fallo Impresion :" + resp2 + ",";
                        }
                    }

                    cajas = element.IMIB_BOXCODE;
                    resp2 = _repository.imprimirEtiquetaCajaDividir(element.IMIB_BOXCODE, request.Impresora);
                    if (resp2 != "OK")
                    {
                        resp += "Fallo Impresion :" + resp2 + ",";
                    }
                    listado = new List<IM_WMS_DetalleImpresionEtiquetasPrecio>();
                }

                for (int i = 1; i <= element.QTY; i++)
                {
                    listado.Add(element);
                }
            });

            if (listado.Count > 0)
            {
                // imprimir
                resp2 = _repository.imprimirEtiquetaprecios2(listado, request.Fecha, request.Impresora);
                if (resp2 != "OK")
                {
                    resp += "Fallo Impresion :" + resp2 + ",";
                }
            }

            if (resp.Length > 0)
            {
                return resp;
            }

            return "OK";
        }

        [HttpGet("GetTallasConfiguracionPrecio")]
        public async Task<IActionResult> GetTallasConfiguracionPrecio()
        {
            try
            {
                var data = await _repository.GetTallasConfiguracionPrecio();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("InsertTallaConfiguracionPrecio")]
        public async Task<IActionResult> InsertTallaConfiguracionPrecio([FromBody] TallaConfiguracionPrecioDto item)
        {
            try
            {
                if (item == null)
                    return BadRequest(new { Success = false, Message = "No se recibieron datos del registro." });

                var response = await _repository.InsertarTallaConfiguracionPrecio(item);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("UpdateTallaConfiguracionPrecio")]
        public async Task<IActionResult> UpdateTallaConfiguracionPrecio([FromBody] TallaConfiguracionPrecioDto item)
        {
            try
            {
                if (item == null || item.Id <= 0)
                    return BadRequest(new { Success = false, Message = "El Id del registro es obligatorio." });

                var response = await _repository.ActualizarTallaConfiguracionPrecio(item);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("DeleteTallaConfiguracionPrecio/{id}")]
        public async Task<IActionResult> DeleteTallaConfiguracionPrecio(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { Success = false, Message = "No se recibieron registros." });

                var response = await _repository.EliminarTallaConfiguracionPrecio(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("ObtenerCategoriaPorClienteBase/{cuentaCliente}/{baseArticulo}")]
        public async Task<IActionResult> ObtenerCategoriaPorClienteBase(string cuentaCliente, string baseArticulo, [FromQuery] string empresa = "IMHN")
        {
            try
            {
                var data = await _repository.ObtenerCategoriaPorClienteBase(cuentaCliente, baseArticulo, empresa);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}