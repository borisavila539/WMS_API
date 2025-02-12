using Core.DTOs.CAEX;
using Core.DTOs.CAEX.Departamento;
using Core.DTOs.CAEX.Guia;
using Core.DTOs.CAEX.Municipios;
using Core.DTOs.CAEX.Poblados;
using Core.DTOs.CAEX.TipoPieza;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWMSCAEXRepository
    {
        public Task<IM_WMSCAEX_CompanyCredencial> GetCompanyCredencial(string company);
        public Task<List<Departamento>> getDepartamentos(string user, string pass);
        public Task<List<Municipio>> GetMunicipios(string user, string pass);
        public Task<List<Poblado>> GetPoblados(string user, string pass);
        public Task<List<Pieza>> GetPiezas(string user, string pass);
        public Task<ResultadoGenerarGuia> GetGenerarGuia(RequestGenerarGuia data) ;
        public Task<IM_WMSCAEX_GetDatosCliente> GetDatosCliente(string cliente);
        public Task<IM_WMSCAEX_ObtenerDetallePickingRouteID> getDetallePickingRoute(string BoxCode);
        public Task<string> postImprimirEtiqueta(List<IM_WMSCAEX_CrearRutas_Cajas> urls);
        public Task<List<IM_WMSCAEX_ObtenerReimpresionEtiquetas>> getObtenerReimpresionEtiquetas(string BoxCode);

    }
}
