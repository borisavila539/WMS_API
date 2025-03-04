using Core.DTOs.MB;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WMSMBController:ControllerBase
    {
        private readonly IWMSMBRespository _WMSMB;

        public WMSMBController(IWMSMBRespository WMSMB)
        {
            _WMSMB = WMSMB;
        }

        [HttpGet("InsertUpdateBox/{Orden}/{Caja}/{Ubicacion}/{Consolidado}/{usuarioRecepcion}/{Camion}")]
        public async Task<ActionResult<IM_WMS_MB_InsertBox>> GetInsertBox(string Orden, int Caja, string Ubicacion, int Consolidado, string usuarioRecepcion, string Camion)
        {
            var resp = await _WMSMB.getInsertBox(Orden, Caja, Ubicacion, Consolidado,usuarioRecepcion,Camion);
            return resp;
        }

        [HttpGet("ObtenerCajasRack/{ubicacion}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_MB_InsertBox>>> getCajasEscaneadasRack(string ubicacion)
        {
            var resp = await _WMSMB.getCajasEscaneadasRack(ubicacion);
            return resp;
        }
    }
}
