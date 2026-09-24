using Core.DTOs.InventarioMovimientos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IIM_WMS_InventarioMovimientosRepository
    {
        Task<List<MovimientoInventarioDto>> GetMovimientosInventario(DateTime fechaIni, DateTime fechaFin, string almacen, string articulo);
        Task<List<KardexInventarioDto>> GetKardexInventario(DateTime fechaIni, DateTime fechaFin, string almacen, string articulo, bool soloConMovimiento);
        Task<List<CorreoReporteInventarioDto>> GetCorreosReporteInventario();
        Task<List<AlmacenReporteInventarioDto>> GetAlmacenesReporteInventario();
        Task<byte[]> GenerarReporteInventarioExcel(DateTime fechaIni, DateTime fechaFin, string articulo);
    }
}
