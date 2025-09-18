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
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WMSController : ControllerBase
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
        public string GetInsertMovimiento(string JOURNALID, string IMBOXCODE, string ITEMBARCODE, string PROCESO)
        {
            var resp = _AX.InsertDeleteMovimientoLine(JOURNALID, ITEMBARCODE, PROCESO, IMBOXCODE);
            return resp;
        }

        [HttpGet("EnviarRecibirtraslado/{TRANSFERID}/{ESTADO}")]
        public string getEnviarRecibirTraslado(string TRANSFERID, string ESTADO)
        {
            var resp = _AX.EnviarRecibirTraslados(TRANSFERID, ESTADO);
            return resp;
        }
        [HttpGet("InsertDeleteRecuccionCajas/{IMBOXCODE}/{ITEMBARCODE}/{PROCESO}")]
        public string GetinsertDeleteReduccionCajas(string IMBOXCODE, string ITEMBARCODE, string PROCESO)
        {
            var resp = _AX.INsertDeleteReduccionCajas(ITEMBARCODE, PROCESO, IMBOXCODE);
            return resp;
        }

        [HttpGet("ImprimirEtiquetaMovimiento/{JOURNALID}/{IMBOXCODE}/{PRINT}")]
        public Task<string> GetImprimirEtiquetaMovimiento(string JOURNALID, string IMBOXCODE, string PRINT)
        {
            var resp = _WMS.GetImprimirEtiquetaMovimiento(JOURNALID, IMBOXCODE, PRINT);
            return resp;
        }

        [HttpGet("Impresoras")]
        public async Task<ActionResult<IEnumerable<ImpresoraDTO>>> GetImpresoras()
        {
            var resp = await _WMS.getImpresoras();
            return resp;
        }

        [HttpGet("DespachotelasDetalle/{TRANSFERIDFROM}/{TRANSFERIDTO}/{INVENTLOCATIONIDTO}/{TIPO}")]
        public async Task<ActionResult<IEnumerable<DespachoTelasDetalleDTO>>> GetDespachotelasDetalle(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO, string TIPO)
        {
            var resp = await _WMS.GetDespacho_Telas(TRANSFERIDFROM, TRANSFERIDTO, INVENTLOCATIONIDTO, TIPO);
            return resp;
        }

        [HttpGet("DespachoTelaPickingPacking/{INVENTSERIALID}/{TIPO}/{CAMION}/{CHOFER}/{InventTransID}/{USER}/{ID}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_Despacho_Tela_Detalle_Rollo>>> GetDespachoTelaPickingPacking(string INVENTSERIALID, string TIPO, string CAMION, string CHOFER, string InventTransID, string USER, int ID)
        {
            var resp = await _WMS.GetDespacho_Tela_Picking_Packing(INVENTSERIALID, TIPO, CAMION, CHOFER, InventTransID, USER, ID);
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
            var resp = await _WMS.getEstadotraslados(TRANSFERIDFROM, TRANSFERIDTO, INVENTLOCATIONIDTO);
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
        public async Task<string> getNotaDespacho(int DESPACHOID, string RECID, string EMPLEADO, string CAMION)
        {
            var resp = await _WMS.getNotaDespacho(DESPACHOID, RECID, EMPLEADO, CAMION);
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
        public Task<string> getImprimirEtiquetaReduccion(string IMBOXCODE, string UBICACION, string USER, string PRINT)
        {
            var resp = _WMS.getImprimirEtiquetaReduccion(IMBOXCODE, UBICACION, USER, PRINT);
            return resp;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginDTO>> PostLogin(LoginDTO datos)
        {
            var resp = await _WMS.PostLogin(datos);

            return Ok(resp);
        }

        [HttpPost("ImprimirEtiquetaRollo")]
        public Task<string> postImprimirEtiquetaRollo(List<EtiquetaRolloDTO> data)
        {
            var resp = _WMS.postImprimirEtiquetaRollo(data);
            return resp;
        }

        //Despacho PT

        [HttpGet("InsertBoxesDespachoPT/{ProdID}/{UserCreated}/{Box}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_Insert_Boxes_Despacho_PT_DTO>>> GetInsert_Boxes_Despacho_PT(string ProdID, string UserCreated, int Box)
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
        public async Task<ActionResult<IM_WMS_Crear_Despacho_PT>> getCrear_Despacho_PT(string driver, string truck, string userCreated, int almacen)
        {
            var resp = await _WMS.GetCrear_Despacho_PT(driver, truck, userCreated, almacen);
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
        public async Task<ActionResult<IM_WMS_EnviarDespacho>> Get_EnviarDespachos(int DespachoID, string user, int cajasSegundas, int cajasTerceras)
        {
            var resp = await _WMS.Get_EnviarDespachos(DespachoID, user, cajasSegundas, cajasTerceras);
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
            var resp = await _WMS.getDetalleAuditoriaCaja(ProdID, Box);
            return resp;
        }

        [HttpGet("InsertAuditoriaCajaTP/{ProdID}/{Box}/{IDUnico}/{QTY}")]
        public async Task<IM_WMS_insertDetalleAdutoriaDenim> getDetalleAuditoriaCaja(string ProdID, int Box, int IDUnico, int QTY)
        {
            var resp = await _WMS.getInsertAuditoriaCajaTP(ProdID, Box, IDUnico, QTY);
            return resp;
        }

        [HttpGet("EnviarAuditoriaTP/{DespachoID}/{usuario}")]
        public async Task<string> getDetalleAuditoriaCaja(int DespachoID, string usuario)
        {
            var resp = await _WMS.getEnviarCorreoAuditoriaTP(DespachoID, usuario);
            return resp;
        }


        //Consulta OP


        [HttpGet("DespachosPTEstado/{DespachoID}")]
        public async Task<IEnumerable<IM_WMS_Get_Despachos_PT_DTO>> getDespachosPTEstado(int DespachoID)
        {
            var resp = await _WMS.getDespachosPTEstado(DespachoID);
            return resp;
        }

        [HttpGet("DespachosPTConsultaOrdenes/{ProdCutSheetID}/{DespachoID}")]
        public async Task<IEnumerable<IM_WMS_ConsultaOP_OrdenesDTO>> getConsultaOpOrdenes(string ProdCutSheetID, int DespachoID)
        {
            var resp = await _WMS.getConsultaOpOrdenes(ProdCutSheetID, DespachoID);
            return resp;
        }
        //Consulta OP Detalle

        [HttpGet("ConsultaOPDetalle/{ProdCutSheetID}")]
        public async Task<IEnumerable<IM_WMS_Consulta_OP_DetalleDTO>> getConsultaOPDetalle(string ProdCutSheetID)
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

        [HttpGet("TransferirMovimiento/{JOURNALID}/{ITEMBARCODE}/{PROCESO}/{BoxCode}")]
        public async Task<string> GetTransferirMovimientoLine(string JOURNALID, string ITEMBARCODE, string PROCESO, string BoxCode)
        {
            var resp = _AX.InsertDeleteTransferirMovimientoLine(JOURNALID, ITEMBARCODE, PROCESO);
            if (resp == "OK")
            {
                await _WMS.GetInsertTransferirCajaDetalle(JOURNALID, ITEMBARCODE, BoxCode, PROCESO);
            }

            return resp;


        }

        [HttpGet("EnviarCorreotransferir/{JOURNALID}/{USERID}")]
        public Task<IM_WMS_EnviarDiarioTransferirDTO> GetEnviarCorreotransferir(string JOURNALID, string USERID)
        {
            var resp = _WMS.getEnviarDiarioTransferir(JOURNALID, USERID);
            return resp;
        }

        [HttpGet("DiariosTransferirAbiertos/{user}/{filtro}")]
        public async Task<ActionResult<IEnumerable<DiariosAbiertosDTO>>> GetDiariosTransferirAbiertos(string user, string filtro)
        {
            var resp = await _WMS.getObtenerDiarioTransferir(user, filtro);
            return Ok(resp);
        }

        [HttpGet("LineasDiarioTransferir/{diario}")]
        public async Task<ActionResult<IEnumerable<LineasDTO>>> GetLienasDiarioTransferir(string diario)
        {
            var resp = await _WMS.GetLineasDiarioTransferir(diario);
            return Ok(resp);
        }

        [HttpGet("ImprimirEtiquetaTransferir/{JOURNALID}/{IMBOXCODE}/{PRINT}")]
        public Task<string> GetImprimirEtiquetaTransferir(string JOURNALID, string IMBOXCODE, string PRINT)
        {
            var resp = _WMS.GetImprimirEtiquetaTransferir(JOURNALID, IMBOXCODE, PRINT);
            return resp;
        }

        //busqueda de rollos en ax
        [HttpGet("BusquedaRollosAX/{INVENTLOCATIONID}/{INVENTSERIALID}/{INVENTBATCHID}/{INVENTCOLORID}/{WMSLOCATIONID}/{REFERENCE}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_BusquedaRollosAXDTO>>> getBusquedaRolloAX(string INVENTLOCATIONID, string INVENTSERIALID, string INVENTBATCHID, string INVENTCOLORID, string WMSLOCATIONID, string REFERENCE)
        {
            var resp = await _WMS.GetBusquedaRollosAX(INVENTLOCATIONID, INVENTSERIALID, INVENTBATCHID, INVENTCOLORID, WMSLOCATIONID, REFERENCE);
            return resp;
        }

        //Liquidacion de la orden
        //Despachos Recibidos
        [HttpGet("DespachoRecibidoLiquidacion/{DespachoID}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_DespachosRecibidosLiquidacionDTO>>> getDespachosRecibidosLiquidacion(int DespachoID)
        {
            var resp = await _WMS.GetDespachosRecibidosLiquidacion(DespachoID);
            return resp;
        }

        //ordenes de los despachos recibidos
        [HttpGet("OrdenesRecibidasDespachoLiquidacion/{DespachoID}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_DespachoPT_OrdenesRecibidasDepachoDTO>>> GetOrdenesRecibidasDepacho(int DespachoID)
        {
            var resp = await _WMS.GetOrdenesRecibidasDepacho(DespachoID);
            return resp;
        }

        //obtener detalle de las ordenes recibidas por despacho
        [HttpGet("DetalleOrdenesRecibidasliquidacion/{DespachoID}/{ProdCutSheetID}")]
        public async Task<IEnumerable<IM_WMS_DespachoPT_DetalleOrdenRecibidaLiquidacionDTO>> GetDetalleOrdenRecibidaLiquidacion(int DespachoID, string ProdCutSheetID)
        {
            var resp = await _WMS.GetDetalleOrdenRecibidaLiquidacion(DespachoID, ProdCutSheetID);
            return resp;

        }

        //inventario ciclico de telas
        [HttpGet("InventarioCiclicoTelasDiariosAbiertos")]
        public async Task<IEnumerable<IM_WMS_InventarioCiclicoTelasDiariosAbiertos>> GetInventarioCiclicoTelasDiariosAbiertos()
        {
            var resp = await _WMS.GetInventarioCiclicoTelasDiariosAbiertos();
            return resp;
        }
        [HttpGet("InventarioCilicoTelaDiario/{JournalID}")]
        public async Task<IEnumerable<IM_WMS_InventarioCilicoTelaDiario>> GetInventarioCilicoTelaDiarios(string JournalID)
        {
            var resp = await _WMS.Get_InventarioCilicoTelaDiarios(JournalID);
            return resp;
        }

        [HttpGet("InventarioCiclicoTelaExist/{JournalID}/{InventSerialID}/{User}/{QTY}")]
        public async Task<IM_WMS_InventarioCilicoTelaDiario> getInventarioCiclicoTelaExist(string JournalID, string InventSerialID, string User, decimal QTY)
        {
            var resp = await _WMS.GetInventarioCilicoTelaDiario(JournalID, InventSerialID, User, QTY);
            return resp;
        }

        [HttpGet("AgregarInventarioCilicoTela/{JournalID}/{InventSerialID}/{ubicacion}/{QTY}")]
        public async Task<IM_WMS_InventarioCilicoTelaDiario> Get_AgregarInventarioCilicoTelaDiario(string JournalID, string InventSerialID, string ubicacion, decimal QTY)
        {
            var resp = await _WMS.Get_AgregarInventarioCilicoTelaDiario(JournalID, InventSerialID, ubicacion, QTY);
            return resp;
        }
        [HttpGet("EnviarInventarioCilcicoTela/{JournalID}")]
        public async Task<string> getEnviarInventarioCiclicoTela(string JournalID)
        {
            var Detalle2 = await _WMS.Get_InventarioCilicoTelaDiarios(JournalID);

            List<INVENTARIOCICLICOTELALINE> lines = new List<INVENTARIOCICLICOTELALINE>();

            var Detalle = Detalle2.FindAll(x => x.Exist);

            Detalle.ForEach(element =>
            {
                INVENTARIOCICLICOTELALINE line = new INVENTARIOCICLICOTELALINE();
                line.JOURNALID = element.JournalID;
                line.INVENTSERIALID = element.InventSerialID;
                line.WMSLOCATIONID = element.WMSLocationID;
                line.INVENTLOCATIONID = element.InventLocationID;
                line.QTY = element.InventOnHand.ToString();
                if (element.Exist && !element.New)
                {
                    line.PROCESO = "UPDATE";
                }
                else if (element.New)
                {
                    line.PROCESO = "ADD";
                }
                lines.Add(line);
            });

            var resp = _AX.InsertAddInventarioCiclicoTelaLine(lines);

            //enviar correo
            string[] texto = resp.Split(";");
            string htmlCorreo = "";

            htmlCorreo += @"<h1>" + JournalID + @"</h1>";
            try
            {
                htmlCorreo += @"<p> Rollos de Tela encontrados: " + Detalle2.Count(x=> x.New == false && x.Exist == true) + "/"+ Detalle2.Count(x => x.New == false) +@"</p>";
                htmlCorreo += @"<p>Rollos agregados al conteo: " + Detalle2.Count(x => x.New == true) + @"</p>";
                if(Detalle2.Count(x => x.Exist == false) > 0)
                {
                    htmlCorreo += @"<h3>Rollos de Tela no encontrados</h3>";
                    htmlCorreo += @"<ol>";

                    Detalle2.FindAll(x => x.Exist == false).ForEach(ele =>
                     {
                         htmlCorreo += @"<li>"+ele.InventSerialID+@"</li>";
                     });
                    htmlCorreo += @"</ol>";
                }

            }
            catch (Exception err)
            {
                htmlCorreo += @"<p>" + resp + @"</p>";
            }


            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(VariablesGlobales.Correo);

                var correos = await _WMS.getCorreoCiclicoTela();


                foreach (IM_WMS_Correos_DespachoPTDTO correo in correos)
                {
                    mail.To.Add(correo.Correo);
                }
                mail.Subject = "Inventario Cilico Tela, Diario: " + JournalID;
                mail.IsBodyHtml = true;

                mail.Body = htmlCorreo;

                SmtpClient oSmtpClient = new SmtpClient();

                oSmtpClient.Host = "smtp.office365.com";
                oSmtpClient.Port = 587;
                oSmtpClient.EnableSsl = true;
                oSmtpClient.UseDefaultCredentials = false;

                NetworkCredential userCredential = new NetworkCredential(VariablesGlobales.Correo, VariablesGlobales.Correo_Password);

                oSmtpClient.Credentials = userCredential;

                oSmtpClient.Send(mail);
                oSmtpClient.Dispose();


            }
            catch (Exception err)
            {
                string error = err.ToString();
            }


            return resp;
        }

        [HttpGet("ActuallizarQTYCiclicoTela/{InventserialID}/{QTY}")]
        public async Task<IM_WMS_InventarioCilicoTelaDiario> UpdateQTYInventSerialID(string InventserialID, decimal QTY)
        {
            var resp = await _WMS.UpdateQTYInventSerialID(InventserialID, QTY);
            return resp;
        }

        //recepcion y ubicacion de cajas
        [HttpGet("RecepcionUbicacionCajas/{opBoxNum}/{ubicacion}/{Tipo}")]
        public async Task<SP_GetBoxesReceived> GetBoxesReceived(string opBoxNum, string ubicacion, string Tipo)
        {
            var resp = await _WMS.getBoxesReceived(opBoxNum, ubicacion, Tipo);
            return resp;
        }

        [HttpPost("RecepcionUbicacionCajas")]
        public async Task<IEnumerable<SP_GetAllBoxesReceived>> GetAllBoxesReceived(Filtros filtro)
        {
            var resp = await _WMS.getAllBoxesReceived(filtro);
            return resp;
        }

        [HttpPost("RecepcionUbicacionCajasCorreo")]
        public async Task<string> postEnviarRecepcionUbicacionCajas(List<Ubicaciones> data)
        {
            var resp = await _WMS.postEnviarRecepcionUbicacionCajas(data);
            return resp;
        }

        [HttpPost("ResumenCajasUnidadesTP")]
        public async Task<IEnumerable<IM_WMS_TP_DetalleCajasResumen>> getResumenCajasUnidadesTP(Filtros filtro)
        {
            var resp = await _WMS.getResumenCajasUnidadesTP(filtro);
            return resp;
        }
        [HttpGet("RecepcionUbicacionCajasSync/{TIPO}")]
        public async Task<IEnumerable<SP_GetAllBoxesReceived>> GetAllBoxesReceived(string TIPO)
        {
            var resp = await _WMS.getAllBoxesReceived(TIPO);
            return resp;
        }

        //Declaracion de envio
        [HttpGet("DeclaracionEnvio/{opBoxNum}/{ubicacion}")]
        public async Task<SP_GetBoxesReceived> GetBoxesReceived(string opBoxNum, string ubicacion)
        {
            var resp = await _WMS.getBoxesReserved(opBoxNum, ubicacion);
            return resp;
        }

        [HttpPost("DeclaracionEnvio")]
        public async Task<IEnumerable<SP_GetAllBoxesReserved_V2>> GetAllBoxesReserved_V2(FiltroDeclaracionEnvio filtro)
        {
            var resp = await _WMS.GetAllBoxesReserved_V2(filtro);
            return resp;
        }
        [HttpGet("DeclaracionEnvioSync")]
        public async Task<IEnumerable<SP_GetAllBoxesReserved_V2>> GetAllBoxesReserved_V2()
        {
            var resp = await _WMS.GetAllBoxesReserved();
            return resp;
        }

        [HttpGet("DeclaracionEnvio/{pais}/{ubicacion}/{fecha}")]
        public async Task<IActionResult> GetDeclaracionEnvioDownload(string pais, string ubicacion, string fecha)
        {
            var data = await _WMS.GetDeclaracionEnvio(pais, ubicacion, fecha);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Hoja1");
            int fila = 5;
            //agregar Encabezados
            worksheet.Cells[4, 1].Value = "Departamento";

            data.ForEach(element =>
            {
                worksheet.Cells[fila, 1].Value = element.Departamento;
                fila++;
            });

            var excelBytes = package.GetAsByteArray();

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Declaracion Envio.xlsx");

        }


        //control cajas etiquetado
        [HttpGet("ControlCajasEtiquetadoAgregar/{caja}/{empleado}")]
        public async Task<IM_WMS_Insert_Control_Cajas_Etiquetado> GetControl_Cajas_Etiquetado(string caja, string empleado)
        {
            var resp = await _WMS.GetControl_Cajas_Etiquetado(caja, empleado);
            return resp;
        }

        [HttpPost("ControlCajasEtiquetado")]
        public async Task<IEnumerable<IM_WMS_Control_Cajas_Etiquetado_Detalle>> Get_Control_Cajas_Etiquetado_Detalles(IM_WMS_Control_Cajas_Etiquetado_Detalle_Filtro filtro)
        {
            var resp = await _WMS.Get_Control_Cajas_Etiquetado_Detalles(filtro);
            return resp;
        }

        //generaicon de precios y codigos
        [HttpGet("ObtenerDetalleGeneracionPrecios/{pedido}/{empresa}")]
        public async Task<IEnumerable<IM_WMS_ObtenerDetalleGeneracionPrecios>> GetObtenerDetalleGeneracionPrecios(string pedido, string empresa)
        {
            var resp = await _WMS.GetObtenerDetalleGeneracionPrecios(pedido, empresa);
            return resp;
        }

        [HttpGet("DescargarDetalleGeneracionPrecios/{pedido}/{empresa}")]
        public async Task<IActionResult> GetDescargarDetalleGeneracionPrecios(string pedido, string empresa)
        {
            var data = await _WMS.GetObtenerDetalleGeneracionPrecios(pedido, empresa);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Hoja1");
            int fila = 1;
            //agregar Encabezados
            worksheet.Cells[fila, 1].Value = "CodigoBarra";
            worksheet.Cells[fila, 2].Value = "Articulo";
            worksheet.Cells[fila, 3].Value = "Base";
            worksheet.Cells[fila, 4].Value = "Estilo";
            worksheet.Cells[fila, 5].Value = "IDColor";
            worksheet.Cells[fila, 6].Value = "Referencia";
            worksheet.Cells[fila, 7].Value = "Descripcion";
            worksheet.Cells[fila, 8].Value = "ColorDescripcion";
            worksheet.Cells[fila, 9].Value = "Talla";
            worksheet.Cells[fila, 10].Value = "Descripcion2";
            worksheet.Cells[fila, 11].Value = "Categoria";
            worksheet.Cells[fila, 12].Value = "Cantidad";
            worksheet.Cells[fila, 13].Value = "Costo";
            worksheet.Cells[fila, 14].Value = "Precio";
            worksheet.Cells[fila, 15].Value = "Departamento";
            worksheet.Cells[fila, 16].Value = "SubCategoria";
            worksheet.Cells[fila, 17].Value = "Pedido";
            worksheet.Cells[fila, 18].Value = "Tienda";

            fila++;

            data.ForEach(element =>
            {
                worksheet.Cells[fila, 1].Value = element.CodigoBarra;
                worksheet.Cells[fila, 2].Value = element.Articulo;
                worksheet.Cells[fila, 3].Value = element.Base;
                worksheet.Cells[fila, 4].Value = element.Estilo;
                worksheet.Cells[fila, 5].Value = element.IDColor;
                worksheet.Cells[fila, 6].Value = element.Referencia;
                worksheet.Cells[fila, 7].Value = element.Descripcion;
                worksheet.Cells[fila, 8].Value = element.ColorDescripcion;
                worksheet.Cells[fila, 9].Value = element.Talla;
                worksheet.Cells[fila, 10].Value = element.Descripcion2;
                worksheet.Cells[fila, 11].Value = element.Categoria;
                worksheet.Cells[fila, 12].Value = element.Cantidad;
                worksheet.Cells[fila, 13].Value = element.Costo;
                worksheet.Cells[fila, 14].Value = element.Precio;
                worksheet.Cells[fila, 15].Value = element.Departamento;
                worksheet.Cells[fila, 16].Value = element.SubCategoria;
                worksheet.Cells[fila, 17].Value = element.Pedido;
                worksheet.Cells[fila, 18].Value = element.DeliveryName;

                fila++;
            });
            fila--;
            var rangeTable = worksheet.Cells[1, 1, fila, 18];
            var table = worksheet.Tables.Add(rangeTable, "MyTable");
            table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;

            var excelBytes = package.GetAsByteArray();

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla.xlsx");

        }

        [HttpGet("DescargarConfiguracionGeneracionPrecios/{pedido}/{empresa}")]
        public async Task<IActionResult> GetDescargarConfiguracionGeneracionPrecios(string pedido, string empresa)
        {
            List<IM_WMS_ObtenerDetalleGeneracionPrecios> data = await _WMS.GetObtenerDetalleGeneracionPrecios(pedido, empresa);

            List<IM_WMS_ObtenerPreciosCodigos> lista = data.GroupBy(g => new { g.CuentaCliente, g.Base, g.IDColor, g.Costo, g.Precio })
                .Select(grupo => new IM_WMS_ObtenerPreciosCodigos
                {
                    ID = 0,
                    CuentaCliente = grupo.Key.CuentaCliente,
                    Base = grupo.Key.Base,
                    IDColor = grupo.Key.IDColor,
                    Costo = grupo.Key.Costo,
                    Precio = grupo.Key.Precio

                }).ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Hoja1");
            int fila = 1;
            //agregar Encabezados
            worksheet.Cells[fila, 1].Value = "CuentaCliente";
            worksheet.Cells[fila, 2].Value = "Base";
            worksheet.Cells[fila, 3].Value = "IDColor";
            worksheet.Cells[fila, 4].Value = "Costo";
            worksheet.Cells[fila, 5].Value = "Precio";
            fila++;

            lista.ForEach(element =>
            {
                worksheet.Cells[fila, 1].Value = element.CuentaCliente;
                worksheet.Cells[fila, 2].Value = element.Base;
                worksheet.Cells[fila, 3].Value = element.IDColor;
                worksheet.Cells[fila, 4].Value = element.Costo;
                worksheet.Cells[fila, 5].Value = element.Precio;
                fila++;
            });
            fila--;
            var rangeTable = worksheet.Cells[1, 1, fila, 5];
            var table = worksheet.Tables.Add(rangeTable, "MyTable");
            table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;

            var excelBytes = package.GetAsByteArray();

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PlantillaConfiguracionPrecio.xlsx");

        }

        [HttpGet("ObtenerPreciosCodigos/{cuentaCliente}")]
        public async Task<IEnumerable<IM_WMS_ObtenerPreciosCodigos>> GetObtenerPreciosCodigos(string cuentaCliente)
        {
            var resp = await _WMS.GetObtenerPreciosCodigos(cuentaCliente);
            return resp;
        }

        [HttpPost("ObtenerPreciosCodigos")]
        public async Task<IM_WMS_ObtenerPreciosCodigos> postInsertUpdatePrecioCodigos(IM_WMS_ObtenerPreciosCodigos data)
        {
            var resp = await _WMS.postInsertUpdatePrecioCodigos(data);
            return resp;
        }
        [HttpPost("UploadExcelPreciosCodigos")]
        public async Task<string> UploadExcel(IFormFile file)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Primer hoja del archivo Excel
                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;


                        // Recorre las filas y columnas
                        for (int row = 2; row <= rowCount; row++) // Empieza en la fila 2, omitiendo el encabezado
                        {

                            var configuracion = new IM_WMS_ObtenerPreciosCodigos
                            {
                                CuentaCliente = worksheet.Cells[row, 1].Text,
                                Base = worksheet.Cells[row, 2].Text,
                                IDColor = worksheet.Cells[row, 3].Text,
                                Costo = Convert.ToDecimal(worksheet.Cells[row, 4].Text),
                                Precio = Convert.ToDecimal(worksheet.Cells[row, 5].Text),
                            };

                            await this.postInsertUpdatePrecioCodigos(configuracion);

                        }
                        // Lógica adicional para guardar los datos o procesarlos
                        // return Ok(data); // Si quieres devolver los datos
                    }
                }
            }
            catch (Exception err)
            {
                return err.ToString();
            }

            return "OK";

        }

        [HttpGet("GetPrecioCodigos")]
        public async Task<IEnumerable<IM_WMS_DetalleImpresionEtiquetasPrecio>> getDatosPrecioCodigos([FromQuery] ImpresionEtiqueta impresionEtiquetaParm)
        {
            var data = await _WMS.GetDetalleImpresionEtiquetasPrecio(impresionEtiquetaParm);
            return data;
        }

        /*[HttpGet("ImpresionPrecioCodigos/{pedido}/{ruta}/{caja}/{fecha}/{impresora}")]
        public async Task<string> getimpresionPrecioCodigos(string pedido, string ruta, string caja, string fecha, string impresora)
        {
            string resp = "";
            string resp2 = "";

            var data = await _WMS.GetDetalleImpresionEtiquetasPrecio(pedido, ruta, caja);
            var tmp = data.FindAll(x => x.Precio == 0);
            string cajas = "";
            if (tmp.Count() != 0)
            {
                return "Existen articulos sin precio";
            }
            else
            {
                data.ForEach(element =>
                {
                    if (cajas == "" || cajas != element.IMIB_BOXCODE)
                    {
                        cajas = element.IMIB_BOXCODE;
                        resp2 = _WMS.imprimirEtiquetaCajaDividir(element.IMIB_BOXCODE, impresora);
                        if (resp2 != "OK")
                        {
                            resp += "Fallo Impresion :" + resp2 + ",";

                        }

                    }

                    int multiplo = element.QTY / 3;
                    int restante = element.QTY - multiplo * 3;
                    if (multiplo > 0)
                    {
                        resp2 = _WMS.imprimirEtiquetaprecios(element, multiplo, 0, fecha, impresora);
                        if (resp2 != "OK")
                        {
                            resp += "Fallo Impresion :" + resp2 + ",";

                        }

                    }
                    if (restante > 0)
                    {
                        resp2 = _WMS.imprimirEtiquetaprecios(element, 0, restante, fecha, impresora);
                        if (resp2 != "OK")
                        {
                            resp += "Fallo Impresion :" + resp2 + ",";
                        }

                    }
                });
            }

            if (resp.Length > 0)
            {
                return resp;
            }

            return "OK";

        }*/

        //version 2 de impresion de etiquetas
        [HttpGet("ImpresionPrecioCodigos")]
        public async Task<string> getimpresionPrecioCodigos2([FromQuery] ImpresionEtiqueta param)
        {
            string resp = "";
            string resp2 = "";
            bool hayCantidad = param.CantidadImprimir != null && param.CantidadImprimir != "" && param.CantidadImprimir != "0";
            param.Normalizar(param);
            var data = await _WMS.GetDetalleImpresionEtiquetasPrecio(param);

            if(hayCantidad && data.Count == 1)
            {
                data[0].QTY = Convert.ToInt32(param.CantidadImprimir);
            }

            var tmp = data.FindAll(x => x.Precio == 0);
            string cajas = "";
            if (tmp.Count() != 0)
            {
                return "Existen articulos sin precio";
            }
            else
            {
                
                List<IM_WMS_DetalleImpresionEtiquetasPrecio> listado = new List<IM_WMS_DetalleImpresionEtiquetasPrecio>();
                data.ForEach(element =>
                {
                    if (cajas == "" || cajas != element.IMIB_BOXCODE)
                    {
                        if(listado.Count > 0)
                        {
                            //imprimir
                            resp2 = _WMS.imprimirEtiquetaprecios2(listado,param.Fecha , param.Impresora);
                            if (resp2 != "OK")
                            {
                                resp += "Fallo Impresion :" + resp2 + ",";

                            }
                        }

                        cajas = element.IMIB_BOXCODE;
                        resp2 = _WMS.imprimirEtiquetaCajaDividir(element.IMIB_BOXCODE,param.Impresora);
                        if (resp2 != "OK")
                        {
                            resp += "Fallo Impresion :" + resp2 + ",";

                        }
                        listado = new List<IM_WMS_DetalleImpresionEtiquetasPrecio>() ;

                    }

                    for(int i  = 1; i <= element.QTY; i++)
                    {
                        listado.Add(element);
                    }                    
                });
                if (listado.Count > 0)
                {
                    //imprimir
                    resp2 = _WMS.imprimirEtiquetaprecios2(listado, param.Fecha, param.Impresora);
                    if (resp2 != "OK")
                    {
                        resp += "Fallo Impresion :" + resp2 + ",";

                    }
                }
            }
            

            if (resp.Length > 0)
            {
                return resp;
            }

            return "OK";

        }
        //version 2



        [HttpGet("ObtenerClientesGeneracionPrecio")]
        public async Task<IEnumerable<IM_WMS_ClientesGeneracionprecios>> GetClientesGeneracionprecios()
        {
            var resp = await _WMS.GetClientesGeneracionprecios();
            return resp;
        }

        [HttpPost("ClientesGeneracionPrecio")]
        public async Task<IM_WMS_ClientesGeneracionprecios> postClientesGeneracionprecios(IM_WMS_ClientesGeneracionprecios data)
        {
            var resp = await _WMS.postClienteGeneracionPrecio(data);
            return resp;
        }
        //Tracking pedido
        [HttpGet("EnviarCorreoTrackingPedidos/{fecha}")]
        public async Task<string> getEnviarCorreoTrackingPedidos(string fecha)
        {
            var resp = await _WMS.getEnviarCorreoTrackingPedidos(fecha);
            return resp;
        }
        [HttpPost("TrackingPedidos")]
        public async Task<IEnumerable<IM_WMS_GenerarDetalleFacturas>> postTrackingPedidos(TrackingPedidosFiltro filtro)
        {
            var resp = await _WMS.getObtenerDetalletrackingPedidos(filtro);
            return resp;

        }

        //Auditoria cajas denim
        [HttpGet("AuditoriaCajasDenim/{OP}/{Ubicacion}/{usuario}")]
        public async Task<IEnumerable<IM_WMS_ObtenerDetalleAdutoriaDenim>> GetObtenerDetalleAdutoriaDenim(string OP, string Ubicacion, string usuario)
        {
            var texto = OP.Split(",");
            var resp = await _WMS.Get_ObtenerDetalleAdutoriaDenims(OP != "-" ? texto[0] : "", OP != "-" ? Convert.ToInt32(texto[1]) : 0, Ubicacion, usuario);
            return resp;
        }

        [HttpGet("AuditoriaInsertCajasDenim/{ID}/{AuditoriaID}")]
        public async Task<IM_WMS_insertDetalleAdutoriaDenim> GetInsertDetalleAdutoriaDenim(int ID, int AuditoriaID)
        {

            var resp = await _WMS.GetInsertDetalleAdutoriaDenim(ID, AuditoriaID);
            return resp;
        }

        [HttpGet("EnviarAuditoriaInsertCajasDenim/{ubicacion}/{usuario}")]
        public async Task<string> GetInsertDetalleAdutoriaDenim(string ubicacion, string usuario)
        {
            var resp = await _WMS.getEnviarCorreoAuditoriaDenim(ubicacion, usuario);
            return resp;
        }

        //recicjale de cajas
        [HttpGet("ReciclajeCajas/{CentroCosto}/{QTY}/{DIARIO}/{CAMION}/{CHOFER}/{USUARIO}")]
        public string getInsetCajaReciclaje(string CentroCosto, string QTY, string DIARIO, string CAMION, string CHOFER, string USUARIO)
        {
            var resp = _AX.InsertCajasRecicladas(QTY, CentroCosto == "-" ? "" : CentroCosto, DIARIO == "-" ? "" : DIARIO);
            if (resp.StartsWith("DIA"))
            {
                _WMS.GetInsertCajasRecicladashistorico(CAMION, CHOFER, CentroCosto, Convert.ToInt32(QTY), USUARIO, resp);
            }
            return resp;
        }

        [HttpGet("ReciclajeCajasCentroCostos")]
        public async Task<IEnumerable<IM_WMS_CentroCostoReciclajeCajas>> GetCentroCostoReciclajeCajas()
        {
            var resp = await _WMS.GetCentroCostoReciclajeCajas();
            return resp;
        }
        [HttpGet("ReciclajeCajasPendientes")]
        public async Task<IEnumerable<IM_WMS_InsertCajasRecicladashistorico>> GetCajasRecicladasPendiente()
        {
            var resp = await _WMS.GetCajasRecicladasPendiente();
            return resp;
        }

        //Devoluciones

        [HttpGet("Devolucion/{Filtro}/{Page}/{Size}/{Estado}")]
        public async Task<IEnumerable<IM_WMS_Devolucion_Busqueda>> getDevolucionesEVA(string Filtro, int Page, int Size,int Estado)
        {
            var resp = await _WMS.getDevolucionesEVA(Filtro, Page, Size,Estado);
            return resp;
        }

        [HttpGet("DevolucionDetalle/{id}")]
        public async Task<IEnumerable<IM_WMS_Devolucion_Detalle_RecibirPlanta>> getDevolucionDetalle(int id)
        {
            var resp = await _WMS.getDevolucionDetalle(id);
            return resp;
        }
        [HttpGet("DevolucionDetalleQTY/{id}/{qty}/{tipo}")]
        public async Task<IEnumerable<IM_WMS_Devolucion_Detalle_RecibirPlanta>> getInsertDevolucionRecibidoEnviado(int id, int qty, string tipo)
        {
            var resp = await _WMS.getInsertDevolucionRecibidoEnviado(id,qty,tipo);
            return resp;
        }

        [HttpGet("Devolucion/Estado/{id}/{Estado}/{usuario}/{camion}")]
        public async Task<IM_WMS_Devolucion_Busqueda> getDevolucionesEVA(int id,string Estado,string usuario,string camion)
        {
            var resp = await _WMS.getActualizarEstadoDevolucion(id, Estado,usuario,camion);
            return resp;
        }

        [HttpGet("DevolucionDetalle/auditoria/{id}")]
        public async Task<IEnumerable<IM_WMS_Devolucion_Detalle_RecibirPlanta>> getDetalleDevolucionAuditoria(int id)
        {
            var resp = await _WMS.getDetalleDevolucionAuditoria(id);
            return resp;
        }

        [HttpGet("Devolucion/Defectos/{id}")]
        public async Task<IEnumerable<DefectosAuditoria>> GetObtenerDefectosDevolucions(int id)
        {
            var resp = await _WMS.GetObtenerDefectosDevolucions(id);
            return resp;
        }
        [HttpGet("Devolucion/DefectosDetalle/{id}/{idDefecto}/{tipo}/{Reparacion}/{operacion}/{qty}")]
        public async Task<IM_WMS_UpdateDetalleDefectoDevolucion> getActualizarDetalleDefectoDevolucion(int id, int idDefecto, string tipo,bool Reparacion, int operacion,int qty)
        {
            var resp = await _WMS.getActualizarDetalleDefectoDevolucion(id, idDefecto, tipo,Reparacion,operacion,qty);
            return resp;
        }
        //por si falla el anterior
        [HttpGet("Devolucion/DefectosDetalle/{id}/{idDefecto}/{tipo}/{Reparacion}/{operacion}")]
        public async Task<IM_WMS_UpdateDetalleDefectoDevolucion> getActualizarDetalleDefectoDevolucion(int id, int idDefecto, string tipo, bool Reparacion, int operacion)
        {
            var resp = await _WMS.getActualizarDetalleDefectoDevolucion(id, idDefecto, tipo, Reparacion, operacion, 1);
            return resp;
        }

        [HttpGet("Devolucion/Tracking/{Filtro}/{Page}/{Size}")]
        public async Task<IEnumerable<IM_WMS_Devolucion_Busqueda>> getObtenerDevolucionTracking(string Filtro, int Page, int Size)
        {
            var resp = await _WMS.getObtenerDevolucionTracking(Filtro, Page, Size);
            return resp;
        }
        
        [HttpGet("Devolucion/ImpresionEtiqueta/{id}/{NumDevolucion}/{CajaPrimera}/{CajaIrregular}/{usuario}")]
        public async Task<string> getImprimirEtiquetasDevolucion(int id, string NumDevolucion, int CajaPrimera, int CajaIrregular,string usuario)
        {
            var resp = await _WMS.getImprimirEtiquetasDevolucion(id, NumDevolucion, CajaPrimera, CajaIrregular,usuario);
            return resp;
        }
        
        [HttpGet("Devolucion/IngresoCajasPacking/{NumDevolucion}/{usuario}/{caja}")]
        public async Task<IM_WMS_CrearCajaDevolucion> getInsertarCajasDevolucion( string NumDevolucion, string usuario, int caja)
        {
            var resp = await _WMS.getInsertarCajasDevolucion( NumDevolucion, usuario, caja);
            return resp;
        }

        [HttpGet("Devolucion/packing")]
        public async Task<List<IM_WMS_DevolucionCajasPacking>> getDevolucionCajasPacking()
        {
            var resp = await _WMS.getDevolucionCajasPacking();
            return resp;
        }

        [HttpPost("Devolucion/EnviarCorreoPacking")]
        public async Task<string> postEnviarCorreoPackig(List<IM_WMS_Devolucion_Busqueda> data)
        {
            var resp = await _WMS.postEnviarCorreoPackig(data);
            return resp;
        }

        [HttpGet("Devolucion/EnviadasCD")]
        public async Task<List<IM_WMS_DevolucionCajasPacking>> getDevolucionCajasEnviadasCD()
        {
            var resp = await _WMS.getDevolucionCajasEnviadasCD();
            return resp;
        }

        [HttpGet("Devolucion/IngresoCajasRecibir/{NumDevolucion}/{usuario}/{caja}")]
        public async Task<IM_WMS_CrearCajaDevolucion> getInsertarCajasDevolucionRecibir(string NumDevolucion, string usuario, int caja)
        {
            var resp = await _WMS.getInsertarCajasDevolucionRecibir(NumDevolucion, usuario, caja);
            return resp;
        }

        [HttpGet("Devolucion/Consolidada")]
        public async Task<List<IM_WMS_Devolucion_Busqueda>> getDevolucionesConsolidar()
        {
            var resp = await _WMS.getDevolucionesConsolidar();
            return resp;
        }

        [HttpPost("Devolucion/ConsolidacionCajas")]
        public async Task<string> postDevolucionConsolidada(List<DevolucionConsolidada> data)
        {
            var resp = await _WMS.postDevolucionConsolidada(data);

            return resp;
        }

        [HttpGet("Devolucion/PackingRecibirCajaConsolidada/{id}/{usuario}/{tipo}")]
        public async Task<IEnumerable<IM_WMS_CrearCajaDevolucion>> getPackingRecibirCajaConsolidada(int id, string usuario, string tipo)
        {
            var resp = await _WMS.getPackingRecibirCajaConsolidada(id, usuario, tipo);
            return resp;
        }
        //otros
        //impresion de etiquetas todo diario de recuento telas
        [HttpGet("ImpresionDiarioRecuentoRollosPendientes/{journalID}")]
        public async Task<string> getInsertarCajasDevolucionRecibir(string journalID)
        {
            var resp = await _WMS.imprimirTodasEtiquetasPendientes(journalID);
            return resp;
        }
    }
    
}
