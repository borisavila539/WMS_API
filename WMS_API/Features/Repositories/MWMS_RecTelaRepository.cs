using Core.DTOs.IM_WMS_RecTela;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;
using Core.Interfaces;
using OfficeOpenXml;
using System.Net.Mail;
using System.IO;
using System.Text;
using System.Net;

namespace WMS_API.Features.Repositories
{
    public class MWMS_RecTelaRepository: IMWMS_RecTelaRepository
    {

        private readonly string _connectionString;
        
        public MWMS_RecTelaRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzasDev");
        }

        public async Task<List<IM_WMS_RecTela_GetListTelasDTO>> GetListTelas(string journalId)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@journalName","REC_TELAS"),
                new SqlParameter("@journalId",journalId)
            };

            var result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_RecTela_GetListTelasDTO>("[IM_WMS_RecTela_GetListTelas]", parametros);


            foreach (var item in result)
            {
                var journalScanCount = await TelaJournalScanCounts(item.JournalId);

                item.JournalScanCounts = journalScanCount;
            }

            return result;
        }


        public async Task<List<IM_WMS_RecTela_TelaJournalScanCountsDTO>> TelaJournalScanCounts(string journalId)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@journalId",journalId)
            };

            var result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_RecTela_TelaJournalScanCountsDTO>("[IM_WMS_RecTela_TelaJournalScanCounts]", parametros);

            return result;
        }

        public async Task<List<IM_WMS_RecTela_DatosRollosProveedorDTO>> DatosRollosProveedor(string journalId)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@journalId",journalId)
            };

            var result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_RecTela_DatosRollosProveedorDTO>("[IM_WMS_RecTela_DatosRollosProveedor]", parametros);

            return result;
        }

        public async Task<List<IM_WMS_RecTela_PostTelaPickingMergeDTO>> PostTelaPickingMergeDTO(string journalId)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@journalId",journalId)
            };

            List<IM_WMS_RecTela_PostTelaPickingMergeDTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_RecTela_PostTelaPickingMergeDTO>("[IM_WMS_RecTela_PostTelaPickingMerge]", parametros);

            return result;
        }


        public async Task<List<IM_WMS_RecTela_UpdateTelaPickingIsScanningDTO>> UpdateTelaPickingIsScanning(List<UpdateTelaPickingIsScanningDto> telapicking)
        {
            var response = new List<IM_WMS_RecTela_UpdateTelaPickingIsScanningDTO>();
            for (int i = 0; i < telapicking.Count; i++)
            {

                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

                var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@userCode",telapicking[i].userCode),
                    new SqlParameter("@vendroll", telapicking[i].vendroll),
                    new SqlParameter("@journalid", telapicking[i].journalId),
                    new SqlParameter("@inventSerialId",telapicking[i].inventSerialId),
                    new SqlParameter("@location",telapicking[i].location),
                    new SqlParameter("@telaPickingDefectoId",telapicking[i].TelaPickingDefectoId)
                };

                IM_WMS_RecTela_UpdateTelaPickingIsScanningDTO result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_RecTela_UpdateTelaPickingIsScanningDTO>("[IM_WMS_RecTela_UpdateTelaPickingIsScanning]", parametros);
                response.Add(result);
            }

            return response;
        }


        public async Task<List<IM_WMS_RecTela_GetTelaPickingDefectoDTO>> GetTelaPickingDefecto()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>();

            List<IM_WMS_RecTela_GetTelaPickingDefectoDTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_RecTela_GetTelaPickingDefectoDTO>("[IM_WMS_RecTela_GetTelaPickingDefecto]", parametros);

            return result;
        }

        public async Task<List<IM_WMS_RecTela_CorreosActivosDTO>> GetCorreosActivos()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>();

            List<IM_WMS_RecTela_CorreosActivosDTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_RecTela_CorreosActivosDTO>("[IM_WMS_RecTela_CorreosActivos]", parametros);

            return result;
        }


        public async Task<List<IM_WMS_RecTela_GetTelaPickingRuleDTO>> GetTelaPickingRule()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>();

            List<IM_WMS_RecTela_GetTelaPickingRuleDTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_RecTela_GetTelaPickingRuleDTO>("[IM_WMS_RecTela_GetTelaPickingRule]", parametros);

            return result;
        }

        public async Task<string> EnviarCorreoDeRecepcionDeTela(string journalId)
        {
            var data = await PostTelaPickingMergeDTO(journalId);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                // Hoja principal con toda la información
                var worksheet = package.Workbook.Worksheets.Add("RecepcionTela");
                var worksheetNoEscaneadas = package.Workbook.Worksheets.Add("NoEscaneadas");
                var worksheetDefectos = package.Workbook.Worksheets.Add("ConDefecto");

                // Encabezado
                string[] headers = { "Diario", "Importación", "Artículo", "Referencia", "Color", "Número de rollo", "Número de rollo proveedor", "Qty", "Ubicación", "Defecto", "Escaneado", "Fecha de actualización", "Escaneado por" };


                foreach (var ws in new[] { worksheet, worksheetNoEscaneadas, worksheetDefectos })
                {
                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cells[1, i + 1].Value = headers[i];
                        ws.Cells[1, i + 1].Style.Font.Bold = true;
                    }
                }

                // Insertar datos
                int filaGeneral = 2, filaNoEscaneadas = 2, filaDefectos = 2;
                foreach (var item in data)
                {
                    var row = new object[]
                    {
                        item.JournalId, 
                        item.InventBatchId,
                        item.ItemId,
                        item.Reference, 
                        item.NameColor + "(" + item.InventColorId + ")", 
                        item.InventSerialId, 
                        item.VendRoll, 
                        Math.Round(item.Qty, 2),
                        item.Location,
                        item.DescriptionDefecto, 
                        item.is_scanning ? "Sí" : "No",
                        item.update_date.ToString("yyyy-MM-dd HH:mm:ss"), 
                        item.User
                    };

                    worksheet.Cells[filaGeneral, 1, filaGeneral, headers.Length].LoadFromArrays(new[] { row });
                    filaGeneral++;

                    if (!item.is_scanning)
                    {
                        worksheetNoEscaneadas.Cells[filaNoEscaneadas, 1, filaNoEscaneadas, headers.Length].LoadFromArrays(new[] { row });
                        filaNoEscaneadas++;
                    }

                    if (!string.IsNullOrEmpty(item.DescriptionDefecto))
                    {
                        worksheetDefectos.Cells[filaDefectos, 1, filaDefectos, headers.Length].LoadFromArrays(new[] { row });
                        filaDefectos++;
                    }
                }


                foreach (var ws in new[] { worksheet, worksheetNoEscaneadas, worksheetDefectos })
                {
                    var range = ws.Cells[1, 1, ws.Dimension.End.Row, headers.Length];
                    var table = ws.Tables.Add(range, ws.Name + "Table");
                    table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;
                }

                worksheet.Cells.AutoFitColumns();
                worksheetNoEscaneadas.Cells.AutoFitColumns();
                worksheetDefectos.Cells.AutoFitColumns();

                Byte[] fileContents = package.GetAsByteArray();
                try
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(VariablesGlobales.Correo);

                    var correos = await GetCorreosActivos();
                    var rollosProveedor = await DatosRollosProveedor(journalId);

                    foreach (IM_WMS_RecTela_CorreosActivosDTO correo in correos)
                    {
                        mail.To.Add(correo.Correo);
                    }

                    mail.Subject = "Recepción de tela " + journalId;
                    mail.IsBodyHtml = true;

                    StringBuilder htmlTable = new StringBuilder();
                    htmlTable.Append("<h2>Reporte de Recepción de Tela</h2>");
                    htmlTable.Append("<table border='1' style='border-collapse:collapse; width:100%; text-align:center;'>");
                    htmlTable.Append("<tr>");
                    htmlTable.Append("<th>Articulo</th>");
                    htmlTable.Append("<th>Importación</ th>");
                    htmlTable.Append("<th>Cantidad de rollo</th>");
                    htmlTable.Append("<th>Nombre proveedor</th>");
                    htmlTable.Append("</tr>");

                    foreach (var item in rollosProveedor)
                    {
                        htmlTable.Append("<tr>");
                        htmlTable.AppendFormat("<td>{0}</td>", item.ItemId);
                        htmlTable.AppendFormat("<td>{0}</td>", item.InventBatchId);
                        htmlTable.AppendFormat("<td>{0}</td>", item.CantidadDeRollos);
                        htmlTable.AppendFormat("<td>{0}</td>", item.NombreProveedor);
                        htmlTable.Append("</tr>");
                    }

                    htmlTable.Append("</table>");

                    mail.Body = htmlTable.ToString();

                    using (MemoryStream ms = new MemoryStream(fileContents))
                    {
                        string fileName = "RecepcionDeTela_" + journalId + ".xlsx";
                        Attachment attachment = new Attachment(ms, fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                        mail.Attachments.Add(attachment);

                        SmtpClient oSmtpClient = new SmtpClient("smtp.office365.com", 587)
                        {
                            EnableSsl = true,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(VariablesGlobales.Correo, VariablesGlobales.Correo_Password)
                        };

                        oSmtpClient.Send(mail);
                        oSmtpClient.Dispose();
                    }
                    return "ok";
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
