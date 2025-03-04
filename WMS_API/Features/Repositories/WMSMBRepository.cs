using Core.DTOs.MB;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class WMSMBRepository : IWMSMBRespository
    {
        private readonly string _connectionString;
        public WMSMBRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzas");
        }        

        public async Task<IM_WMS_MB_InsertBox> getInsertBox(string Orden, int Caja, string Ubicacion, int Consolidado,string usuarioRecepcion,string Camion)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Orden",Orden),
                new SqlParameter("@Caja",Caja),
                new SqlParameter("@Ubicacion",Ubicacion),
                new SqlParameter("@Consolidado",Consolidado),
                new SqlParameter("@UsuarioRecepcion",usuarioRecepcion),
                new SqlParameter("@Camion",Camion)
            };

            IM_WMS_MB_InsertBox result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_MB_InsertBox>("[IM_WMS_MB_InsertBox]", parametros);

            return result;
        }
        public async Task<List<IM_WMS_MB_InsertBox>> getCajasEscaneadasRack(string ubicacion)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@ubicacion",ubicacion)
            };

            List<IM_WMS_MB_InsertBox> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_MB_InsertBox>("[IM_WMS_MB_ObtenerCajasEscaneadasRack]", parametros);
            return result;



        }

        public async Task<string> postEnviarCorreoRecepcion(List<IM_WMS_MB_InsertBox> data)
        {
            DateTime date = DateTime.Now;
            string fecha = date.Day + "-" + date.Month + "-" + date.Year;

            data = data.FindAll(x => x.FechaRecepcion.Day + "-" + x.FechaRecepcion.Month + "-" + x.FechaRecepcion.Year == fecha).ToList();

            try
            {
                Byte[] fileContents;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Hoja1");
                    worksheet.Cells[1, 1].Value = "Lote";
                    worksheet.Cells[1, 2].Value = "OP";
                    worksheet.Cells[1, 3].Value = "Articulo";
                    worksheet.Cells[1, 4].Value = "NumeroCaja";
                    worksheet.Cells[1, 5].Value = "Talla";
                    worksheet.Cells[1, 6].Value = "Cantidad";
                    worksheet.Cells[1, 7].Value = "FechaEnvio";
                    worksheet.Cells[1, 8].Value = "FechaRecepcion";
                    worksheet.Cells[1, 9].Value = "Color";
                    worksheet.Cells[1, 10].Value = "Ubicacion";

                    int fila = 2;
                    data.ForEach(x =>
                    {
                        worksheet.Cells[fila, 1].Value = x.Lote;
                        worksheet.Cells[fila, 2].Value = x.Orden;
                        worksheet.Cells[fila, 3].Value = x.Articulo;
                        worksheet.Cells[fila, 4].Value = x.NumeroCaja;
                        worksheet.Cells[fila, 5].Value = x.Talla;
                        worksheet.Cells[fila, 6].Value = x.Cantidad;
                        worksheet.Cells[fila, 8].Value = x.FechaRecepcion;
                        worksheet.Cells[fila, 9].Value = x.Color;
                        worksheet.Cells[fila, 10].Value = x.UbicacionRecepcion;
                        fila++;

                    });
                    var rangeTable = worksheet.Cells[1, 1, fila, 10];
                    var table = worksheet.Tables.Add(rangeTable, "MyTable");
                    table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;

                    //sacar resumen
                    var listagrupada = data
                        .GroupBy(caja => caja.Lote)
                        .Select(grupo => new IM_WMS_MB_InsertBox
                        {
                            Lote = grupo.Key,
                            Cantidad = grupo.Sum(caja => caja.Cantidad)
                        }).ToList();

                    ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add("Hoja2");
                    worksheet2.Cells[1, 1].Value = "Lote";
                    worksheet2.Cells[1, 2].Value = "Cantidad";
                    int fila2 = 2;
                    listagrupada.ForEach(x =>
                    {
                        worksheet2.Cells[fila2, 1].Value = x.Lote;
                        worksheet2.Cells[fila2, 2].Value = x.Cantidad;
                        fila2++;
                    });

                    var rangeTable2 = worksheet2.Cells[1, 1, fila2, 2];
                    var table2 = worksheet2.Tables.Add(rangeTable2, "MyTable2");
                    table2.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;

                    //resumen tallas
                    var pivoteSheet = package.Workbook.Worksheets.Add("Hoja3");
                    var pivotTable = pivoteSheet.PivotTables.Add(pivoteSheet.Cells[1, 1], rangeTable, "PivotTable");

                    //fila
                    pivotTable.RowFields.Add(pivotTable.Fields["Lote"]);

                    //Columna
                    pivotTable.ColumnFields.Add(pivotTable.Fields["Talla"]);

                    //valores                
                    var cantidadField = pivotTable.DataFields.Add(pivotTable.Fields["Cantidad"]);
                    cantidadField.Function = DataFieldFunctions.Sum;


                    fileContents = package.GetAsByteArray();
                    try
                    {
                        MailMessage mail = new MailMessage();

                        mail.From = new MailAddress(VariablesGlobales.Correo);

                        /*var correos = await getCorreosRecepcionUbicacionCajas();

                        correos.ForEach(x =>
                        {
                            mail.To.Add(x.Correo);

                        });*/
                        mail.To.Add("bavila@intermoda.com.hn");


                        TimeSpan tiempo = data[0].FechaRecepcion - data[data.Count - 1].FechaRecepcion;


                        mail.Subject = "Recepcion Producto Terminado MB " + fecha;
                        mail.IsBodyHtml = true;

                        mail.Body = "<p>Recepcion Producto Terminado MB" + " Camion: " + data[0].Camion + " Usuario: " + data[0].usuarioRecepcion + ", Descargado en " + tiempo.Hours + " horas y " + tiempo.Minutes + " Minutos " + tiempo.Seconds + " Segundos</p><h2>Traslados:</h2>";

                        using (MemoryStream ms = new MemoryStream(fileContents))
                        {

                            Attachment attachment = new Attachment(ms, "Recepcion Cajas Bodega " + fecha + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                            mail.Attachments.Add(attachment);

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
                    }
                    catch (Exception err)
                    {
                        return err.ToString();
                    }
                }
            } catch (Exception err)
            {
                return err.ToString();
            }
            return "OK";

        }
    }
}
