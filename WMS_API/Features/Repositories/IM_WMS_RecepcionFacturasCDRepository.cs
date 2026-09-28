using Core.DTOs.GeneracionPrecios;
using Core.DTOs.RecepcionFacturasCD;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class IM_WMS_RecepcionFacturasCDRepository : IIM_WMS_RecepcionFacturasCDRepository
    {
        private readonly string _connectionString;

        public IM_WMS_RecepcionFacturasCDRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("IMFinanzas");
        }

        public async Task<List<FacturaRecepcionCDDto>> GetFacturasRecepcionCD(DateTime fechaIni, DateTime fechaFin)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@FechaIni", fechaIni.Date),
                new SqlParameter("@FechaFin", fechaFin.Date)
            };

            try
            {
                // Se usa la versión dinámica porque el mapeador genérico no llena DateTime? (FechaRecibidaEnCD).
                var filas = await executeProcedure.ExecuteStoredProcedureDynamic("[dbo].[usp_PedidosFacturasRecibidasCD]", parametros);

                return filas.Select(f => new FacturaRecepcionCDDto
                {
                    PV = f["PV"]?.ToString(),
                    Ruta = f["Ruta"]?.ToString(),
                    NombreCliente = f["NombreCliente"]?.ToString(),
                    Cuenta = f["Cuenta"]?.ToString(),
                    Factura = f["Factura"]?.ToString(),
                    FechaGeneracionFactura = Convert.ToDateTime(f["FechaGeneracionFactura"]),
                    FechaRecibidaEnCD = f["FechaRecibidaEnCD"] == null ? (DateTime?)null : Convert.ToDateTime(f["FechaRecibidaEnCD"]),
                    Ubicacion = f["Ubicacion"]?.ToString(),
                    Recibido = f["Recibido"]?.ToString()
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CuentaExcluidaRecepcionCDDto>> GetCuentasExcluidas()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            try
            {
                return await executeProcedure.ExecuteStoredProcedureList<CuentaExcluidaRecepcionCDDto>(
                    "[dbo].[IM_WMS_RecepcionFacturasCD_CuentasExcluidas_Obtener]", new List<SqlParameter>());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RespuestaSP> InsertarCuentaExcluida(CuentaExcluidaRecepcionCDRequest item)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@CuentaCliente", item.CuentaCliente ?? ""),
                new SqlParameter("@Motivo", string.IsNullOrWhiteSpace(item.Motivo) ? (object)DBNull.Value : item.Motivo),
                new SqlParameter("@Usuario", string.IsNullOrWhiteSpace(item.Usuario) ? (object)DBNull.Value : item.Usuario)
            };

            try
            {
                return await executeProcedure.ExecuteStoredProcedure<RespuestaSP>(
                    "[dbo].[IM_WMS_RecepcionFacturasCD_CuentasExcluidas_Insertar]", parametros);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ResultadoCuentaExcluidaRecepcionCDDto>> InsertarCuentasExcluidas(CuentasExcluidasRecepcionCDRequest request)
        {
            // Una llamada al SP por cuenta: cada una se valida contra AX y reporta su propio resultado.
            var cuentas = (request.Cuentas ?? new List<string>())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim().ToUpper())
                .Distinct()
                .ToList();

            var resultados = new List<ResultadoCuentaExcluidaRecepcionCDDto>();

            foreach (string cuenta in cuentas)
            {
                RespuestaSP respuesta = await InsertarCuentaExcluida(new CuentaExcluidaRecepcionCDRequest
                {
                    CuentaCliente = cuenta,
                    Motivo = request.Motivo,
                    Usuario = request.Usuario
                });

                resultados.Add(new ResultadoCuentaExcluidaRecepcionCDDto
                {
                    CuentaCliente = cuenta,
                    Exito = respuesta.Exito,
                    Mensaje = respuesta.Mensaje,
                    Id = respuesta.Id
                });
            }

            return resultados;
        }

        public async Task<RespuestaSP> EliminarCuentaExcluida(int id, string usuario)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@Usuario", string.IsNullOrWhiteSpace(usuario) ? (object)DBNull.Value : usuario)
            };

            try
            {
                return await executeProcedure.ExecuteStoredProcedure<RespuestaSP>(
                    "[dbo].[IM_WMS_RecepcionFacturasCD_CuentasExcluidas_Eliminar]", parametros);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
