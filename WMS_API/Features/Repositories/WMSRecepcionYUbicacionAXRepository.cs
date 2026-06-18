using Core.DTOs.RecepcionYUbicacionAX;
using Core.DTOs.Serigrafia;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class WMSRecepcionYUbicacionAXRepository : IWMSRecepcionYUbicacionAX
    {
        private readonly string _connectionString;
        private readonly string _connectionStringPiso;



        public WMSRecepcionYUbicacionAXRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzas");
            _connectionStringPiso = configuracion.GetConnectionString("IMAplicativos");
        }

        public async Task<List<TrasladoAxDto>> GetTrasladosAXAsync()
        {
            var result = new List<TrasladoAxDto>();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

                result = await executeProcedure.ExecuteStoredProcedureList<TrasladoAxDto>("[IM_WMS_GetTrasladosAX_Ultimos3Meses]", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        public async Task<List<ReporteInformacionAXTrasladoDto>> GetReporteInformacionAXTrasladoAsync(string idTraslado)
        {
            var result = new List<ReporteInformacionAXTrasladoDto>();

            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionStringPiso);

                var parameter = new List<SqlParameter>
                {
                    new SqlParameter("@idTraslado",idTraslado),
                };

                result = await executeProcedure.ExecuteStoredProcedureList<ReporteInformacionAXTrasladoDto>(
                    "[SP_Reporte_InformacionAXTraslado]",
                    parameter
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public async Task<InformacionEmpresa> GetUbicacionEmpresa(string nombre)
        {
            InformacionEmpresa result = new InformacionEmpresa();
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Nombre", nombre),
                };
                var response = await executeProcedure.ExecuteStoredProcedure<InformacionEmpresa>(
                    "IM_WMS_GetUbicacionEmpresa",
                    parameters
                );
                result = response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;

        }
    }
}
