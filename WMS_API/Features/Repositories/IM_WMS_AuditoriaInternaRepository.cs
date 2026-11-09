using Core.DTOs.AuditoriaInterna;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class IM_WMS_AuditoriaInternaRepository : IIM_WMS_AuditoriaInternaRepository
    {
        private readonly string _connectionString;

        public IM_WMS_AuditoriaInternaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("IMFinanzas");
        }

        public async Task<AuditoriaInternaAbrirCajaResponse> AbrirCaja(AbrirCajaAuditoriaRequest request)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@QRCaja", request.QRCaja.Trim()),
                new SqlParameter("@Usuario", request.Usuario.Trim())
            };

            var respuesta = (await Ejecutar<AuditoriaInternaRespuestaDto>("[dbo].[IM_WMS_AuditoriaInterna_AbrirCaja]", parametros)).FirstOrDefault();

            var response = new AuditoriaInternaAbrirCajaResponse { Respuesta = respuesta };

            // Tambien se devuelve la caja cerrada, para mostrar quien y cuando la audito
            if (respuesta != null && respuesta.Id > 0)
            {
                var caja = await ObtenerCaja(respuesta.Id);
                response.Caja = caja.Caja;
                response.Lineas = caja.Lineas;
            }

            return response;
        }

        public async Task<AuditoriaInternaEscanearResponse> EscanearPieza(EscanearPiezaAuditoriaRequest request)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@AuditoriaId", request.AuditoriaId),
                new SqlParameter("@Codigo", request.Codigo.Trim()),
                new SqlParameter("@Usuario", request.Usuario.Trim())
            };

            var escaneo = (await Ejecutar<AuditoriaInternaEscaneoDto>("[dbo].[IM_WMS_AuditoriaInterna_EscanearPieza]", parametros)).FirstOrDefault();

            return new AuditoriaInternaEscanearResponse
            {
                Escaneo = escaneo,
                Lineas = await ObtenerLineas(request.AuditoriaId)
            };
        }

        public async Task<AuditoriaInternaRespuestaDto> CerrarCaja(CerrarCajaAuditoriaRequest request)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@AuditoriaId", request.AuditoriaId),
                new SqlParameter("@Observaciones", (object)request.Observaciones ?? DBNull.Value),
                new SqlParameter("@Usuario", request.Usuario.Trim()),
                new SqlParameter("@Cerrar", request.Cerrar)
            };

            return (await Ejecutar<AuditoriaInternaRespuestaDto>("[dbo].[IM_WMS_AuditoriaInterna_CerrarCaja]", parametros)).FirstOrDefault();
        }

        public async Task<AuditoriaInternaAbrirCajaResponse> ObtenerCaja(int auditoriaId)
        {
            var parametros = new List<SqlParameter> { new SqlParameter("@AuditoriaId", auditoriaId) };

            var caja = (await Ejecutar<AuditoriaInternaCajaDto>("[dbo].[IM_WMS_AuditoriaInterna_ObtenerCaja]", parametros)).FirstOrDefault();

            return new AuditoriaInternaAbrirCajaResponse
            {
                Caja = caja,
                Lineas = caja == null ? new List<AuditoriaInternaLineaDto>() : await ObtenerLineas(auditoriaId)
            };
        }

        public async Task<List<AuditoriaInternaPendienteDto>> ObtenerPendientes()
        {
            return await Ejecutar<AuditoriaInternaPendienteDto>("[dbo].[IM_WMS_AuditoriaInterna_Pendientes]", null);
        }

        public async Task<List<AuditoriaInternaFaltanteDto>> ObtenerFaltantes(int auditoriaId)
        {
            var parametros = new List<SqlParameter> { new SqlParameter("@AuditoriaId", auditoriaId) };
            return await Ejecutar<AuditoriaInternaFaltanteDto>("[dbo].[IM_WMS_AuditoriaInterna_ObtenerFaltantes]", parametros);
        }

        public async Task<List<AuditoriaInternaHistorialEscaneoDto>> ObtenerEscaneos(int auditoriaId)
        {
            var parametros = new List<SqlParameter> { new SqlParameter("@AuditoriaId", auditoriaId) };
            return await Ejecutar<AuditoriaInternaHistorialEscaneoDto>("[dbo].[IM_WMS_AuditoriaInterna_ObtenerEscaneos]", parametros);
        }

        public async Task<AuditoriaInternaPermisoDto> PermisoReabrir(string usuario)
        {
            var parametros = new List<SqlParameter> { new SqlParameter("@Usuario", usuario.Trim()) };
            return (await Ejecutar<AuditoriaInternaPermisoDto>("[dbo].[IM_WMS_AuditoriaInterna_PermisoReabrir]", parametros))
                .FirstOrDefault() ?? new AuditoriaInternaPermisoDto();
        }

        public async Task<AuditoriaInternaRespuestaDto> ReabrirCaja(ReabrirCajaAuditoriaRequest request)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@AuditoriaId", request.AuditoriaId),
                new SqlParameter("@Motivo", (object)request.Motivo ?? DBNull.Value),
                new SqlParameter("@Usuario", request.Usuario.Trim())
            };

            return (await Ejecutar<AuditoriaInternaRespuestaDto>("[dbo].[IM_WMS_AuditoriaInterna_ReabrirCaja]", parametros)).FirstOrDefault();
        }

        public async Task<AuditoriaInternaRespuestaDto> ReiniciarCaja(ReabrirCajaAuditoriaRequest request)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@AuditoriaId", request.AuditoriaId),
                new SqlParameter("@Motivo", (object)request.Motivo ?? DBNull.Value),
                new SqlParameter("@Usuario", request.Usuario.Trim())
            };

            return (await Ejecutar<AuditoriaInternaRespuestaDto>("[dbo].[IM_WMS_AuditoriaInterna_ReiniciarCaja]", parametros)).FirstOrDefault();
        }

        public async Task<AuditoriaInternaRespuestaDto> EliminarCaja(ReabrirCajaAuditoriaRequest request)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@AuditoriaId", request.AuditoriaId),
                new SqlParameter("@Motivo", (object)request.Motivo ?? DBNull.Value),
                new SqlParameter("@Usuario", request.Usuario.Trim())
            };

            return (await Ejecutar<AuditoriaInternaRespuestaDto>("[dbo].[IM_WMS_AuditoriaInterna_EliminarCaja]", parametros)).FirstOrDefault();
        }

        public async Task<List<AuditoriaInternaHistorialDto>> ObtenerHistorial(int auditoriaId)
        {
            var parametros = new List<SqlParameter> { new SqlParameter("@AuditoriaId", auditoriaId) };
            return await Ejecutar<AuditoriaInternaHistorialDto>("[dbo].[IM_WMS_AuditoriaInterna_ObtenerHistorial]", parametros);
        }

        public async Task<AuditoriaInternaDashboardResumenDto> DashboardResumen(DateTime fechaIni, DateTime fechaFin)
        {
            return (await Ejecutar<AuditoriaInternaDashboardResumenDto>("[dbo].[IM_WMS_AuditoriaInterna_DashboardResumen]", ParametrosFechas(fechaIni, fechaFin)))
                .FirstOrDefault() ?? new AuditoriaInternaDashboardResumenDto();
        }

        public async Task<List<AuditoriaInternaDashboardContratistaDto>> DashboardPorContratista(DateTime fechaIni, DateTime fechaFin)
        {
            return await Ejecutar<AuditoriaInternaDashboardContratistaDto>("[dbo].[IM_WMS_AuditoriaInterna_DashboardPorContratista]", ParametrosFechas(fechaIni, fechaFin));
        }

        public async Task<List<AuditoriaInternaDashboardDiaDto>> DashboardPorDia(DateTime fechaIni, DateTime fechaFin)
        {
            return await Ejecutar<AuditoriaInternaDashboardDiaDto>("[dbo].[IM_WMS_AuditoriaInterna_DashboardPorDia]", ParametrosFechas(fechaIni, fechaFin));
        }

        public async Task<List<AuditoriaInternaDashboardCajaDto>> DashboardCajas(DateTime fechaIni, DateTime fechaFin, string porFecha)
        {
            var parametros = ParametrosFechas(fechaIni, fechaFin);
            parametros.Add(new SqlParameter("@PorFecha", porFecha == "AUDITORIA" ? "AUDITORIA" : "RECEPCION"));
            return await Ejecutar<AuditoriaInternaDashboardCajaDto>("[dbo].[IM_WMS_AuditoriaInterna_DashboardCajas]", parametros);
        }

        public async Task<List<AuditoriaInternaDashboardPendienteDto>> DashboardPendientes(DateTime fechaIni, DateTime fechaFin)
        {
            return await Ejecutar<AuditoriaInternaDashboardPendienteDto>("[dbo].[IM_WMS_AuditoriaInterna_DashboardPendientes]", ParametrosFechas(fechaIni, fechaFin));
        }

        private async Task<List<AuditoriaInternaLineaDto>> ObtenerLineas(int auditoriaId)
        {
            var parametros = new List<SqlParameter> { new SqlParameter("@AuditoriaId", auditoriaId) };
            return await Ejecutar<AuditoriaInternaLineaDto>("[dbo].[IM_WMS_AuditoriaInterna_ObtenerLineas]", parametros);
        }

        private static List<SqlParameter> ParametrosFechas(DateTime fechaIni, DateTime fechaFin)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@FechaIni", fechaIni.Date),
                new SqlParameter("@FechaFin", fechaFin.Date)
            };
        }

        // Se usa la version dinamica porque el mapeador generico no llena tipos que aceptan null (DateTime?, int?).
        private async Task<List<T>> Ejecutar<T>(string procedimiento, List<SqlParameter> parametros) where T : new()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var filas = await executeProcedure.ExecuteStoredProcedureDynamic(procedimiento, parametros);
            return filas.Select(Mapear<T>).ToList();
        }

        private static T Mapear<T>(Dictionary<string, object> fila) where T : new()
        {
            T obj = new T();

            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (!fila.TryGetValue(prop.Name, out var valor) || valor == null)
                    continue;

                Type tipo = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                if (tipo == typeof(string))
                    prop.SetValue(obj, valor.ToString());
                else if (tipo == typeof(bool))
                    prop.SetValue(obj, Convert.ToBoolean(valor));
                else
                    prop.SetValue(obj, Convert.ChangeType(valor, tipo));
            }

            return obj;
        }
    }
}
