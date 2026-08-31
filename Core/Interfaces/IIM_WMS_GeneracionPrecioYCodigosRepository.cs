using Core.DTOs.Despacho_PT.GenerarDespachoPorImportacionExcel;
using Core.DTOs.GeneracionPrecios;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IIM_WMS_GeneracionPrecioYCodigosRepository
    {
        public ConfiguracionPrecioImportResultDto ParsearExcel(IFormFile file);
        public Task<List<ConfiguracionPrecioDto>> GetConfiguracionPrecio();
        public Task<int> InsertConfiguracionPrecio(ConfiguracionPrecioDto item, int userCreated);
        public Task<int> UpdateConfiguracionPrecio(ConfiguracionPrecioDto item, int userModified);
        public Task<SpResponseDTO> DeleteConfiguracionPrecio(int id);
        public Task<List<IM_WMS_ObtenerDetalleGeneracionPrecios>> GetObtenerDetalleGeneracionPrecios(string pedido, string empresa);
        public Task<List<IM_WMS_HistoricoGeneracionPrecioCodigoDto>> GenerarHistoricoPrecios(string pedido, string empresa);
        public Task<List<IM_WMS_HistoricoGeneracionPrecioCodigoDto>> ObtenerHistoricoPedidos(string pedido);
        public Task<ConfirmacionGeneracionPrecioDto> ConfirmarGeneracionPrecioLinea(int id, string usuario, string hostName);
        public Task<List<IM_WMS_DetalleImpresionEtiquetasPrecio>> GetDetalleImpresionEtiquetasPrecio(ImpresionEtiqueta parms);
        public string imprimirEtiquetaprecios2(List<IM_WMS_DetalleImpresionEtiquetasPrecio> data, string fecha, string impresora);
        public string imprimirEtiquetaCajaDividir(string caja, string impresora);

        public Task<List<TallaConfiguracionPrecioDto>> GetTallasConfiguracionPrecio();
        public Task<RespuestaSP> InsertarTallaConfiguracionPrecio(TallaConfiguracionPrecioDto item);
        public Task<RespuestaSP> ActualizarTallaConfiguracionPrecio(TallaConfiguracionPrecioDto item);
        public Task<RespuestaSP> EliminarTallaConfiguracionPrecio(int id);

        public Task<List<CategoriaPorClienteBaseDto>> ObtenerCategoriaPorClienteBase(string cuentaCliente, string baseArticulo, string empresa);

    }
}
