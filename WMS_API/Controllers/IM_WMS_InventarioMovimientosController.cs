using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IM_WMS_InventarioMovimientosController : ControllerBase
    {
        private readonly IIM_WMS_InventarioMovimientosRepository _Repository;

        public IM_WMS_InventarioMovimientosController(IIM_WMS_InventarioMovimientosRepository iM_WMS_InventarioMovimientosRepository)
        {
            _Repository = iM_WMS_InventarioMovimientosRepository;
        }

        [HttpGet("Movimientos")]
        public async Task<IActionResult> GetMovimientos(
            [FromQuery] DateTime fechaIni,
            [FromQuery] DateTime fechaFin,
            [FromQuery] string almacen = null,
            [FromQuery] string articulo = null)
        {
            var resultado = await _Repository.GetMovimientosInventario(fechaIni, fechaFin, almacen, articulo);
            return Ok(resultado);
        }

        [HttpGet("ResumenMovimientos")]
        public async Task<IActionResult> GetKardex(
            [FromQuery] DateTime fechaIni,
            [FromQuery] DateTime fechaFin,
            [FromQuery] string almacen = null,
            [FromQuery] string articulo = null,
            [FromQuery] bool soloConMovimiento = true)
        {
            var resultado = await _Repository.GetKardexInventario(fechaIni, fechaFin, almacen, articulo, soloConMovimiento);
            return Ok(resultado);
        }

        [HttpGet("MovimientosReporteExcel")]
        public async Task<IActionResult> GetReporteExcel(
            [FromQuery] DateTime fechaIni,
            [FromQuery] DateTime fechaFin,
            [FromQuery] string articulo = null)
        {
            var excelBytes = await _Repository.GenerarReporteInventarioExcel(fechaIni, fechaFin, articulo);

            if (excelBytes == null || excelBytes.Length == 0)
                return NotFound(new { success = false, message = "No se encontraron movimientos de inventario para los filtros indicados." });

            string nombreArchivo = $"ReporteInventario_{fechaIni:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }
    }
}
