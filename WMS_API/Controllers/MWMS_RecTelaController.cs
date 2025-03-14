﻿using Core.DTOs.IM_WMS_RecTela;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<List<UpdateTelaPickingIsScanningDto>>> UpdateTelaPickingIsScanning( [FromBody] List<UpdateTelaPickingIsScanningDto> telaPicking)
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
    }
}
