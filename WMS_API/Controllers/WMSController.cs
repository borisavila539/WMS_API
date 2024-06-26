﻿using Core.DTOs;
using Core.DTOs.Despacho_PT;
using Core.DTOs.DiarioTransferir;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WMSController:ControllerBase
    {
        private readonly IWMSRepository _WMS;
        private readonly IAX _AX;

        public WMSController(IWMSRepository WMS, IAX AX)
        {
            _WMS = WMS;
            _AX = AX;
        }

        [HttpGet("DiariosAbiertos/{user}/{filtro}")]
        public async Task<ActionResult<IEnumerable<DiariosAbiertosDTO>>> GetDiariosAbiertos(string user, string filtro)
        {
            var resp = await _WMS.GetDiariosAbiertos(user, filtro);
            return Ok(resp);
        }

        [HttpGet("LineasDiario/{diario}")]
        public async Task<ActionResult<IEnumerable<LineasDTO>>> GetDiariosAbiertos(string diario)
        {
            var resp = await _WMS.GetLineasDiario(diario);
            return Ok(resp);
        }

        [HttpGet("InsertDeleteMovimiento/{JOURNALID}/{IMBOXCODE}/{ITEMBARCODE}/{PROCESO}")]
        public string GetInsertMovimiento(string JOURNALID, string IMBOXCODE,string ITEMBARCODE,string PROCESO)
        {
            var resp = _AX.InsertDeleteMovimientoLine(JOURNALID, ITEMBARCODE,PROCESO,IMBOXCODE);
            return resp;
        }

        [HttpGet("EnviarRecibirtraslado/{TRANSFERID}/{ESTADO}")]
        public string getEnviarRecibirTraslado(string TRANSFERID, string ESTADO)
        {
            var resp = _AX.EnviarRecibirTraslados(TRANSFERID,ESTADO);
            return resp;
        }
        [HttpGet("InsertDeleteRecuccionCajas/{IMBOXCODE}/{ITEMBARCODE}/{PROCESO}")]
        public string GetinsertDeleteReduccionCajas( string IMBOXCODE, string ITEMBARCODE, string PROCESO)
        {
            var resp = _AX.INsertDeleteReduccionCajas( ITEMBARCODE, PROCESO, IMBOXCODE);
            return resp;
        }

        [HttpGet("ImprimirEtiquetaMovimiento/{JOURNALID}/{IMBOXCODE}/{PRINT}")]
        public Task<string> GetImprimirEtiquetaMovimiento(string JOURNALID, string IMBOXCODE, string PRINT)
        {
            var resp = _WMS.GetImprimirEtiquetaMovimiento(JOURNALID,IMBOXCODE,PRINT);
            return resp;
        }

        [HttpGet("Impresoras")]
        public async Task<ActionResult<IEnumerable<ImpresoraDTO>>> GetImpresoras()
        {
            var resp = await _WMS.getImpresoras();
            return resp;
        }

        [HttpGet("DespachotelasDetalle/{TRANSFERIDFROM}/{TRANSFERIDTO}/{INVENTLOCATIONIDTO}/{TIPO}")]
        public async Task<ActionResult<IEnumerable<DespachoTelasDetalleDTO>>> GetDespachotelasDetalle(string TRANSFERIDFROM,string TRANSFERIDTO, string INVENTLOCATIONIDTO,string TIPO)
        {
            var resp = await _WMS.GetDespacho_Telas(TRANSFERIDFROM, TRANSFERIDTO, INVENTLOCATIONIDTO,TIPO);
            return resp;
        }

        [HttpGet("DespachoTelaPickingPacking/{INVENTSERIALID}/{TIPO}/{CAMION}/{CHOFER}/{InventTransID}/{USER}/{ID}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_Despacho_Tela_Detalle_Rollo>>> GetDespachoTelaPickingPacking(string INVENTSERIALID, string TIPO, string CAMION, string CHOFER, string InventTransID,string USER,int ID)
        {
            var resp = await _WMS.GetDespacho_Tela_Picking_Packing(INVENTSERIALID, TIPO,CAMION,CHOFER,InventTransID,USER,ID);
            return resp;
        }
        [HttpGet("TrasladosAbiertos/{INVENTLOCATIONID}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_TrasladosAbiertos>>> getTrasladosAbiertos(string INVENTLOCATIONID)
        {
            var resp = await _WMS.getTrasladosAbiertos(INVENTLOCATIONID);
            return resp;
        }
        [HttpGet("Estadotraslados/{TRANSFERIDFROM}/{TRANSFERIDTO}/{INVENTLOCATIONIDTO}")]
        public async Task<IEnumerable<IM_WMS_EstadoTrasladosDTO>> getEstadotraslados(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO)
        {
            var resp = await _WMS.getEstadotraslados(TRANSFERIDFROM,TRANSFERIDTO,INVENTLOCATIONIDTO);
            return resp;
        }
        [HttpGet("EstadoTrasladoTipo/{TRANSFERIDFROM}/{TRANSFERIDTO}/{INVENTLOCATIONIDTO}")]
        public async Task<IEnumerable<EstadoTrasladoTipoDTO>> GetEstadoTrasladoTipos(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO)
        {
            var resp = await _WMS.gteEstadoTrasladoTipo(TRANSFERIDFROM, TRANSFERIDTO, INVENTLOCATIONIDTO);
            return resp;
        }

        [HttpGet("CrearDespacho/{RecIDTraslados}/{Chofer}/{camion}")]
        public async Task<IEnumerable<CrearDespachoDTO>> GetCrearDespachos(string RecIDTraslados, string Chofer, string camion)
        {
            var resp = await _WMS.GetCrearDespacho(RecIDTraslados, Chofer, camion);
            return resp;
        }

        [HttpGet("ObternerDespacho/{RecIDTraslados}")]
        public async Task<IEnumerable<CrearDespachoDTO>> GetObtenerDespachos(string RecIDTraslados)
        {
            var resp = await _WMS.GetObtenerDespachos(RecIDTraslados);
            return resp;
        }
        [HttpGet("CerrarDespacho/{ID}")]
        public async Task<IEnumerable<CerrarDespachoDTO>> getCerrarDespacho(int ID)
        {
            var resp = await _WMS.getCerrarDespacho(ID);
            return resp;
        }

        [HttpGet("NotaDespacho/{DESPACHOID}/{RECID}/{EMPLEADO}/{CAMION}")]
        public async Task<string> getNotaDespacho(int DESPACHOID,string RECID, string EMPLEADO,string CAMION)
        {
            var resp = await _WMS.getNotaDespacho(DESPACHOID, RECID, EMPLEADO,CAMION);
            return resp;
        }
        [HttpGet("RollosDespacho/{DespachoID}")]
        public async Task<IEnumerable<RolloDespachoDTO>> GetRollosDespacho(int DespachoID)
        {
            var resp = await _WMS.getRollosDespacho(DespachoID);
            return resp;
        }

        [HttpGet("LineasReduccion/{IMBOXCODE}")]
        public async Task<ActionResult<IEnumerable<LineasDTO>>> GetLineasReduccion(string IMBOXCODE)
        {
            var resp = await _WMS.GetLineasReducionCajas(IMBOXCODE);
            return Ok(resp);
        }

        [HttpGet("ImprimirEtiquetaReduccion/{IMBOXCODE}/{UBICACION}/{USER}/{PRINT}")]
        public  Task<string> getImprimirEtiquetaReduccion(string IMBOXCODE,string UBICACION,string USER, string PRINT)
        {
            var resp = _WMS.getImprimirEtiquetaReduccion(IMBOXCODE,UBICACION,USER,PRINT);
            return resp;
        }
       
        [HttpPost("Login")]
        public async Task<ActionResult<LoginDTO>> PostLogin(LoginDTO datos)
        {
            var resp = await _WMS.PostLogin(datos);

            return Ok(resp);
        }

        [HttpPost("ImprimirEtiquetaRollo")]
        public  Task<string> postImprimirEtiquetaRollo(List<EtiquetaRolloDTO> data)
        {
            var resp =  _WMS.postImprimirEtiquetaRollo(data);
            return resp;
        }

        //Despacho PT

        [HttpGet("InsertBoxesDespachoPT/{ProdID}/{UserCreated}/{Box}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_Insert_Boxes_Despacho_PT_DTO>>> GetInsert_Boxes_Despacho_PT(string ProdID,string UserCreated,int Box)
        {
            var resp = await _WMS.GetInsert_Boxes_Despacho_PT(ProdID, UserCreated, Box);
            return resp;
        }

        [HttpGet("PickingDespachoPT/{Almacen}")]
        public async Task<IEnumerable<IM_WMS_Picking_Despacho_PT_DTO>> GetPicking_Despacho_PT(int Almacen)
        {
            var resp = await _WMS.GetPicking_Despacho_PT(Almacen);
            return resp;
        }

        [HttpGet("EstatusOP_PT/{almacen}")]
        public async Task<IEnumerable<IM_WMS_Get_EstatusOP_PT_DTO>> Get_EstatusOP_PT(int almacen)
        {
            var resp = await _WMS.get_EstatusOP_PT(almacen);
            return resp;
        }

        [HttpPost("Insert_Estatus_Unidades_OP")]
        public async Task<ActionResult<IM_WMS_Insert_Estatus_Unidades_OP_DTO>> GetIM_WMS_Insert_Estatus_Unidades_OPs(IM_WMS_Insert_Estatus_Unidades_OP_DTO data)
        {
            var resp = await _WMS.GetM_WMS_Insert_Estatus_Unidades_OPs(data);
            return Ok(resp);
        }

        [HttpGet("Crear_Despacho_PT/{driver}/{truck}/{userCreated}/{almacen}")]
        public async Task<ActionResult<IM_WMS_Crear_Despacho_PT>> getCrear_Despacho_PT(string driver, string truck, string userCreated,int almacen)
        {
            var resp = await _WMS.GetCrear_Despacho_PT(driver, truck, userCreated,almacen);
            return resp;
        }

        [HttpGet("ObtenerDespachosPT/{estado}/{almacen}/{DespachoId}")]
        public async Task<IEnumerable<IM_WMS_Get_Despachos_PT_DTO>> Get_Despachos_PT(string estado, int almacen, int DespachoId)
        {
            var resp = await _WMS.Get_Despachos_PT_DTOs(estado, almacen, DespachoId);
            return resp;
        }

        [HttpGet("packingDespachoPT/{ProdID}/{UserCreated}/{Box}/{DespachoID}")]
        public async Task<ActionResult<IM_WMS_Packing_DespachoPTDTO>> GetPacking_DespachoPT(string ProdID, string UserCreated, int Box, int DespachoID)
        {
            var resp = await _WMS.GetPacking_DespachoPT(ProdID, UserCreated, Box, DespachoID);
            return resp;
        }

        [HttpGet("PackingDespachoPT/{DespachoID}")]
        public async Task<IEnumerable<IM_WMS_Picking_Despacho_PT_DTO>> GetPacking_Despacho_PT(int DespachoID)
        {
            var resp = await _WMS.GetDetalleDespachoPT(DespachoID);
            return resp;
        }

        [HttpGet("DespachoPTEnviados/{DespachoID}")]
        public async Task<IEnumerable<IM_WMS_ObtenerDespachoPTEnviados>> GetObtenerDespachoPTEnviados(int DespachoID)
        {
            var resp = await _WMS.GetObtenerDespachoPTEnviados(DespachoID);
            return resp;
        }

        //Enviar despacho
        [HttpGet("EnviarDespachoPT/{DespachoID}/{user}/{cajasSegundas}/{cajasTerceras}")]
        public async Task<ActionResult<IM_WMS_EnviarDespacho>> Get_EnviarDespachos(int DespachoID,string user,int cajasSegundas,int cajasTerceras)
        {
            var resp = await _WMS.Get_EnviarDespachos(DespachoID,user,cajasSegundas,cajasTerceras);
            return resp;
        }

        [HttpGet("DespachoPTEstado/{estado}")]
        public async Task<IEnumerable<IM_WMS_Get_Despachos_PT_DTO>> Get_Despachos_PT_Enviados(string estado)
        {
            var resp = await _WMS.GetDespachosEstado(estado);
            return resp;
        }

        //Recibir Despacho
        [HttpGet("ReceiveDespachoPT/{ProdID}/{UserCreated}/{Box}")]
        public async Task<ActionResult<IM_WMS_DespachoPT_RecibirDTO>> GetRecibir_DespachoPT(string ProdID, string UserCreated, int Box)
        {
            var resp = await _WMS.GetRecibir_DespachoPT(ProdID, UserCreated, Box);
            return resp;
        }

        //Cajas a auditar del despacho ?
        [HttpGet("DespachoPTCajasAuditar/{DespachoID}")]
        public async Task<IEnumerable<IM_WMS_DespachoPT_CajasAuditarDTO>> getCajasAuditar(int DespachoID)
        {
            var resp = await _WMS.getCajasAuditar(DespachoID);
            return resp;
        }

        //detalle de la caja auditoria

        [HttpGet("DetalleAuditoriaCaja/{ProdID}/{Box}")]
        public async Task<IEnumerable<IM_WMS_Detalle_Auditoria_CajaDTO>> getDetalleAuditoriaCaja(string ProdID, int Box)
        {
            var resp = await _WMS.getDetalleAuditoriaCaja(ProdID,Box);
            return resp;
        }


        //Consulta OP


        [HttpGet("DespachosPTEstado/{DespachoID}")]
        public async Task<IEnumerable<IM_WMS_Get_Despachos_PT_DTO>> getDespachosPTEstado( int DespachoID)
        {
            var resp = await _WMS.getDespachosPTEstado(DespachoID);
            return resp;
        }
        
        [HttpGet("DespachosPTConsultaOrdenes/{ProdCutSheetID}/{DespachoID}")]
        public async Task<IEnumerable<IM_WMS_ConsultaOP_OrdenesDTO>> getConsultaOpOrdenes( string ProdCutSheetID, int DespachoID)
        {
            var resp = await _WMS.getConsultaOpOrdenes(ProdCutSheetID,DespachoID);
            return resp;
        }
        //Consulta OP Detalle

        [HttpGet("ConsultaOPDetalle/{ProdCutSheetID}")]
        public async Task<IEnumerable<IM_WMS_Consulta_OP_DetalleDTO>> getConsultaOPDetalle( string ProdCutSheetID)
        {
            var resp = await _WMS.getConsultaOPDetalle(ProdCutSheetID);
            return resp;
        }

        //Consulta Op Detalle cajas
        
        [HttpGet("ConsultaOPDetalleCajas/{ProdCutSheetID}/{DespachoID}")]
        public async Task<IEnumerable<IM_WMS_Consulta_OP_Detalle_CajasDTO>> getConsultaOPDetalleCajas(string ProdCutSheetID, int DespachoID)
        {
            var resp = await _WMS.getConsultaOPDetalleCajas(ProdCutSheetID, DespachoID);
            return resp;
        }







        //entrada de Diarios de movimiento

        [HttpGet("EntradaMovimiento/{JOURNALID}/{ITEMBARCODE}/{PROCESO}")]
       
        public string GetEntradaMovimiento(string JOURNALID, string ITEMBARCODE, string PROCESO)
        {
            var resp = _AX.InsertDeleteEntradaMovimientoLine(JOURNALID, ITEMBARCODE, PROCESO);
            return resp;
        }

        //transferir de almacen a otro almacen
        [HttpGet("TransferirMovimiento/{JOURNALID}/{ITEMBARCODE}/{PROCESO}")]
        public string GetTransferirMovimientoLine(string JOURNALID, string ITEMBARCODE, string PROCESO)
        {
            var resp = _AX.InsertDeleteTransferirMovimientoLine(JOURNALID, ITEMBARCODE, PROCESO);
            return resp;
        }

        [HttpGet("EnviarCorreotransferir/{JOURNALID}/{USERID}")]
        public Task<IM_WMS_EnviarDiarioTransferirDTO> GetEnviarCorreotransferir(string JOURNALID, string USERID)
        {
            var resp =  _WMS.getEnviarDiarioTransferir(JOURNALID, USERID);
            return resp;
        }

        [HttpGet("DiariosTransferirAbiertos/{user}/{filtro}")]
        public async Task<ActionResult<IEnumerable<DiariosAbiertosDTO>>> GetDiariosTransferirAbiertos(string user, string filtro)
        {
            var resp = await _WMS.getObtenerDiarioTransferir(user, filtro);
            return Ok(resp);
        }


    }
}
