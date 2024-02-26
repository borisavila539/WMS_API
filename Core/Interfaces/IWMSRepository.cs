using Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWMSRepository
    {
        public Task<LoginDTO> PostLogin(LoginDTO datos);
        public Task<List<DiariosAbiertosDTO>> GetDiariosAbiertos(string user, string filtro);
        public Task<List<LineasDTO>> GetLineasDiario(string diario);
        public Task<List<EtiquetaDTO>> GetDatosEtiquetaMovimiento(string diario, string IMBoxCode);
        public Task<string> GetImprimirEtiquetaMovimiento(string diario, string IMBoxCode, string PRINT);
        public Task<List<ImpresoraDTO>> getImpresoras();

        public Task<List<IM_WMS_Despacho_Tela_Detalle_AX>> GetIM_WMS_Despacho_Telas(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO);
        public Task<List<IM_WMS_Despacho_Tela_Detalle_Rollo>> Get_Despacho_Tela_Detalle_Rollo(string INVENTSERIALID);

        public Task<List<DespachoTelasDetalleDTO>> GetDespacho_Telas(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO, string tipo);
        public Task<List<IM_WMS_Despacho_Tela_Detalle_Rollo>> GetDespacho_Tela_Picking_Packing(string INVENTSERIALID, string TIPO, string CAMION, string CHOFER);
        public Task<string> postImprimirEtiquetaRollo(List<EtiquetaRolloDTO> data);
        public Task<List<IM_WMS_TrasladosAbiertos>> getTrasladosAbiertos(string INVENTLOXATIONIDTO);
    }
}
