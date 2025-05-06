using Core.DTOs.IM_WMS_RecTela;
using Core.DTOs.IM_WMS_RecTela.RecTelaByVendroll;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/WMS/[controller]")]
    [ApiController]
    public class MWMS_RecTelaController : Controller
    {
        private readonly IMWMS_RecTelaRepository _IMWMS_RecTelaRepository;

        public MWMS_RecTelaController(IMWMS_RecTelaRepository MWMS_RecTelaController)
        {
            _IMWMS_RecTelaRepository = MWMS_RecTelaController;
        }

        [HttpGet("GetListTelas/{journalId}")]
        public async Task<ActionResult<List<IM_WMS_RecTela_GetListTelasDTO>>> GetListTelas(string journalId)
        {
            var resp = await _IMWMS_RecTelaRepository.GetListTelas(journalId);
            return Ok(resp);
        }

        [HttpPost("PostTelaPickingMerge/{journalId}")]
        public async Task<ActionResult<List<IM_WMS_RecTela_PostTelaPickingMergeDTO>>> PostTelaPickingMerge(string journalId)
        {
            var resp = await _IMWMS_RecTelaRepository.PostTelaPickingMergeDTO(journalId);
            return Ok(resp);
        }


        [HttpPost("UpdateTelaPickingIsScanning")]
        public async Task<ActionResult<List<UpdateTelaPickingIsScanningDto>>> UpdateTelaPickingIsScanning([FromBody] List<UpdateTelaPickingIsScanningDto> telaPicking)
        {
            var resp = await _IMWMS_RecTelaRepository.UpdateTelaPickingIsScanning(telaPicking);
            return Ok(resp);
        }

        [HttpGet("GetTelaPickingDefecto")]
        public async Task<ActionResult<List<IM_WMS_RecTela_GetTelaPickingDefectoDTO>>> GetTelaPickingDefecto()
        {
            var resp = await _IMWMS_RecTelaRepository.GetTelaPickingDefecto();
            return Ok(resp);
        }

        [HttpGet("GetTelaPickingRule")]
        public async Task<ActionResult<List<IM_WMS_RecTela_GetTelaPickingRuleDTO>>> GetTelaPickingRule()
        {
            var resp = await _IMWMS_RecTelaRepository.GetTelaPickingRule();
            return Ok(resp);
        }

        [HttpGet("EnviarCorreoDeRecepcionDeTela/{journalId}")]
        public async Task<ActionResult<string>> EnviarCorreoDeRecepcionDeTela(string journalId)
        {
            var resp = await _IMWMS_RecTelaRepository.EnviarCorreoDeRecepcionDeTela(journalId);
            return Ok(resp);
        }

        [HttpPost("GetListTelasFilter")]
        public async Task<ActionResult<List<IM_WMS_RecTela_GetListTelasFilterDTO>>> GetListTelasFilter([FromBody] ParamsTelasFilterDTO parmsFilter)
        {
            var resp = await _IMWMS_RecTelaRepository.GetListTelasFilter(parmsFilter);
            return Ok(resp);
        }


        [HttpPost("GetListTelasFilterByReference")]
        public async Task<ActionResult<dynamic>> GetListTelasFilterByReference([FromBody] ParamsTelasFilterDTO parmsFilter)
        {
            var resp = await _IMWMS_RecTelaRepository.GetListTelasFilterByReference(parmsFilter);
            return Ok(resp);
        }


        [HttpPost("PostPrintEtiquetasTela/{ipImpresora}")]
        public async Task<ActionResult<dynamic>> PostPrintEtiquetasTela([FromBody] List<IM_WMS_RecTela_PostTelaPickingMergeDTO> data, string ipImpresora)
        {
            var resp = await _IMWMS_RecTelaRepository.PostPrintEtiquetasTela(data, ipImpresora);
            return Ok(resp);
        }

        [HttpGet("TopTelaPickingByVendroll")]
        public async Task<ActionResult<List<IM_WMS_RecTela_TopTelaPickingByVendrollDTO>>> TopTelaPickingByVendroll()
        {
            var resp = await _IMWMS_RecTelaRepository.TopTelaPickingByVendroll();
            return Ok(resp);
        }

        [HttpPost("PostTelaPickingByVendroll")]
        public async Task<ActionResult<IM_WMS_RecTela_PostTelaPickingByVendrollDTO>> PostTelaPickingByVendroll([FromBody] IM_WMS_TelaPickingByVendrollBodyDTO data)
        {
            var resp = await _IMWMS_RecTelaRepository.PostTelaPickingByVendroll(data);
            return Ok(resp);
        }

        [HttpGet("GetRollorByUUID/{activityUUI}")]
        public async Task<ActionResult<List<IM_WMS_RecTela_TopTelaPickingByVendrollDTO>>> GetRollorByUUID(string activityUUI)
        {
            var resp = await _IMWMS_RecTelaRepository.GetRollorByUUID(activityUUI);
            return Ok(resp);
        }

        [HttpGet("GetListaDeTipoDeTela")]
        public async Task<ActionResult<List<IM_WMS_RecTela_TopTelaPickingByVendrollDTO>>> GetListaDeTipoDeTela()
        {
            var resp = await _IMWMS_RecTelaRepository.GetListaDeTipoDeTela();
            return Ok(resp);
        }

        [HttpGet("GetListaProveedores")]
        public async Task<ActionResult<List<IM_WMS_RecTela_GetListaProveedoresDTO>>> GetListaProveedores()
        {
            var resp = await _IMWMS_RecTelaRepository.GetListaProveedores();
            return Ok(resp);
        }

    }
}
