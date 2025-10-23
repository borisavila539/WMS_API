using Core.DTOs.CAEX.Guia;
using Core.DTOs.MB;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WMSMBController : ControllerBase
    {
        private readonly IWMSMBRespository _WMSMB;

        public WMSMBController(IWMSMBRespository WMSMB)
        {
            _WMSMB = WMSMB;
        }

        [HttpGet("InsertUpdateBox/{Orden}/{Caja}/{Ubicacion}/{Consolidado}/{usuarioRecepcion}/{Camion}")]
        public async Task<ActionResult<IM_WMS_MB_InsertBox>> GetInsertBox(string Orden, int Caja, string Ubicacion, int Consolidado, string usuarioRecepcion, string Camion)
        {
            var resp = await _WMSMB.getInsertBox(Orden, Caja, Ubicacion, Consolidado, usuarioRecepcion, Camion);
            return resp;
        }

        [HttpGet("ObtenerCajasRack/{ubicacion}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_MB_InsertBox>>> getCajasEscaneadasRack(string ubicacion)
        {
            var resp = await _WMSMB.getCajasEscaneadasRack(ubicacion);
            return resp;
        }
        [HttpPost("EnviarCorreoRecepcion")]
        public async Task<string> postEnviarCorreoRecepcion(List<IM_WMS_MB_InsertBox> data)
        {
            var resp = await _WMSMB.postEnviarCorreoRecepcion(data);
            return resp;
        }

        //cajas disponibles para generar el Despacho
        [HttpPost("CajasDisponibles")]
        public async Task<ActionResult<IEnumerable<IM_WMS_MB_CajasDisponibles2>>> GetCajasDisponibles(FiltroCajasDisponiblesMB filtro)
        {
            var resp = await _WMSMB.GetCajasDisponibles(filtro);
            return resp;
        }

        //descargar excel con cajas Disponibles
        [HttpGet("DescargarCajasDisponibles")]
        public async Task<IActionResult> getDescargarDisponibles()
        {
            FiltroCajasDisponiblesMB Filtro = new FiltroCajasDisponiblesMB();
            Filtro.Articulo = "";
            Filtro.Color = "";
            Filtro.Orden = "";
            Filtro.Page = 0;
            Filtro.Size = 0;
            var data = await _WMSMB.GetCajasDisponiblesTodo(Filtro);

            Byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Hoja1");
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "IDConsolidado";
                worksheet.Cells[1, 3].Value = "Lote";
                worksheet.Cells[1, 4].Value = "Orden";
                worksheet.Cells[1, 5].Value = "Articulo";
                worksheet.Cells[1, 6].Value = "DescripcioMB";
                worksheet.Cells[1, 7].Value = "NumeroCaja";
                worksheet.Cells[1, 8].Value = "Talla";
                worksheet.Cells[1, 9].Value = "Color";
                worksheet.Cells[1, 10].Value = "NombreColor";
                worksheet.Cells[1, 11].Value = "Cantidad";



                int fila = 2;

                foreach (var element in data)
                {
                    worksheet.Cells[fila, 1].Value = element.ID;
                    worksheet.Cells[fila, 2].Value = element.IDConsolidado;
                    worksheet.Cells[fila, 3].Value = element.Lote;
                    worksheet.Cells[fila, 4].Value = element.Orden;
                    worksheet.Cells[fila, 5].Value = element.Articulo;
                    worksheet.Cells[fila, 6].Value = element.DescripcioMB;
                    worksheet.Cells[fila, 7].Value = element.NumeroCaja;
                    worksheet.Cells[fila, 8].Value = element.Talla;
                    worksheet.Cells[fila, 9].Value = element.Color;
                    worksheet.Cells[fila, 10].Value = element.NombreColor;
                    worksheet.Cells[fila, 11].Value = element.Cantidad;
                    fila++;
                }
                fila--;
                var rangeTable = worksheet.Cells[1, 1, fila, 11];
                rangeTable.AutoFitColumns();
                var table = worksheet.Tables.Add(rangeTable, "MyTable");
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;


                fileContents = package.GetAsByteArray();
            }
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CajasDisponibles.xlsx");

        }

        [HttpPost("SubirArchivo")]
        public async Task<string> subirArchivo(IFormFile file)
        {
            //limpiar los que ya estan
            FiltroCajasDisponiblesMB Filtro = new FiltroCajasDisponiblesMB();
            Filtro.Articulo = "";
            Filtro.Color = "";
            Filtro.Orden = "";
            Filtro.Page = 0;
            Filtro.Size = 0;
            var data = await _WMSMB.GetCajasDisponiblesTodo(Filtro);

            foreach (var item in data)
            {
                await _WMSMB.getActualizarCajasParaDespacho(item.ID, false);
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        int id = Convert.ToInt32(worksheet.Cells[row, 1].Text);
                        await _WMSMB.getActualizarCajasParaDespacho(id, true);
                    }
                }
            }

            return "OK";
        }

        [HttpGet("ActualizarCajasParaDespacho/{id}/{PickToDespacho}")]
        public async Task<ActionResult<IM_WMS_MB_CajasDisponibles>> getActualizarCajasParaDespacho(int id, bool PickToDespacho)
        {
            var resp = await _WMSMB.getActualizarCajasParaDespacho(id, PickToDespacho);
            return resp;
        }
        [HttpGet("ResumenArticulosSeleccionados")]
        public async Task<ActionResult<IEnumerable<IM_WMS_MB_ResumenArticulosSeleccionados>>> GetResumenArticulosSeleccionados()
        {
            var resp = await _WMSMB.GetResumenArticulosSeleccionados();
            return resp;

        }

        [HttpGet("DescargarResumen")]
        public async Task<IActionResult> getDescargarResumen()
        {
            var data = await _WMSMB.GetResumenArticulosSeleccionados();
            Byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Hoja1");
                worksheet.Cells[1, 1].Value = "Articulo";
                worksheet.Cells[1, 2].Value = "DescripcionMB";
                worksheet.Cells[1, 3].Value = "Talla";
                worksheet.Cells[1, 4].Value = "Color";
                worksheet.Cells[1, 5].Value = "QTYTotal";
                worksheet.Cells[1, 6].Value = "QTYCajas";

                int fila = 2;

                foreach (var element in data)
                {
                    worksheet.Cells[fila, 1].Value = element.Articulo;
                    worksheet.Cells[fila, 2].Value = element.descripcioMB;
                    worksheet.Cells[fila, 3].Value = element.Talla;
                    worksheet.Cells[fila, 4].Value = element.Color;
                    worksheet.Cells[fila, 5].Value = element.QTYTotal;
                    worksheet.Cells[fila, 6].Value = element.QTYCajas;
                    fila++;
                }
                fila--;
                var rangeTable = worksheet.Cells[1, 1, fila, 6];
                rangeTable.AutoFitColumns();
                var table = worksheet.Tables.Add(rangeTable, "MyTable");
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;


                //resumen tallas
                var pivoteSheet = package.Workbook.Worksheets.Add("Hoja2");
                var pivotTable = pivoteSheet.PivotTables.Add(pivoteSheet.Cells[1, 1], rangeTable, "PivotTable");

                //fila
                var rowField1 = pivotTable.RowFields.Add(pivotTable.Fields["Articulo"]);
                rowField1.Compact = false;
                rowField1.Outline = false;
                rowField1.ShowAll = false;
                rowField1.SubTotalFunctions = eSubTotalFunctions.None;

                var rowField2 = pivotTable.RowFields.Add(pivotTable.Fields["DescripcionMB"]);
                rowField2.Compact = false;
                rowField2.Outline = false;
                rowField2.ShowAll = false;
                rowField2.SubTotalFunctions = eSubTotalFunctions.None;

                //Columna
                pivotTable.ColumnFields.Add(pivotTable.Fields["Talla"]);
                pivotTable.ColumnHeaderCaption = "Talla";
                //valores                
                var cantidadField = pivotTable.DataFields.Add(pivotTable.Fields["QTYTotal"]);
                cantidadField.Function = DataFieldFunctions.Sum;


                // Configurar tabla en **formato tabular**
                pivotTable.Compact = false;
                pivotTable.Outline = false;
                pivotTable.GridDropZones = false;
                pivotTable.RowHeaderCaption = "Articulo";

                // Ocultar subtotales generales
                pivotTable.RowGrandTotals = true;
                pivotTable.ColumnGrandTotals = true;


                fileContents = package.GetAsByteArray();
            }
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Vista Pedido a Generar MB.xlsx");

        }

        [HttpGet("GenerarDespacho/{usuario}")]
        public async Task<ActionResult<IM_WMS_MB_CrearDespacho>> getGenerarDespacho(string usuario)
        {
            var data = await _WMSMB.GetResumenArticulosSeleccionados();
            int unidades = 0;
            int cajas = 0;
            Byte[] fileContents;
            Byte[] fileTemplate;
            

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Hoja1");
                worksheet.Cells[1, 1].Value = "Articulo";
                worksheet.Cells[1, 2].Value = "DescripcionMB";
                worksheet.Cells[1, 3].Value = "Talla";
                worksheet.Cells[1, 4].Value = "Color";
                worksheet.Cells[1, 5].Value = "QTYTotal";
                worksheet.Cells[1, 6].Value = "QTYCajas";
              
                int fila = 2;

                foreach (var element in data)
                {
                    worksheet.Cells[fila, 1].Value = element.Articulo;
                    worksheet.Cells[fila, 2].Value = element.descripcioMB;
                    worksheet.Cells[fila, 3].Value = element.Talla;
                    worksheet.Cells[fila, 4].Value = element.Color;
                    worksheet.Cells[fila, 5].Value = $"{element.NombreColor}({element.Color})";
                    worksheet.Cells[fila, 5].Value = element.QTYTotal;
                    worksheet.Cells[fila, 6].Value = element.QTYCajas;

                    unidades += element.QTYTotal;
                    cajas += element.QTYCajas;
                    fila++;
                }
                fila--;
                var rangeTable = worksheet.Cells[1, 1, fila, 6];
                rangeTable.AutoFitColumns();
                var table = worksheet.Tables.Add(rangeTable, "MyTable");
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;


                //resumen tallas
                var pivoteSheet = package.Workbook.Worksheets.Add("Hoja2");
                var pivotTable = pivoteSheet.PivotTables.Add(pivoteSheet.Cells[1, 1], rangeTable, "PivotTable");

                //fila
                var rowField1 = pivotTable.RowFields.Add(pivotTable.Fields["Articulo"]);
                rowField1.Compact = false;
                rowField1.Outline = false;
                rowField1.ShowAll = false;
                rowField1.SubTotalFunctions = eSubTotalFunctions.None;

                var rowField2 = pivotTable.RowFields.Add(pivotTable.Fields["DescripcionMB"]);
                rowField2.Compact = false;
                rowField2.Outline = false;
                rowField2.ShowAll = false;
                rowField2.SubTotalFunctions = eSubTotalFunctions.None;

                //Columna
                pivotTable.ColumnFields.Add(pivotTable.Fields["Talla"]);
                pivotTable.ColumnHeaderCaption = "Talla";
                //valores                
                var cantidadField = pivotTable.DataFields.Add(pivotTable.Fields["QTYTotal"]);
                cantidadField.Function = DataFieldFunctions.Sum;


                // Configurar tabla en **formato tabular**
                pivotTable.Compact = false;
                pivotTable.Outline = false;
                pivotTable.GridDropZones = false;
                pivotTable.RowHeaderCaption = "Articulo";

                // Ocultar subtotales generales
                pivotTable.RowGrandTotals = true;
                pivotTable.ColumnGrandTotals = true;


                fileContents = package.GetAsByteArray();
            }

            using (ExcelPackage packageSimple = new ExcelPackage())
            {
                ExcelWorksheet worksheet = packageSimple.Workbook.Worksheets.Add("Articulos");

                worksheet.Cells[1, 1].Value = "Articulo";
                worksheet.Cells[1, 2].Value = "Talla";
                worksheet.Cells[1, 3].Value = "Color";
                worksheet.Cells[1, 4].Value = "Cantidad";

                int fila = 2;

                foreach (var element in data)
                {
                    worksheet.Cells[fila, 1].Value = element.Articulo;
                    worksheet.Cells[fila, 2].Value = element.Talla;
                    worksheet.Cells[fila, 3].Value = element.Color;
                    worksheet.Cells[fila, 4].Value = element.QTYTotal;
                    fila++;
                }

                var rangeTable = worksheet.Cells[1, 1, fila - 1, 4];
                rangeTable.AutoFitColumns();
                var table = worksheet.Tables.Add(rangeTable, "TablaArticulos");
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;

                fileTemplate = packageSimple.GetAsByteArray();
            }

            var resp = await _WMSMB.getGenerarDespacho(usuario);

            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(VariablesGlobales.Correo);

                var correos = await _WMSMB.getCorreosDespachoMB();

                correos.ForEach(x =>
                {
                    mail.To.Add(x.Correo);

                });
                //mail.To.Add("bavila@intermoda.com.hn");


                mail.Subject = "Desapcho MB " + resp.ID;
                mail.IsBodyHtml = true;

                mail.Body = "<p>Buen dia,</p>";
                mail.Body += "<p>Adjunto lista de empaque del despacho #" + resp.ID + " de producto MB.</p>";
                mail.Body += "<p>Cajas: " + cajas + ", Unidades: " + unidades + "</p>";

                mail.Body += "<p>Saludos</p>";



                using  (MemoryStream pl=new MemoryStream(fileTemplate)) 
                using (MemoryStream ms = new MemoryStream(fileContents))
                {

                    Attachment attachment = new Attachment(ms, "Desapcho MB " + resp.ID + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    Attachment attachmentPantilla= new Attachment(pl, "Pantilla Pedidos " + resp.ID + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    mail.Attachments.Add(attachment);
                    mail.Attachments.Add(attachmentPantilla);


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
                return resp;
            }
            catch (Exception err)
            {
                return null;
            }


        }

        [HttpGet("DespachosPendientes")]
        public async Task<ActionResult<IEnumerable<IM_WMS_MB_CrearDespacho>>> getDespachosPendientes()
        {
            var resp = await _WMSMB.getDespachosPendientes();
            return resp;
        }

        [HttpGet("DespachoPicking/{DespachoID}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_MB_PICKING>>> getPicking(int DespachoID)
        {
            var resp = await _WMSMB.getPicking(DespachoID);
            return resp;
        }

        [HttpGet("DespachoUpdatePicking/{id}/{usuario}")]
        public async Task<ActionResult<IM_WMS_MB_PICKING>> getUpdatePicking(int id, string usuario)
        {
            var resp = await _WMSMB.getUpdatePicking(id, usuario);
            return resp;
        }

        [HttpGet("DespachoPacking/{DespachoID}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_MB_PACKING>>> getPacking(int DespachoID)
        {
            var resp = await _WMSMB.getPacking(DespachoID);
            return resp;
        }

        [HttpGet("DespachoUpdatePacking/{id}/{usuario}/{pallet}")]
        public async Task<ActionResult<IM_WMS_MB_PACKING>> getUpdatePicking(int id, string usuario, string pallet)
        {
            var resp = await _WMSMB.getUpdatePacking(id, usuario, pallet);
            return resp;
        }
        [HttpGet("Tracking/{id}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_MB_Tracking>>> getTracking(int id)
        {
            var resp = await _WMSMB.getTracking(id);
            return resp;
        }

        [HttpGet("TrackingPallet/{id}")]
        public async Task<ActionResult<IEnumerable<IM_WMS_MB_Trackingpallet>>> getTrackingPallet(int id)
        {
            var resp = await _WMSMB.getTrackingPallet(id);
            return resp;
        }

        [HttpGet("DescargarResumenDespachoPallet/{id}")]
        public async Task<IActionResult> getDescargarResumenDespachoPallet(int id)
        {
            var data = await _WMSMB.GetResumenDespachoPallets(id);
            Byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Hoja1");
                int fila = 1;
                worksheet.Cells[fila, 1].Value = "ID";
                worksheet.Cells[fila, 2].Value = "IDConsolidado";
                worksheet.Cells[fila, 3].Value = "NumeroCaja";
                worksheet.Cells[fila, 4].Value = "Lote";
                worksheet.Cells[fila, 5].Value = "Orden";
                worksheet.Cells[fila, 6].Value = "Articulo";
                worksheet.Cells[fila, 7].Value = "talla";
                worksheet.Cells[fila, 8].Value = "Color";
                worksheet.Cells[fila, 9].Value = "Cantidad";
                worksheet.Cells[fila, 10].Value = "UbicacionRecepcion";
                worksheet.Cells[fila, 11].Value = "Picking";
                worksheet.Cells[fila, 12].Value = "Packing";
                worksheet.Cells[fila, 13].Value = "Pallet";

                fila++;

                foreach (var element in data)
                {
                    worksheet.Cells[fila, 1].Value = element.ID;
                    worksheet.Cells[fila, 2].Value = element.IDConsolidado;
                    worksheet.Cells[fila, 3].Value = element.NumeroCaja;
                    worksheet.Cells[fila, 4].Value = element.Lote;
                    worksheet.Cells[fila, 5].Value = element.Orden;
                    worksheet.Cells[fila, 6].Value = element.Articulo;
                    worksheet.Cells[fila, 7].Value = element.talla;
                    worksheet.Cells[fila, 8].Value = element.Color;
                    worksheet.Cells[fila, 9].Value = element.Cantidad;
                    worksheet.Cells[fila, 10].Value = element.UbicacionRecepcion;
                    worksheet.Cells[fila, 11].Value = element.Picking;
                    worksheet.Cells[fila, 12].Value = element.Packing;
                    worksheet.Cells[fila, 13].Value = element.Pallet;
                    fila++;
                }
                fila--;
                var rangeTable = worksheet.Cells[1, 1, fila, 13];
                rangeTable.AutoFitColumns();
                var table = worksheet.Tables.Add(rangeTable, "MyTable");
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;

                fileContents = package.GetAsByteArray();
            }
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Despacho MB " + id + ".xlsx");

        }

        [HttpGet("GetEtiquetaDespacho/{workOrderId}/{boxNum}")]
        public async Task<ActionResult<IM_WMS_MB_ReimpresionEtiqueta>> GetEtiquetaDespacho(string workOrderId, string boxNum)
        {
            var resultado = await _WMSMB.GetEtiquetaDespacho(workOrderId, boxNum);
            if (resultado == null) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("ReimpirmirEtiquetaDespachoMB/{impresora}")]
        public async Task<ActionResult> ReimprimirEtiquetaD([FromBody] IM_WMS_MB_ReimpresionEtiqueta iM_WMS_MB_ReimpresionEtiqueta, string impresora)
        {
            var resultado = await _WMSMB.ImprimirEtiquetaDespachoNormal(iM_WMS_MB_ReimpresionEtiqueta,impresora);
            impresora = resultado.ToString();
            return Ok(resultado);
        }

        [HttpGet("ValidarAcceso/{codigoUsuario}")]
        public async Task<ActionResult> ValidarAccesoPorPantalla(string codigoUsuario)
        {
            var resultado = await _WMSMB.ValidarAccesoAPantlla(codigoUsuario);
            
            return Ok(resultado);
            
        }
        
    }
}
