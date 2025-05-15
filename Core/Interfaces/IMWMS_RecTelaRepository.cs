using Core.DTOs.IM_WMS_RecTela;
using Core.DTOs.IM_WMS_RecTela.RecTelaByVendroll;
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
        public Task<string> EnviarCorreoDeRecepcionDeTela(string journalId);

        public Task<List<IM_WMS_RecTela_CorreosActivosDTO>> GetCorreosActivos();
        public Task<List<IM_WMS_RecTela_TelaJournalScanCountsDTO>> TelaJournalScanCounts(string journalId);
        public Task<List<IM_WMS_RecTela_DatosRollosProveedorDTO>> DatosRollosProveedor(string journalId);
        public Task<List<IM_WMS_RecTela_GetListTelasFilterDTO>> GetListTelasFilter(ParamsTelasFilterDTO parmsFilter);
        public Task<IM_WMS_RecTela_GetListTelasFilterByReferenceDTO> GetListTelasFilterByReference(ParamsTelasFilterDTO parmsFilter);
        public Task<string> PostPrintEtiquetasTela(List<IM_WMS_RecTela_PostTelaPickingMergeDTO> data, string ipImpresora);


        Task<List<IM_WMS_RecTela_TopTelaPickingByVendrollDTO>> TopTelaPickingByVendroll(string? nombreProveedor);
        Task<IM_WMS_RecTela_PostTelaPickingByVendrollDTO> PostTelaPickingByVendroll(IM_WMS_TelaPickingByVendrollBodyDTO body);
        Task<List<IM_WMS_RecTela_GetRolloByUUIDDTO>> GetRolloByUUID(string activityUUI);
        Task<List<IM_WMS_RecTela_GetListaDeTipoDeTelaDTO>> GetListaDeTipoDeTela(string? proveedorId);

        Task<List<IM_WMS_RecTela_GetListaProveedoresDTO>> GetListaProveedores(string nombreProveedor);
        Task<List<IM_WMS_TelaPickingByVendrollCorreosDTO>> TelaPickingByVendrollCorreos();
        Task<string> PostCorreoTelaPickingByVendroll(List<IM_WMS_RecTela_GetRolloByUUIDDTO> dataList);

    }
}
