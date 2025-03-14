using Core.DTOs.IM_WMS_RecTela;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;
using Core.Interfaces;

namespace WMS_API.Features.Repositories
{
    public class MWMS_RecTelaRepository: IMWMS_RecTelaRepository
    {

        private readonly string _connectionString;
        
        public MWMS_RecTelaRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzasDev");
        }

        public async Task<List<IM_WMS_RecTela_GetListTelasDTO>> GetListTelas(string journalId)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@journalName","REC_TELAS"),
                new SqlParameter("@journalId",journalId)
            };

            var result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_RecTela_GetListTelasDTO>("[IM_WMS_RecTela_GetListTelas]", parametros);
   
            return result;
        }


        public async Task<List<IM_WMS_RecTela_PostTelaPickingMergeDTO>> PostTelaPickingMergeDTO(string journalId)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@journalId",journalId)
            };

            List<IM_WMS_RecTela_PostTelaPickingMergeDTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_RecTela_PostTelaPickingMergeDTO>("[IM_WMS_RecTela_PostTelaPickingMerge]", parametros);

            return result;
        }


        public async Task<List<IM_WMS_RecTela_UpdateTelaPickingIsScanningDTO>> UpdateTelaPickingIsScanning(List<UpdateTelaPickingIsScanningDto> telapicking)
        {
            var response = new List<IM_WMS_RecTela_UpdateTelaPickingIsScanningDTO>();
            for (int i = 0; i < telapicking.Count; i++)
            {

                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

                var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@userCode",telapicking[i].userCode),
                    new SqlParameter("@vendroll", telapicking[i].vendroll),
                    new SqlParameter("@journalid", telapicking[i].journalId),
                    new SqlParameter("@inventSerialId",telapicking[i].inventSerialId),
                    new SqlParameter("@location",telapicking[i].location),
                    new SqlParameter("@telaPickingDefectoId",telapicking[i].TelaPickingDefectoId)
                };

                IM_WMS_RecTela_UpdateTelaPickingIsScanningDTO result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_RecTela_UpdateTelaPickingIsScanningDTO>("[IM_WMS_RecTela_UpdateTelaPickingIsScanning]", parametros);
                response.Add(result);
            }

            return response;
        }


        public async Task<List<IM_WMS_RecTela_GetTelaPickingDefectoDTO>> GetTelaPickingDefecto()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>();

            List<IM_WMS_RecTela_GetTelaPickingDefectoDTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_RecTela_GetTelaPickingDefectoDTO>("[IM_WMS_RecTela_GetTelaPickingDefecto]", parametros);

            return result;
        }


        public async Task<List<IM_WMS_RecTela_GetTelaPickingRuleDTO>> GetTelaPickingRule()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>();

            List<IM_WMS_RecTela_GetTelaPickingRuleDTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_RecTela_GetTelaPickingRuleDTO>("[IM_WMS_RecTela_GetTelaPickingRule]", parametros);

            return result;
        }
    }
}
