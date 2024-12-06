using Core.DTOs;
using Core.DTOs.AuditoriaCajasDenim;
using Core.DTOs.BusquedaRolloAX;
using Core.DTOs.Cajasrecicladas;
using Core.DTOs.ControCajasEtiquetado;
using Core.DTOs.DeclaracionEnvio;
using Core.DTOs.Despacho_PT;
using Core.DTOs.Despacho_PT.Liquidacion;
using Core.DTOs.Devoluciones;
using Core.DTOs.DiarioTransferir;
using Core.DTOs.GeneracionPrecios;
using Core.DTOs.InventarioCiclicoTela;
using Core.DTOs.RecepcionUbicacionCajas;
using Core.DTOs.TrackingPedidos;
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
        public Task<IM_WMS_EnviarDespacho> Get_EnviarDespachos(int DespachoID,string user,int cajasSegundas,int cajasTerceras);
        public Task<List<IM_WMS_Get_Despachos_PT_DTO>> GetDespachosEstado(string estado);
        public Task<List<IM_WMS_ObtenerDespachoPTEnviados>> GetObtenerDespachoPTEnviados(int despachoID);
        public Task<IM_WMS_DespachoPT_RecibirDTO> GetRecibir_DespachoPT(string ProdID, string userCreated, int Box);

        //Adutoria de cajas
        public Task<List<IM_WMS_DespachoPT_CajasAuditarDTO>> getCajasAuditar(int despachoID);
        public Task<List<IM_WMS_Detalle_Auditoria_CajaDTO>> getDetalleAuditoriaCaja(string ProdID, int box);
        public Task<IM_WMS_insertDetalleAdutoriaDenim> getInsertAuditoriaCajaTP(string ProdID, int Box, int IDUnico, int QTY);
        public Task<string> getEnviarCorreoAuditoriaTP(int DespachoID, string usuario);

        public Task<List<IM_WMS_Get_Despachos_PT_DTO>> getDespachosPTEstado(int DespachoID);
        public Task<List<IM_WMS_Correos_DespachoPTDTO>> getCorreosDespachoPT(string user);
        

        //consulta OP
        public Task<List<IM_WMS_Consulta_OP_DetalleDTO>> getConsultaOPDetalle(string @Prodcutsheetid);
        public Task<List<IM_WMS_ConsultaOP_OrdenesDTO>> getConsultaOpOrdenes(string ProdCutSheetID, int DespachoID);
        public Task<List<IM_WMS_Consulta_OP_Detalle_CajasDTO>> getConsultaOPDetalleCajas(string ProdCutSheetID, int DespachoID);

        public Task<IM_WMS_ObtenerSecuencia_PL_PT_DTO> getSecuencia_PL_PT(int despachoID, string user, int almacenTo);

        //Transferir
        public Task<IM_WMS_EnviarDiarioTransferirDTO> getEnviarDiarioTransferir(string JournalID, string userID);
        public Task<List<DiariosAbiertosDTO>> getObtenerDiarioTransferir(string user, string filtro);
        public Task<IM_WMS_InsertTransferirCajaDetalle> GetInsertTransferirCajaDetalle(string journalID, string ItemBarcode, string BoxNum,string Proceso);
        public Task<List<LineasDTO>> GetLineasDiarioTransferir(string diario);
        public Task<string> GetImprimirEtiquetaTransferir(string diario, string IMBoxCode, string PRINT);


        //busqueda de rollos en ax

        public Task<List<IM_WMS_BusquedaRollosAXDTO>> GetBusquedaRollosAX(string INVENTLOCATIONID,string INVENTSERIALID,string INVENTBATCHID,string INVENTCOLORID,string WMSLOCATIONID, string REFERENCE);

        //Liquidacion de la orden
        //Buscar despachos que esten recibidos
        public Task<List<IM_WMS_DespachosRecibidosLiquidacionDTO>> GetDespachosRecibidosLiquidacion(int despachoID);
        public Task<List<IM_WMS_DespachoPT_OrdenesRecibidasDepachoDTO>> GetOrdenesRecibidasDepacho(int despachoID);
        public Task<List<IM_WMS_DespachoPT_DetalleOrdenRecibidaLiquidacionDTO>> GetDetalleOrdenRecibidaLiquidacion(int despachoID, string ProdCutSheetID);

        //inventario cliclico de telas
        public Task<List<IM_WMS_InventarioCiclicoTelasDiariosAbiertos>> GetInventarioCiclicoTelasDiariosAbiertos();
        public Task<List<IM_WMS_InventarioCilicoTelaDiario>> Get_InventarioCilicoTelaDiarios(string JournalID);
        public Task<IM_WMS_InventarioCilicoTelaDiario> GetInventarioCilicoTelaDiario(string JournalID, string InventSerialID, string user, decimal QTY);
        public Task<IM_WMS_InventarioCilicoTelaDiario> Get_AgregarInventarioCilicoTelaDiario(string JournalID, string InventSerialID, string ubicacion, decimal QTY);
        public Task<List<IM_WMS_Correos_DespachoPTDTO>> getCorreoCiclicoTela();
        public Task<IM_WMS_InventarioCilicoTelaDiario> UpdateQTYInventSerialID(string InventserialID, decimal QTY);

        //Recepcion y ubicacion de cajas
        public Task<SP_GetBoxesReceived> getBoxesReceived(string opBoxNum, string ubicacion,string TIPO);
        public Task<List<SP_GetAllBoxesReceived>> getAllBoxesReceived( Filtros filtro);
        public Task<List<SP_GetAllBoxesReceived>> getAllBoxesReceived(string TIPO);
        public Task<string> postEnviarRecepcionUbicacionCajas(List<Ubicaciones> data);

        //Declaracion de envio

        public Task<SP_GetBoxesReceived> getBoxesReserved(string opBoxNum, string ubicacion);
        public Task<List<SP_GetAllBoxesReserved_V2>> GetAllBoxesReserved_V2(FiltroDeclaracionEnvio data);
        public Task<List<SP_GetAllBoxesReserved_V2>> GetAllBoxesReserved();
        public Task<List<IMDeclaracionEnvio>> GetDeclaracionEnvio(string pais, string ubicacion, string fecha);

        //Control cajas etiquetado

        public Task<IM_WMS_Insert_Control_Cajas_Etiquetado> GetControl_Cajas_Etiquetado(string caja, string empleado);
        public Task<List<IM_WMS_Control_Cajas_Etiquetado_Detalle>> Get_Control_Cajas_Etiquetado_Detalles(IM_WMS_Control_Cajas_Etiquetado_Detalle_Filtro filtro);

        //generacion de precios y codigos
        public Task<List<IM_WMS_ObtenerDetalleGeneracionPrecios>> GetObtenerDetalleGeneracionPrecios(string pedido, string empresa);
        public Task<List<IM_WMS_ObtenerPreciosCodigos>> GetObtenerPreciosCodigos(string cuentaCliente);
        public Task<IM_WMS_ObtenerPreciosCodigos> postInsertUpdatePrecioCodigos(IM_WMS_ObtenerPreciosCodigos data);
        public Task<List<IM_WMS_DetalleImpresionEtiquetasPrecio>> GetDetalleImpresionEtiquetasPrecio(string Pedido, string Ruta, string Caja);
        public string imprimirEtiquetaprecios(IM_WMS_DetalleImpresionEtiquetasPrecio data, int multiplo, int faltante,string fecha,string impresora);
        public string imprimirEtiquetaCajaDividir(string caja,string impresora);
        public Task<List<IM_WMS_ClientesGeneracionprecios>> GetClientesGeneracionprecios();
        public Task<IM_WMS_ClientesGeneracionprecios> postClienteGeneracionPrecio(IM_WMS_ClientesGeneracionprecios data);

        //Tracking Pedidos
        public Task<string> getEnviarCorreoTrackingPedidos( string fecha);
        public Task<List<IM_WMS_GenerarDetalleFacturas>> getObtenerDetalletrackingPedidos(TrackingPedidosFiltro filtro);

        //auditoria Denim
        public Task<List<IM_WMS_ObtenerDetalleAdutoriaDenim>> Get_ObtenerDetalleAdutoriaDenims(string OP, int Caja, string Ubicacion, string Usuario);
        public Task<IM_WMS_insertDetalleAdutoriaDenim> GetInsertDetalleAdutoriaDenim(int ID, int AuditoriaID);
        public Task<string> getEnviarCorreoAuditoriaDenim(string Ubicaicon,string usuario);

        //Reciclaje de cajas
        public Task<List<IM_WMS_CentroCostoReciclajeCajas>> GetCentroCostoReciclajeCajas();
        public Task<IM_WMS_InsertCajasRecicladashistorico> GetInsertCajasRecicladashistorico(string Camion, string Chofer, string CentroCostos, int QTY, string usuario, string diario);

        public Task<List<IM_WMS_InsertCajasRecicladashistorico>> GetCajasRecicladasPendiente();

        //Devoluciones
        public Task<List<IM_WMS_Devolucion_Busqueda>> getDevolucionesEVA(string filtro, int page, int size,int estado);
        public Task<List<IM_WMS_Devolucion_Detalle_RecibirPlanta>> getDevolucionDetalle(int id);
        public Task<List<IM_WMS_Devolucion_Detalle_RecibirPlanta>> getInsertDevolucionRecibidoEnviado(int id,int qty, string tipo);
        public Task<IM_WMS_Devolucion_Busqueda> getActualizarEstadoDevolucion(int id, string estado,string usuario);
        public Task<List<IM_WMS_Devolucion_Detalle_RecibirPlanta>> getDetalleDevolucionAuditoria(int id);
        public Task<List<IM_WMS_ObtenerEstructuraDefectosDevolucion>> GetObtenerEstructuraDefectosDevolucions();
        public Task<DefectosDevolucion> getActualizarDetalleDefectoDevolucion(int id, int idDefecto, string tipo);
        public Task<List<IM_WMS_Devolucion_Busqueda>> getObtenerDevolucionTracking(string filtro, int page, int size);
        public Task<string> getImprimirEtiquetasDevolucion(int id, string NumDevolucion, int CajaPrimera, int CajaIrregular);
        public Task<IM_WMS_CrearCajaDevolucion> getInsertarCajasDevolucion(string NumDevolucion, string usuario, int Caja);
        public Task<List<IM_WMS_DevolucionCajasPacking>> getDevolucionCajasPacking();


    }
}
