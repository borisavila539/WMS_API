using Core.DTOs.GeneracionPrecios;
using Core.DTOs.RecepcionFacturasCD;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IIM_WMS_RecepcionFacturasCDRepository
    {
        Task<List<FacturaRecepcionCDDto>> GetFacturasRecepcionCD(DateTime fechaIni, DateTime fechaFin);
        Task<List<CuentaExcluidaRecepcionCDDto>> GetCuentasExcluidas();
        Task<RespuestaSP> InsertarCuentaExcluida(CuentaExcluidaRecepcionCDRequest item);
        Task<List<ResultadoCuentaExcluidaRecepcionCDDto>> InsertarCuentasExcluidas(CuentasExcluidasRecepcionCDRequest request);
        Task<RespuestaSP> EliminarCuentaExcluida(int id, string usuario);
    }
}
