using Core.DTOs;
using Core.DTOs.Despacho_PT;

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


        //===================================Despacho PT
        //Picking
        public Task<List<IM_WMS_Insert_Boxes_Despacho_PT_DTO>> GetInsert_Boxes_Despacho_PT(string ProdID,string userCreated, int Box);
        public Task<List<IM_WMS_Picking_Despacho_PT_DTO>> GetPicking_Despacho_PT(int Almacen);
        public Task<List<IM_WMS_Get_EstatusOP_PT_DTO>> get_EstatusOP_PT(int almacen);
        public Task<IM_WMS_Insert_Estatus_Unidades_OP_DTO> GetM_WMS_Insert_Estatus_Unidades_OPs(IM_WMS_Insert_Estatus_Unidades_OP_DTO data);
        public Task<IM_WMS_Crear_Despacho_PT> GetCrear_Despacho_PT(string driver, string truck, string userCreated, int almacen);
        public Task<List<IM_WMS_Get_Despachos_PT_DTO>> Get_Despachos_PT_DTOs(string estado, int almacen, int DespachoId);
        public Task<IM_WMS_Packing_DespachoPTDTO> GetPacking_DespachoPT(string ProdID, string userCreated, int Box, int DespachoID);
        public Task<List<IM_WMS_Picking_Despacho_PT_DTO>> GetDetalleDespachoPT(int DespachoID);
        public Task<IM_WMS_EnviarDespacho> Get_EnviarDespachos(int DespachoID,string user);
        public Task<List<IM_WMS_Get_Despachos_PT_DTO>> GetDespachosEstado(string estado);
        public Task<List<IM_WMS_ObtenerDespachoPTEnviados>> GetObtenerDespachoPTEnviados(int despachoID);
        public Task<IM_WMS_DespachoPT_RecibirDTO> GetRecibir_DespachoPT(string ProdID, string userCreated, int Box);
        public Task<List<IM_WMS_DespachoPT_CajasAuditarDTO>> getCajasAuditar(int despachoID);
        public Task<List<IM_WMS_Detalle_Auditoria_CajaDTO>> getDetalleAuditoriaCaja(string ProdID, int box);
        public Task<List<IM_WMS_Get_Despachos_PT_DTO>> getDespachosPTEstado(int DespachoID);
        

        //consulta OP
        public Task<List<IM_WMS_Consulta_OP_DetalleDTO>> getConsultaOPDetalle(string @Prodcutsheetid);
        public Task<List<IM_WMS_ConsultaOP_OrdenesDTO>> getConsultaOpOrdenes(string ProdCutSheetID, int DespachoID);
        public Task<List<IM_WMS_Consulta_OP_Detalle_CajasDTO>> getConsultaOPDetalleCajas(string ProdCutSheetID, int DespachoID);

        //==========================Transferir
        public Task<List<DiariosAbiertosDTO>> getObtenerDiarioTransferir(string user, string filtro);


    }
}
