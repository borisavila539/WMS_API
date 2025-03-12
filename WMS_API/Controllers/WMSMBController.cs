using Core.DTOs.MB;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WMSMBController:ControllerBase
    {
        private readonly IWMSMBRespository _WMSMB;

        public WMSMBController(IWMSMBRespository WMSMB)
        {
            _WMSMB = WMSMB;
        }

        [HttpGet("InsertUpdateBox/{Orden}/{Caja}/{Ubicacion}/{Consolidado}/{usuarioRecepcion}/{Camion}")]
        public async Task<ActionResult<IM_WMS_MB_InsertBox>> GetInsertBox(string Orden, int Caja, string Ubicacion, int Consolidado, string usuarioRecepcion, string Camion)
        {
            var resp = await _WMSMB.getInsertBox(Orden, Caja, Ubicacion, Consolidado,usuarioRecepcion,Camion);
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
        public async Task<ActionResult<IEnumerable<IM_WMS_MB_CajasDisponibles>>> GetCajasDisponibles(FiltroCajasDisponiblesMB filtro)
        {
            var resp = await _WMSMB.GetCajasDisponibles(filtro);
            return resp;
        }

        [HttpGet("ActualizarCajasParaDespacho/{id}/{PickToDespacho}")]
        public async Task<ActionResult<IM_WMS_MB_CajasDisponibles>> getActualizarCajasParaDespacho(int id, bool PickToDespacho)
        {
            var resp = await _WMSMB.getActualizarCajasParaDespacho(id, PickToDespacho);
            return resp;
        }
        [HttpGet("ResumenArticulosSeleccionados")]
        public async Task<ActionResult<IEnumerable< IM_WMS_MB_ResumenArticulosSeleccionados>>> GetResumenArticulosSeleccionados()
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

        [HttpGet("GernerarDespacho/{usuario}")]
        public async Task<ActionResult<IM_WMS_MB_CrearDespacho>> getGenerarDespacho(string usuario)
        {
            var resp = await _WMSMB.getGenerarDespacho(usuario);
            return resp;
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
        public async Task<ActionResult<IM_WMS_MB_PACKING>> getUpdatePicking(int id, string usuario,string pallet)
        {
            var resp = await _WMSMB.getUpdatePacking(id, usuario,pallet);
            return resp;
        }

    }
}
