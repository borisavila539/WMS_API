using Core.DTOs.IM_WMS_RecTela;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMWMS_RecTelaRepository
    {
        public Task<List<IM_WMS_RecTela_GetListTelasDTO>> GetListTelas(string journalId);
        public Task<List<IM_WMS_RecTela_PostTelaPickingMergeDTO>> PostTelaPickingMergeDTO(string journalId);
        public Task<List<IM_WMS_RecTela_UpdateTelaPickingIsScanningDTO>> UpdateTelaPickingIsScanning(List<UpdateTelaPickingIsScanningDto> telapicking);
        public Task<List<IM_WMS_RecTela_GetTelaPickingDefectoDTO>> GetTelaPickingDefecto();
        public Task<List<IM_WMS_RecTela_GetTelaPickingRuleDTO>> GetTelaPickingRule();
    }
}
