using Core.DTOs.InventarioMovimientos;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class IM_WMS_InventarioMovimientosRepository : IIM_WMS_InventarioMovimientosRepository
    {
        private readonly string _connectionString;

        public IM_WMS_InventarioMovimientosRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("IMFinanzas");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<List<MovimientoInventarioDto>> GetMovimientosInventario(DateTime fechaIni, DateTime fechaFin, string almacen, string articulo)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@FechaIni", fechaIni),
                new SqlParameter("@FechaFin", fechaFin),
                new SqlParameter("@Almacen", string.IsNullOrWhiteSpace(almacen) ? (object)DBNull.Value : almacen),
                new SqlParameter("@Articulo", string.IsNullOrWhiteSpace(articulo) ? (object)DBNull.Value : articulo)
            };

            try
            {
                return await executeProcedure.ExecuteStoredProcedureList<MovimientoInventarioDto>("[dbo].[SP_GetMovimientosInventario]", parametros);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<KardexInventarioDto>> GetKardexInventario(DateTime fechaIni, DateTime fechaFin, string almacen, string articulo, bool soloConMovimiento)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@FechaIni", fechaIni),
                new SqlParameter("@FechaFin", fechaFin),
                new SqlParameter("@Almacen", string.IsNullOrWhiteSpace(almacen) ? (object)DBNull.Value : almacen),
                new SqlParameter("@Articulo", string.IsNullOrWhiteSpace(articulo) ? (object)DBNull.Value : articulo),
                new SqlParameter("@SoloConMovimiento", soloConMovimiento)
            };

            try
            {
                return await executeProcedure.ExecuteStoredProcedureList<KardexInventarioDto>("[dbo].[SP_GetKardexInventario]", parametros);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CorreoReporteInventarioDto>> GetCorreosReporteInventario()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            try
            {
                return await executeProcedure.ExecuteStoredProcedureList<CorreoReporteInventarioDto>("[dbo].[SP_GetCorreosReporteInventario]", new List<SqlParameter>());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<AlmacenReporteInventarioDto>> GetAlmacenesReporteInventario()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            try
            {
                return await executeProcedure.ExecuteStoredProcedureList<AlmacenReporteInventarioDto>("[dbo].[IM_WMS_ObtenerAlmacenesReporteInventario]", new List<SqlParameter>());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> GenerarReporteInventarioExcel(DateTime fechaIni, DateTime fechaFin, string articulo)
        {
            try
            {
                List<string> almacenesReporte = (await GetAlmacenesReporteInventario() ?? new List<AlmacenReporteInventarioDto>())
                    .Where(a => !string.IsNullOrWhiteSpace(a.Almacen))
                    .Select(a => a.Almacen.Trim())
                    .ToList();

                if (!almacenesReporte.Any())
                    return new byte[0];

                Color colorNavy = ColorTranslator.FromHtml("#0D1B2A");
                bool huboDatos = false;
                var resumenPorAlmacen = new List<(string Almacen, int Articulos, int Movimientos)>();
                byte[] excelBytes;

                using (ExcelPackage package = new ExcelPackage())
                {
                    // El reporte siempre va por almacén (obtenidos desde IM_WMS_ObtenerAlmacenesReporteInventario) + rango de fechas.
                    foreach (string almacen in almacenesReporte)
                    {
                        List<KardexInventarioDto> kardex = await GetKardexInventario(fechaIni, fechaFin, almacen, articulo, soloConMovimiento: true) ?? new List<KardexInventarioDto>();
                        List<MovimientoInventarioDto> movimientos = await GetMovimientosInventario(fechaIni, fechaFin, almacen, articulo) ?? new List<MovimientoInventarioDto>();

                        if (kardex.Any() || movimientos.Any())
                            huboDatos = true;

                        CrearHojaResumen(package, almacen, fechaIni, fechaFin, articulo, kardex, colorNavy);
                        CrearHojaDetalle(package, almacen, fechaIni, fechaFin, articulo, movimientos, colorNavy);

                        resumenPorAlmacen.Add((almacen, kardex.Count, movimientos.Count));
                    }

                    if (!huboDatos)
                        return new byte[0];

                    excelBytes = package.GetAsByteArray();
                }

                // Envío automático por correo del reporte generado (solo pruebas)
                await EnviarReportePorCorreo(excelBytes, fechaIni, fechaFin, resumenPorAlmacen);

                return excelBytes;
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }

        private static string NombreHoja(string prefijo, string almacen)
        {
            // Los nombres de hoja en Excel no admiten : \ / ? * [ ] y tienen máximo 31 caracteres.
            string nombre = $"{prefijo}_{almacen}";
            foreach (char c in new[] { ':', '\\', '/', '?', '*', '[', ']' })
                nombre = nombre.Replace(c, '-');

            return nombre.Length > 31 ? nombre.Substring(0, 31) : nombre;
        }

        private static void CrearHojaResumen(ExcelPackage package, string almacen, DateTime fechaIni, DateTime fechaFin, string articulo, List<KardexInventarioDto> kardex, Color colorNavy)
        {
            ExcelWorksheet hoja = package.Workbook.Worksheets.Add(NombreHoja("Resumen", almacen));

            hoja.Cells["A1"].Value = $"REPORTE DE INVENTARIO - RESUMEN - ALMACÉN {almacen}";
            hoja.Cells["A1"].Style.Font.Bold = true;
            hoja.Cells["A1"].Style.Font.Size = 14;

            hoja.Cells["A2"].Value = $"Almacén: {almacen}    Periodo: {fechaIni:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}    Artículo: {(string.IsNullOrWhiteSpace(articulo) ? "Todos" : articulo)}";
            hoja.Cells["A2"].Style.Font.Italic = true;

            int filaEncabezado = 4;
            string[] headers = { "Sitio", "Almacén", "Artículo", "Descripción", "Saldo Inicial", "Entradas", "Salidas", "Saldo Final" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = hoja.Cells[filaEncabezado, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(colorNavy);
            }

            int fila = filaEncabezado + 1;
            foreach (var item in kardex)
            {
                hoja.Cells[fila, 1].Value = item.Sitio;
                hoja.Cells[fila, 2].Value = item.Almacen;
                hoja.Cells[fila, 3].Value = item.Articulo;
                hoja.Cells[fila, 4].Value = item.Descripcion;
                hoja.Cells[fila, 5].Value = item.SaldoInicial;
                hoja.Cells[fila, 6].Value = item.TotalEntradas;
                hoja.Cells[fila, 7].Value = item.TotalSalidas;
                hoja.Cells[fila, 8].Value = item.SaldoFinal;
                fila++;
            }

            int filaUltima = fila - 1;
            if (filaUltima >= filaEncabezado + 1)
            {
                hoja.Cells[filaEncabezado + 1, 5, filaUltima, 8].Style.Numberformat.Format = "#,##0.00";

                // Filtro sobre el encabezado y los datos (sin incluir la fila de totales)
                hoja.Cells[filaEncabezado, 1, filaUltima, 8].AutoFilter = true;

                // Fila de totales
                hoja.Cells[fila, 4].Value = "TOTALES:";
                hoja.Cells[fila, 4].Style.Font.Bold = true;
                hoja.Cells[fila, 5].Formula = $"SUM(E{filaEncabezado + 1}:E{filaUltima})";
                hoja.Cells[fila, 6].Formula = $"SUM(F{filaEncabezado + 1}:F{filaUltima})";
                hoja.Cells[fila, 7].Formula = $"SUM(G{filaEncabezado + 1}:G{filaUltima})";
                hoja.Cells[fila, 8].Formula = $"SUM(H{filaEncabezado + 1}:H{filaUltima})";
                hoja.Cells[fila, 5, fila, 8].Style.Font.Bold = true;
                hoja.Cells[fila, 5, fila, 8].Style.Numberformat.Format = "#,##0.00";
            }

            hoja.Cells[filaEncabezado, 1, Math.Max(filaEncabezado, filaUltima), 8].AutoFitColumns();
            hoja.View.FreezePanes(filaEncabezado + 1, 1);
        }

        private static void CrearHojaDetalle(ExcelPackage package, string almacen, DateTime fechaIni, DateTime fechaFin, string articulo, List<MovimientoInventarioDto> movimientos, Color colorNavy)
        {
            ExcelWorksheet hoja = package.Workbook.Worksheets.Add(NombreHoja("Detalle", almacen));

            hoja.Cells["A1"].Value = $"REPORTE DE INVENTARIO - DETALLE DE MOVIMIENTOS - ALMACÉN {almacen}";
            hoja.Cells["A1"].Style.Font.Bold = true;
            hoja.Cells["A1"].Style.Font.Size = 14;

            hoja.Cells["A2"].Value = $"Almacén: {almacen}    Periodo: {fechaIni:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}    Artículo: {(string.IsNullOrWhiteSpace(articulo) ? "Todos" : articulo)}";
            hoja.Cells["A2"].Style.Font.Italic = true;

            int filaEncabezado = 4;
            string[] headers = { "Sitio", "Almacén", "Fecha", "Artículo", "Descripción", "Movimiento", "Tipo Referencia", "Documento", "Usuario Crea", "Usuario Registra", "Color", "Talla", "Lote", "N° Serie", "Ubicación", "Entrada", "Salida", "Albarán", "Factura", "Comprobante" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = hoja.Cells[filaEncabezado, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(colorNavy);
            }

            int fila = filaEncabezado + 1;
            foreach (var item in movimientos)
            {
                hoja.Cells[fila, 1].Value = item.Sitio;
                hoja.Cells[fila, 2].Value = item.Almacen;
                hoja.Cells[fila, 3].Value = item.FechaFisica;
                hoja.Cells[fila, 4].Value = item.Articulo;
                hoja.Cells[fila, 5].Value = item.Descripcion;
                hoja.Cells[fila, 6].Value = item.Movimiento;
                hoja.Cells[fila, 7].Value = item.TipoReferencia;
                hoja.Cells[fila, 8].Value = item.Documento;
                hoja.Cells[fila, 9].Value = item.UsuarioCrea;
                hoja.Cells[fila, 10].Value = item.UsuarioRegistra;
                hoja.Cells[fila, 11].Value = item.Color;
                hoja.Cells[fila, 12].Value = item.Talla;
                hoja.Cells[fila, 13].Value = item.Lote;
                hoja.Cells[fila, 14].Value = item.NumSerie;
                hoja.Cells[fila, 15].Value = item.Ubicacion;
                hoja.Cells[fila, 16].Value = item.Entrada;
                hoja.Cells[fila, 17].Value = item.Salida;
                hoja.Cells[fila, 18].Value = item.Albaran;
                hoja.Cells[fila, 19].Value = item.Factura;
                hoja.Cells[fila, 20].Value = item.Comprobante;
                fila++;
            }

            int filaUltima = fila - 1;
            if (filaUltima >= filaEncabezado + 1)
            {
                hoja.Cells[filaEncabezado + 1, 3, filaUltima, 3].Style.Numberformat.Format = "dd/MM/yyyy";
                hoja.Cells[filaEncabezado + 1, 16, filaUltima, 17].Style.Numberformat.Format = "#,##0.00";

                // Filtro sobre el encabezado y los datos (sin incluir la fila de totales)
                hoja.Cells[filaEncabezado, 1, filaUltima, 20].AutoFilter = true;

                // Fila de totales
                hoja.Cells[fila, 15].Value = "TOTALES:";
                hoja.Cells[fila, 15].Style.Font.Bold = true;
                hoja.Cells[fila, 16].Formula = $"SUM(P{filaEncabezado + 1}:P{filaUltima})";
                hoja.Cells[fila, 17].Formula = $"SUM(Q{filaEncabezado + 1}:Q{filaUltima})";
                hoja.Cells[fila, 16, fila, 17].Style.Font.Bold = true;
                hoja.Cells[fila, 16, fila, 17].Style.Numberformat.Format = "#,##0.00";
            }

            hoja.Cells[filaEncabezado, 1, Math.Max(filaEncabezado, filaUltima), 20].AutoFitColumns();
            hoja.View.FreezePanes(filaEncabezado + 1, 1);
        }

        private async Task EnviarReportePorCorreo(byte[] excelBytes, DateTime fechaIni, DateTime fechaFin, List<(string Almacen, int Articulos, int Movimientos)> resumenPorAlmacen)
        {
            try
            {
                string nombreArchivoExcel = $"ReporteInventario_{fechaIni:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx";

                List<CorreoReporteInventarioDto> destinatarios = await GetCorreosReporteInventario() ?? new List<CorreoReporteInventarioDto>();

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(VariablesGlobales.Correo);

                if (destinatarios.Any())
                {
                    foreach (var destinatario in destinatarios.Where(d => !string.IsNullOrWhiteSpace(d.Correo)))
                        mail.To.Add(destinatario.Correo.Trim());
                }
                else
                {
                    // Respaldo por si la tabla de correos no devolvió resultados
                    mail.To.Add("ebueso@intermoda.com.hn");
                }

                mail.Subject = $"Reporte de Inventario ({fechaIni:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy})";
                mail.IsBodyHtml = true;

                var filasResumen = new StringBuilder();
                foreach (var r in resumenPorAlmacen)
                {
                    filasResumen.Append($"<tr><td>{r.Almacen}</td><td style='text-align:center'>{r.Articulos}</td><td style='text-align:center'>{r.Movimientos}</td></tr>");
                }

                mail.Body = $@"
                <div style='font-family: Arial, sans-serif; color: #0D1B2A;'>
                    <h2>Reporte de Movimientos de Inventario</h2>
                    <p>Se ha generado el reporte para el periodo <b>{fechaIni:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}</b>, con una hoja de <b>Resumen</b> y una de <b>Detalle</b> por cada almacén de bodega:</p>
                    <table cellpadding='6' cellspacing='0' style='border-collapse:collapse;border:1px solid #ccc;'>
                        <thead>
                            <tr style='background:#0D1B2A;color:#fff;'><th>Almacén</th><th>Artículos (Resumen)</th><th>Movimientos (Detalle)</th></tr>
                        </thead>
                        <tbody>
                            {filasResumen}
                        </tbody>
                    </table>
                    <br>
                    <p>Adjunto encontrará el <b>reporte en formato Excel</b> con todas las hojas.</p>
                    <br>
                    <p>Atentamente,<br><b>Sistema de Control de Inventarios WMS</b><br>Intermoda Honduras S.A. de C.V.</p>
                </div>";

                using (MemoryStream ms = new MemoryStream(excelBytes))
                {
                    Attachment adjunto = new Attachment(ms, nombreArchivoExcel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    mail.Attachments.Add(adjunto);

                    using (SmtpClient oSmtpClient = new SmtpClient())
                    {
                        oSmtpClient.Host = "smtp.office365.com";
                        oSmtpClient.Port = 587;
                        oSmtpClient.EnableSsl = true;
                        oSmtpClient.UseDefaultCredentials = false;
                        oSmtpClient.Credentials = new NetworkCredential(VariablesGlobales.Correo, VariablesGlobales.Correo_Password);

                        await oSmtpClient.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception)
            {
                // No se interrumpe la generación/descarga del Excel si falla el envío del correo.
            }
        }
    }
}
