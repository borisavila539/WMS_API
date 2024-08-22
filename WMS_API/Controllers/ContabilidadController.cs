using Core.DTOs.Contabilidad;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;

namespace WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContabilidadController : ControllerBase
    {
        [HttpGet("Proveedores")]
        public async Task<List<ReporteProveedoreDTO>> getReporte()
        {
            var Lista = new List<ReporteProveedoreDTO>();
            Boolean next = true;
            Boolean FechaVencimiento = false;
            string ruta = @"C:\Users\bavila\Downloads\Antiguedad de proveedores.xlsx";
            try
            {
                ExcelWorksheet worksheet;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using(ExcelPackage package = new ExcelPackage(ruta))
                {
                    worksheet = package.Workbook.Worksheets[0];
                    var fila = 1;
                    var tmp = new ReporteProveedoreDTO();

                    do
                    {
                        string cellvalue = "";
                        try
                        {
                            cellvalue = (string)worksheet.Cells[fila, 3].Value;
                        }
                        catch (Exception ex)
                        {

                        }
                        if(FechaVencimiento && cellvalue != "Total")
                        {
                            var cell = (string)worksheet.Cells[fila, 4].Value;

                            var valor = (string)worksheet.Cells[fila, 5].Value;
                            valor =valor.Replace(".", "");
                            valor =valor.Replace(",", ".");

                            if (cell.StartsWith("AA-"))
                            {
                                tmp.Anticipos += Convert.ToDecimal(valor);
                            }
                            else
                            {
                                tmp.SaldoReal += Convert.ToDecimal(valor);
                            }
                        }
                        else if(cellvalue == "Fecha de vencimiento")
                        {
                            FechaVencimiento = true;
                        }
                        else if (cellvalue != null && cellvalue.StartsWith("PRV-"))
                        {
                            tmp.Proveedor = cellvalue;
                            tmp.Nombre = (string)worksheet.Cells[fila, 4].Value;
                            tmp.Grupo = (string)worksheet.Cells[fila, 5].Value;
                            var t = (double)worksheet.Cells[fila + 3, 5].Value;
                            DateTime fechaBase = new DateTime(1899, 12, 30);

                            tmp.Fecha = fechaBase.AddDays(t);
                            tmp.SaldoReal = 0;
                            tmp.Anticipos = 0;
                        }
                        else if(cellvalue == "Total")
                        {
                           
                            Lista.Add(tmp);
                            tmp = new ReporteProveedoreDTO();
                            FechaVencimiento = false;
                        }
                        else if (cellvalue == "Total general")
                        {
                            next = false;
                        }
                        fila++;
                    } while (next);
                }
            }
            catch (Exception ex)
            {

            }

            return Lista;

        }
    }
}
