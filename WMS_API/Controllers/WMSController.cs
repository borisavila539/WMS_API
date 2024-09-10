using Core.DTOs;
using Core.DTOs.BusquedaRolloAX;
using Core.DTOs.ControCajasEtiquetado;
using Core.DTOs.DeclaracionEnvio;
using Core.DTOs.Despacho_PT;
using Core.DTOs.Despacho_PT.Liquidacion;
using Core.DTOs.DiarioTransferir;
using Core.DTOs.GeneracionPrecios;
using Core.DTOs.InventarioCiclicoTela;
using Core.DTOs.RecepcionUbicacionCajas;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
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
        public async Task<IM_WMS_InventarioCilicoTelaDiario> getInventarioCiclicoTelaExist(string JournalID, string InventSerialID, string User,decimal QTY)
        {
            var resp = await _WMS.GetInventarioCilicoTelaDiario(JournalID,InventSerialID,User,QTY);
            return resp;
        }

        [HttpGet("AgregarInventarioCilicoTela/{JournalID}/{InventSerialID}/{ubicacion}/{QTY}")]
        public async Task<IM_WMS_InventarioCilicoTelaDiario> Get_AgregarInventarioCilicoTelaDiario(string JournalID, string InventSerialID, string ubicacion, decimal QTY)
        {
            var resp = await _WMS.Get_AgregarInventarioCilicoTelaDiario(JournalID, InventSerialID, ubicacion, QTY);
            return resp;
        }
        [HttpGet("EnviarInventarioCilcicoTela/{JournalID}")]
        public async Task<string> getEnviarInventarioCiclicoTela( string JournalID)
        {
            var Detalle = await _WMS.Get_InventarioCilicoTelaDiarios(JournalID);

            List<INVENTARIOCICLICOTELALINE> lines = new List<INVENTARIOCICLICOTELALINE>();

            Detalle = Detalle.FindAll(x => x.Exist);

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
                }else if (element.New)
                {
                    line.PROCESO = "ADD";
                }
                lines.Add(line);
            });

            var resp = _AX.InsertAddInventarioCiclicoTelaLine(lines);

            //enviar correo
            string []texto = resp.Split(";");
            string htmlCorreo = "";

            htmlCorreo += @"<h1>"+JournalID+@"</h1>";
            htmlCorreo += @"<p>" + texto[1] + @"</p>";
            htmlCorreo += @"<p>" + texto[2] + @"</p>";
            htmlCorreo += @"<p>" + texto[3] + @"</p>";


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
        public async Task<SP_GetBoxesReceived> GetBoxesReceived(string opBoxNum,string ubicacion,string Tipo)
        {
            var resp = await _WMS.getBoxesReceived(opBoxNum, ubicacion,Tipo);
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
        public async Task<IM_WMS_Insert_Control_Cajas_Etiquetado> GetControl_Cajas_Etiquetado(string caja,string empleado)
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
        public async Task<IEnumerable<IM_WMS_ObtenerDetalleGeneracionPrecios>> GetObtenerDetalleGeneracionPrecios(string pedido,string empresa)
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
            worksheet.Cells[fila, 13].Value = "Costo;";
            worksheet.Cells[fila, 14].Value = "Departamento";
            worksheet.Cells[fila, 15].Value = "SubCategoria";
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
                worksheet.Cells[fila, 14].Value = element.Departamento;
                worksheet.Cells[fila, 15].Value = element.SubCategoria;
                fila++;
            });
            fila--;
            var rangeTable = worksheet.Cells[1, 1, fila, 15];
            var table = worksheet.Tables.Add(rangeTable, "MyTable");
            table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;

            var excelBytes = package.GetAsByteArray();

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla.xlsx");

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

    }
}
