﻿using Core.DTOs;
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
    }
}
