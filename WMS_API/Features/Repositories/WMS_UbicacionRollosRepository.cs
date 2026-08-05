using Core.DTOs;
using Core.DTOs.UbiacacionRollos;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class WMS_UbicacionRollosRepository:IWMS_UbiacionRollosRepository    
    {
        private readonly string _connectionString;
        public WMS_UbicacionRollosRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzas");

        }
        public async Task<RespuestaExistenciaUbicacion> GetExistenciaUbiacion(string ubicacion, string almacen)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            RespuestaExistenciaUbicacion result = new RespuestaExistenciaUbicacion();
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Ubiacion",ubicacion),
                new SqlParameter("@Almacen",almacen)
            };

            try
            {

                result = await executeProcedure.ExecuteStoredProcedure<RespuestaExistenciaUbicacion>("[IM_WMS_GetExistenciaUbiacion]", parametros);
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public async Task<RespuestaConsultarRollo> GetRolloParaCambioDeUbiacion(string codigoBarra)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            RespuestaConsultarRollo result = new RespuestaConsultarRollo();
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@NumeroRollo",codigoBarra),
            };
            try
            {
                result = await executeProcedure.ExecuteStoredProcedure<RespuestaConsultarRollo>("[IM_WMS_GetConsultarRollo]", parametros);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<List<InventarioRolloPorUbiacionAlmacenDto>> GetConsultarRollosPorUbicacion(string almacen, string ubicacion)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            List<InventarioRolloPorUbiacionAlmacenDto> result = new List<InventarioRolloPorUbiacionAlmacenDto>();
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Almacen",almacen),
                new SqlParameter("@Ubicacion",ubicacion)
            };

            try
            {
                result = await executeProcedure.ExecuteStoredProcedureList<InventarioRolloPorUbiacionAlmacenDto>("[IM_WMS_GetConsultarRollosPorUbicacion]", parametros);
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }
        public async Task<List<InventarioRolloPorAlmacenDto>> ConsultarInventarioRollosPorAlmacen(string almacen)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            List<InventarioRolloPorAlmacenDto> result = new List<InventarioRolloPorAlmacenDto>();
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Almacen",almacen),
            };

            try
            {
                result = await executeProcedure.ExecuteStoredProcedureList<InventarioRolloPorAlmacenDto>("[IM_WMS_ConsultarInventarioRollos]", parametros);
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public async Task<List<ValidarRolloRepetidoDto>> ValidarRolloRepetidoEnInventario(ValidarRolloRepetidoRequestDto request)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            List<ValidarRolloRepetidoDto> result = new List<ValidarRolloRepetidoDto>();

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@NumeroSerie", request.NumeroSerie),
            };

            try
            {
                result = await executeProcedure.ExecuteStoredProcedureList<ValidarRolloRepetidoDto>(
                    "[IM_WMS_ValidarRolloRepetidoEnInventario]",
                    parametros
                );
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        public async Task<List<DiarioMovimientoPendienteDto>> ObtenerDiariosMovimientoPendientes()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            List<DiarioMovimientoPendienteDto> result = new List<DiarioMovimientoPendienteDto>();
            var parametros = new List<SqlParameter>();
            try
            {
                result = await executeProcedure.ExecuteStoredProcedureList<DiarioMovimientoPendienteDto>("[dbo].[IM_WMS_GetDiariosMovimientoPendientes]", parametros);
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
        public async Task<List<DetalleDiarioMovimientoDto>> ObtenerDetalleDiarioMovimiento(string journalId)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            List<DetalleDiarioMovimientoDto> result = new List<DetalleDiarioMovimientoDto>();

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@JournalId", journalId),
            };

            try
            {
                result = await executeProcedure.ExecuteStoredProcedureList<DetalleDiarioMovimientoDto>("[dbo].[IM_WMS_GetDetalleDiarioMovimiento]", parametros);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
    }
}
