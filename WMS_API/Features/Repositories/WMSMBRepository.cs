using Core.DTOs.CAEX.Guia;
using Core.DTOs.Despacho_PT;
using Core.DTOs.MB;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using OfficeOpenXml.FormulaParsing.FormulaExpressions;
using OfficeOpenXml.Packaging.Ionic.Zip;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class WMSMBRepository : IWMSMBRespository
    {
        private readonly string _connectionString;
        public WMSMBRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzasDev");
        }

        public async Task<IM_WMS_MB_InsertBox> getInsertBox(string Orden, int Caja, string Ubicacion, int Consolidado, string usuarioRecepcion, string Camion)
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
        public async Task<List<IM_WMS_Correos_DespachoPTDTO>> getCorreosRecepcionUbicacionCajas()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
            };

            List<IM_WMS_Correos_DespachoPTDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Correos_DespachoPTDTO>("[IM_WMS_CorreosRecepcionUbicacionCajas]", parametros);

            return response;
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
                    worksheet.Cells[1, 7].Value = "FechaRecepcion";
                    worksheet.Cells[1, 8].Value = "Color";
                    worksheet.Cells[1, 9].Value = "Ubicacion";

                    int fila = 2;
                    data.ForEach(x =>
                    {
                        worksheet.Cells[fila, 1].Value = x.Lote;
                        worksheet.Cells[fila, 2].Value = x.Orden;
                        worksheet.Cells[fila, 3].Value = x.Articulo;
                        worksheet.Cells[fila, 4].Value = x.NumeroCaja;
                        worksheet.Cells[fila, 5].Value = x.Talla;
                        worksheet.Cells[fila, 6].Value = x.Cantidad;
                        worksheet.Cells[fila, 7].Value = x.FechaRecepcion;
                        worksheet.Cells[fila, 8].Value = x.Color;
                        worksheet.Cells[fila, 9].Value = x.UbicacionRecepcion;
                        fila++;

                    });
                    var rangeTable = worksheet.Cells[1, 1, fila, 9];
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

                        var correos = await getCorreosRecepcionUbicacionCajas();

                        correos.ForEach(x =>
                        {
                            mail.To.Add(x.Correo);

                        });
                        //mail.To.Add("bavila@intermoda.com.hn");


                        TimeSpan tiempo = data[0].FechaRecepcion - data[data.Count - 1].FechaRecepcion;


                        mail.Subject = "Recepcion Producto Terminado MB " + fecha;
                        mail.IsBodyHtml = true;

                        mail.Body = "<p>Recepcion Producto Terminado MB" + " Camion: " + data[0].Camion + " Usuario: " + data[0].usuarioRecepcion + ", Descargado en " + tiempo.Hours + " horas y " + tiempo.Minutes + " Minutos " + tiempo.Seconds + " Segundos</p>";

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
        public async Task<List<IM_WMS_MB_CajasDisponibles>> GetCajasDisponiblesTodo(FiltroCajasDisponiblesMB Filtro)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@orden",Filtro.Orden),
                new SqlParameter("@Articulo",Filtro.Articulo),
                new SqlParameter("@Color",Filtro.Color),
                new SqlParameter("@page",Filtro.Page),
                new SqlParameter("@size",Filtro.Size)
            };

            List<IM_WMS_MB_CajasDisponibles> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_MB_CajasDisponibles>("[IM_WMS_MB_CajasDisponibles]", parametros);
            return result;
        }
        public async Task<List<IM_WMS_MB_CajasDisponibles2>> GetCajasDisponibles(FiltroCajasDisponiblesMB Filtro)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@orden",Filtro.Orden),
                new SqlParameter("@Articulo",Filtro.Articulo),
                new SqlParameter("@Color",Filtro.Color),
                new SqlParameter("@page",Filtro.Page),
                new SqlParameter("@size",Filtro.Size)
            };

            List<IM_WMS_MB_CajasDisponibles> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_MB_CajasDisponibles>("[IM_WMS_MB_CajasDisponibles]", parametros);
            List<IM_WMS_MB_CajasDisponibles2> data = new List<IM_WMS_MB_CajasDisponibles2>();

            foreach (var item in result.FindAll(x => x.IDConsolidado == 0))
            {
                IM_WMS_MB_CajasDisponibles2 tmp = new IM_WMS_MB_CajasDisponibles2();

                tmp.ID = item.ID;
                tmp.IDConsolidado = item.IDConsolidado;
                tmp.Lote = item.Lote;
                tmp.Orden = item.Orden;
                tmp.Articulo = item.Articulo;
                tmp.DescripcioMB = item.DescripcioMB;
                tmp.NumeroCaja = item.NumeroCaja;
                tmp.Talla = item.Talla;
                tmp.Cantidad = item.Cantidad;
                tmp.Color = item.Color;
                tmp.NombreColor = item.NombreColor;
                tmp.CantidadTotal = item.CantidadTotal;
                tmp.CantidadCajas = item.CantidadCajas;
                tmp.PickToDespacho = item.PickToDespacho;
                tmp.subRows = new List<IM_WMS_MB_CajasDisponibles2>();
                if (result.Count(x => x.IDConsolidado == tmp.ID) > 0)
                {
                    List<IM_WMS_MB_CajasDisponibles2> subRows = new List<IM_WMS_MB_CajasDisponibles2>();
                    //subRows.Add(tmp);

                    tmp.NumeroCaja = 0;
                    tmp.Talla = "";
                    tmp.Cantidad = 0;
                    tmp.Color = "";
                    tmp.NombreColor = "";

                    foreach (var subItem in result.FindAll(x => x.IDConsolidado == tmp.ID || x.ID == tmp.ID))
                    {
                        IM_WMS_MB_CajasDisponibles2 tmpSubrow = new IM_WMS_MB_CajasDisponibles2();
                        tmpSubrow.ID = subItem.ID;
                        tmpSubrow.IDConsolidado = tmp.ID;
                        tmpSubrow.Lote = subItem.Lote;
                        tmpSubrow.Orden = subItem.Orden;
                        tmpSubrow.Articulo = subItem.Articulo;
                        tmpSubrow.DescripcioMB = subItem.DescripcioMB;
                        tmpSubrow.NumeroCaja = subItem.NumeroCaja;
                        tmpSubrow.Talla = subItem.Talla;
                        tmpSubrow.Cantidad = subItem.Cantidad;
                        tmpSubrow.Color = subItem.Color;
                        tmpSubrow.NombreColor = subItem.NombreColor;
                        tmpSubrow.CantidadTotal = 0;
                        tmpSubrow.CantidadCajas = 0;
                        tmpSubrow.PickToDespacho = subItem.PickToDespacho;
                        tmpSubrow.subRows = new List<IM_WMS_MB_CajasDisponibles2>();
                        subRows.Add(tmpSubrow);
                    }
                    tmp.subRows = subRows;

                }

                data.Add(tmp);
            }
            return data;
        }

        public async Task<IM_WMS_MB_CajasDisponibles> getActualizarCajasParaDespacho(int id, bool PickToDespacho)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@id",id),
                new SqlParameter("@PickToDespacho",PickToDespacho)

            };

            IM_WMS_MB_CajasDisponibles result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_MB_CajasDisponibles>("[IM_WMS_MB_ActualizarCajasParaDespacho]", parametros);
            return result;
        }

        public async Task<List<IM_WMS_MB_ResumenArticulosSeleccionados>> GetResumenArticulosSeleccionados()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
            };

            List<IM_WMS_MB_ResumenArticulosSeleccionados> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_MB_ResumenArticulosSeleccionados>("[IM_WMS_MB_ResumenArticulosSeleccionados]", parametros);
            return result;
        }

        public async Task<IM_WMS_MB_CrearDespacho> getGenerarDespacho(string usuario)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Usuario",usuario)

            };

            IM_WMS_MB_CrearDespacho result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_MB_CrearDespacho>("[IM_WMS_MB_CrearDespacho]", parametros);
            return result;
        }

        public async Task<List<IM_WMS_MB_CrearDespacho>> getDespachosPendientes()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {

            };

            List<IM_WMS_MB_CrearDespacho> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_MB_CrearDespacho>("[IM_WMS_MB_ObtenerDespachosPendientes]", parametros);
            return result;
        }

        public async Task<List<IM_WMS_MB_PICKING>> getPicking(int DespachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID",DespachoID)

            };

            List<IM_WMS_MB_PICKING> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_MB_PICKING>("[IM_WMS_MB_PICKING]", parametros);
            return result;
        }

        public async Task<IM_WMS_MB_PICKING> getUpdatePicking(int id, string usuario)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@id",id),
                new SqlParameter("@usuario",usuario)

            };

            IM_WMS_MB_PICKING result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_MB_PICKING>("[IM_WMS_MB_UpdatePICKING]", parametros);
            return result;
        }

        public async Task<List<IM_WMS_MB_PACKING>> getPacking(int DespachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID",DespachoID)

            };

            List<IM_WMS_MB_PACKING> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_MB_PACKING>("[IM_WMS_MB_PACKING]", parametros);
            return result;
        }

        public async Task<IM_WMS_MB_PACKING> getUpdatePacking(int id, string usuario, string Pallet)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@id",id),
                new SqlParameter("@usuario",usuario),
                new SqlParameter("@pallet",Pallet)

            };

            IM_WMS_MB_PACKING result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_MB_PACKING>("[IM_WMS_MB_UpdatePACKING]", parametros);
            return result;
        }

        public async Task<List<IM_WMS_MB_Tracking>> getTracking(int id)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@id",id)

            };

            List<IM_WMS_MB_Tracking> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_MB_Tracking>("[IM_WMS_MB_Tracking]", parametros);
            return result;
        }

        public async Task<List<IM_WMS_MB_Trackingpallet>> getTrackingPallet(int id)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@id",id)

            };

            List<IM_WMS_MB_Trackingpallet> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_MB_Trackingpallet>("[IM_WMS_MB_Trackingpallet]", parametros);
            return result;
        }

        public async Task<List<IM_WMS_MB_ResumenDespachoPallet>> GetResumenDespachoPallets(int id)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@id",id)

            };

            List<IM_WMS_MB_ResumenDespachoPallet> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_MB_ResumenDespachoPallet>("[IM_WMS_MB_ResumenDespachoPallet]", parametros);
            return result;
        }

        public async Task<List<IM_WMS_Correos_DespachoPTDTO>> getCorreosDespachoMB()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
            };

            List<IM_WMS_Correos_DespachoPTDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Correos_DespachoPTDTO>("[IM_WMS_MB_CorreosDesapchoMB]", parametros);

            return response;
        }

        public async Task<IM_WMS_MB_ReimpresionEtiqueta> GetEtiquetaDespacho(string workOrderId, string boxNum)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametro = new List<SqlParameter>
            {    new SqlParameter("@workOrderId", workOrderId),
                 new SqlParameter("@boxNum",boxNum)
            };
            var respone = await executeProcedure.ExecuteStoredProcedure<IM_WMS_MB_ReimpresionEtiqueta>("IM_WMS_MB_ReimpresionEtiqueta", parametro);
            return respone;
        }
        public async Task<string> ImprimirEtiquetaDespachoNormal(IM_WMS_MB_ReimpresionEtiqueta data, string impresora)
        {
            var resultadoUpdateCantidad = await UpdateCantidad(data.WorkOrderId, Convert.ToInt32(data.Qty), data.BoxNum);
            var seActulizoCantidad = resultadoUpdateCantidad != null && resultadoUpdateCantidad.Cantidad == Convert.ToInt32(data.Qty);
            if (!seActulizoCantidad)
            {
                return "No se pudo actualizar la cantidad";
            }

            string etiqueta = "^XA";
            etiqueta += "^PW800";      // Ancho total de la etiqueta
            etiqueta += "^LL1200";    // Largo total de la etiqueta
            etiqueta += "^MD8";     // Densidad media de impresión
            etiqueta += "^PRC";     // Velocidad de impresión

            etiqueta += "^CF0,40";
            etiqueta += @"^FO70,60^FD" + data.DescripcionCompleta + " ^FS";

            data.UbicacionCompleta = QuitarCaracteresEspeciales(data.UbicacionCompleta);
            etiqueta += @"^CF0,28";
            etiqueta += @"^FO75,110^FD" + data.UbicacionCompleta + "^FS";

            //lineas
            etiqueta += @"^FO30,30^GB740,1140,2^FS  
            ^FO30,150^GB740,2,2^FS   
            ^FO30,300^GB502,2,2^FS   
            ^FO30,510^GB740,2,2^FS   
            ^FO30,680^GB442,2,2^FS   
            ^FO30,732^GB320,2,2^FS   
            ^FO30,790^GB740,2,2^FS   

            ^FO530,150^GB2,360,2^FS   
            ^FO300,300^GB2,210,2^FS   
            ^FO470,510^GB2,280,2^FS   
            ^FO350,680^GB2,110,2^FS";
            etiqueta += @"^CF0,50";
            etiqueta += @"^FO70,200^FD" + data.WorkOrderId + "^FS";

            etiqueta += @"^CF0,35";
            etiqueta += @"^FO590,160^FD" + "Caja #" + "^FS";

            etiqueta += "^CF0,80";
            etiqueta += "^FO610,220^FD" + data.BoxNum + "^FS";
            etiqueta += "^FO570,290^BQN,2,6";

            etiqueta += @"^FDQA," + data.WorkOrderId + "," + data.BoxNum + "^FS";

            etiqueta += @"^CF0,35";
            etiqueta += @"^FO550,465^FD" + data.UserName + "^FS";

            etiqueta += @"^CF0,65";
            etiqueta += @"^FO80,370^FD" + data.BatchId + "^FS";

            etiqueta += @"^CF0,40";
            etiqueta += @"^FO330,320^FDQUANTITY^FS";

            etiqueta += @"^CF0,80";
            etiqueta += @"^FO350,385^FD" + data.Qty + "^FS";


            List<string> parrafosAceptables = new List<string> { "" };

            int espacioDisponible = 35;
            if (data.ProductNameMB.Length > espacioDisponible)
            {
                string[] palabras = data.ProductNameMB.Split(' ');
                int parrafo = 0;

                foreach (string palabra in palabras)
                {
                    if (palabra.Length + 1 <= espacioDisponible)
                    {
                        parrafosAceptables[parrafo] += palabra + " ";
                        espacioDisponible -= palabra.Length + 1;
                    }
                    else
                    {
                        parrafosAceptables.Add(palabra + " ");
                        parrafo++;
                        espacioDisponible = 30 - (palabra.Length + 1);
                    }
                }

                int separacionVerticalEntrelineas = 550;
                foreach (string parr in parrafosAceptables)
                {
                    etiqueta += @"^CF0,25";
                    etiqueta += @"^FO40," + separacionVerticalEntrelineas + "^FD" + parr.Trim() + "^FS";
                    separacionVerticalEntrelineas += 40;
                }
            }
            else
            {
                etiqueta += @"^CF0,25";
                etiqueta += @"^FO40,550^FD" + data.ProductNameMB + "^FS";
            }


            etiqueta += "^BY1,3,1";
            etiqueta += "^FO530,600^BCN,100,N,N,N";
            etiqueta += @"^FD" + data.Barcode + "^FS";
            etiqueta += "^CF,25,30";
            etiqueta += @"^FO530,720^FD" + data.Barcode + "^FS";



            etiqueta += @"^CF0,30";
            etiqueta += @"^FO50,700^FD" + data.ProductId + "^FS";
            etiqueta += @"^FO50,750^FD" + data.Style + "^FS";

            etiqueta += "^CF0,25";
            etiqueta += "^FO380,690^FDTalla^FS";
            etiqueta += "^CF0,60";
            etiqueta += "^FO375,720^FD" + data.Size + "^FS";

            etiqueta += "^CF0,25";
            etiqueta += "^FO70,1100^FD" + DateTime.Now.ToString("d/M/yyyy HH:mm:ss") + " " +
                        QuitarCaracteresEspeciales(DateTime.Now.ToString("tt")) + " " + data.BoxCategoryDescription + "^FS";

            etiqueta += "^CF0,20";
            etiqueta += "^FO590,1100^FD Reimpresion # " + resultadoUpdateCantidad.ReimpresionNum + " ^FS";


            etiqueta += "^XZ";
            try
            {
                using (TcpClient client = new TcpClient(impresora, 9100))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(etiqueta);

                        stream.Write(bytes, 0, bytes.Length);
                        Thread.Sleep(1200);

                    }
                }
            }
            catch (Exception err)
            {
                return err.ToString();
            }
            ;

            return "OK";
        }


        static string QuitarCaracteresEspeciales(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            // Normalizar a forma de descomposición (NFD
            string normalizado = texto.Normalize(NormalizationForm.FormD);

            // Quitar los acentos (marcas de combinación)
            var sb = new StringBuilder();
            foreach (var c in normalizado)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                if (cat != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            string sinAcentos = sb.ToString().Normalize(NormalizationForm.FormC);

            // Eliminar caracteres no alfanuméricos (dejando solo letras, números y espacios)
            string limpio = Regex.Replace(sinAcentos, @"[^a-zA-Z0-9,\.\s]", "");

            // Quitar espacios múltiples
            limpio = Regex.Replace(limpio, @"\s+", " ").Trim();

            return limpio;
        }

        public async Task<RespuestaUpdateReimpresión> UpdateCantidad(string op, int nuevaCantidad, string boxNum)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Orden", op),
                new SqlParameter("@NuevaCantidad", nuevaCantidad),
                new SqlParameter("@BoxNum", boxNum)
            };
            var resultado = await executeProcedure.ExecuteStoredProcedure<RespuestaUpdateReimpresión>("UpdateCantidad_MB_Cajas", parameters);

            return resultado;
        }

        public async Task<bool> ValidarAccesoAPantlla(string codigoUsuario)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CodigoUsuario", codigoUsuario)
            };
            var resultado = await executeProcedure.ExecuteStoredProcedure<RespuestaValidacionUsuario>("IM_WMS_MB_ValidarAcessoPantalla", parameters);

            var esUsuarioValido = codigoUsuario == resultado.CodigoUsuario; 

            return esUsuarioValido;

        }
    }
}
