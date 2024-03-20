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
        public Task<List<LineasDTO>> GetLineasReducionCajas(string IMBOXCODE);
        public Task<List<EtiquetaDTO>> GetDatosEtiquetaMovimiento(string diario, string IMBoxCode);
        public Task<string> GetImprimirEtiquetaMovimiento(string diario, string IMBoxCode, string PRINT);
        public Task<List<ImpresoraDTO>> getImpresoras();

        public Task<List<IM_WMS_Despacho_Tela_Detalle_AX>> GetIM_WMS_Despacho_Telas(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO);
        public Task<List<IM_WMS_Despacho_Tela_Detalle_Rollo>> Get_Despacho_Tela_Detalle_Rollo(string INVENTSERIALID,string InventTransID);

        public Task<List<DespachoTelasDetalleDTO>> GetDespacho_Telas(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO, string tipo);
        public Task<List<IM_WMS_Despacho_Tela_Detalle_Rollo>> GetDespacho_Tela_Picking_Packing(string INVENTSERIALID, string TIPO, string CAMION, string CHOFER, string InventTransID,string USER, int IDRemision );
        public Task<string> postImprimirEtiquetaRollo(List<EtiquetaRolloDTO> data);
        public Task<List<IM_WMS_TrasladosAbiertos>> getTrasladosAbiertos(string INVENTLOXATIONIDTO);
        public Task<List<IM_WMS_EstadoTrasladosDTO>> getEstadotraslados(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO);
        public Task<List<EstadoTrasladoTipoDTO>> gteEstadoTrasladoTipo(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO);
        public Task<List<CrearDespachoDTO>> GetCrearDespacho(string RecIDTraslados, string Chofer, string camion);
        public Task<List<CrearDespachoDTO>> GetObtenerDespachos(string RecIDTraslados);
        public Task<List<CerrarDespachoDTO>> getCerrarDespacho(int id);
        public Task<string> getNotaDespacho(int DespachoID, string recid,string empleado, string camio);
        public Task<List<RolloDespachoDTO>> getRollosDespacho(int despachoID);
        public Task<string> getImprimirEtiquetaReduccion(string IMBOXCODE, string ubicacion, string empacador,string PRINT);
    }
}
