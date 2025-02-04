using Core.DTOs.CAEX;
using Core.DTOs.CAEX.Departamento;
using Core.DTOs.CAEX.Guia;
using Core.DTOs.CAEX.Municipios;
using Core.DTOs.CAEX.Poblados;
using Core.DTOs.CAEX.TipoPieza;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WMSCAEXController : ControllerBase
    {
        private readonly IWMSCAEXRepository _WMSCAEX;

        public WMSCAEXController(IWMSCAEXRepository wMSCAEX)
        {
            _WMSCAEX = wMSCAEX;
        }

        [HttpGet("CompanyCredencial/{Company}")]
        public async Task<ActionResult<IM_WMSCAEX_CompanyCredencial>> GetCompanyCredencial(string Company)
        {
            var resp = await _WMSCAEX.GetCompanyCredencial(Company);
            return resp;
        }

        [HttpGet("Departamentos/{Company}")]
        public async Task<ActionResult<IEnumerable<Departamento>>> getDepartamentos(string Company)
        {
            var CompanyInfo = await _WMSCAEX.GetCompanyCredencial(Company);

            var resp =await _WMSCAEX.getDepartamentos(CompanyInfo.Username, CompanyInfo.Password);

            return resp;
        }

        [HttpGet("Municipios/{Company}")]
        public async Task<ActionResult<IEnumerable<Municipio>>> GetMunicipios(string Company)
        {
            var CompanyInfo = await _WMSCAEX.GetCompanyCredencial(Company);

            var resp = await _WMSCAEX.GetMunicipios(CompanyInfo.Username, CompanyInfo.Password);

            return resp;
        }

        [HttpGet("Poblados/{Company}")]
        public async Task<ActionResult<IEnumerable<Poblado>>> GetPoblados(string Company)
        {
            var CompanyInfo = await _WMSCAEX.GetCompanyCredencial(Company);

            var resp = await _WMSCAEX.GetPoblados(CompanyInfo.Username, CompanyInfo.Password);

            return resp;
        }

        [HttpGet("Piezas/{Company}")]
        public async Task<ActionResult<IEnumerable<Pieza>>> GetPiezas(string Company)
        {
            var CompanyInfo = await _WMSCAEX.GetCompanyCredencial(Company);

            var resp = await _WMSCAEX.GetPiezas(CompanyInfo.Username, CompanyInfo.Password);

            return resp;
        }

        [HttpGet("Cliente/{cliente}")]
        public async Task<ActionResult<IM_WMSCAEX_GetDatosCliente>> GetDatosCliente(string cliente)
        {
            var resp = await _WMSCAEX.GetDatosCliente(cliente);
            return resp;
        }

        [HttpGet("ListaEmpaque/{BoxCode}")]
        public async Task<ActionResult<IM_WMSCAEX_ObtenerDetallePickingRouteID>> getDetallePickingRoute(string BoxCode)
        {
            var resp = await _WMSCAEX.getDetallePickingRoute(BoxCode);
            return resp;
        }

        [HttpGet("GenerarGuia/{Cuentacliente}/{ListasEmpaque}/{cajas}/{usuario}")]
        public async Task<ActionResult<ResultadoGenerarGuia>> GetGenerarGuia(string Cuentacliente, string ListasEmpaque, int cajas, string usuario)
        {
            var resp = await _WMSCAEX.GetGenerarGuia(Cuentacliente,ListasEmpaque,cajas,usuario) ;
            return resp;
        }
    }
}
