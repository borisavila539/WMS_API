using Core.DTOs;
using Core.DTOs.Despacho_PT;
using Core.DTOs.Reduccion_Cajas;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OfficeOpenXml.Style;
using Core.DTOs.DiarioTransferir;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class WMSRepository : IWMSRepository
    {
        private readonly string _connectionString;

        public WMSRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzas");
        }

        public async Task<LoginDTO> PostLogin(LoginDTO datos)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@user",datos.user),
                new SqlParameter("@pass",datos.pass)
            };

            LoginDTO result = await executeProcedure.ExecuteStoredProcedure<LoginDTO>("[IM_Login_WMS]", parametros);

            return result;
        }       

        //Diarios de salida

        public async Task<List<DiariosAbiertosDTO>> GetDiariosAbiertos(string user, string filtro)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@user",user),
                new SqlParameter("@filtro", filtro)
            };

            List<DiariosAbiertosDTO> result = await executeProcedure.ExecuteStoredProcedureList<DiariosAbiertosDTO>("[IM_ObtenerDiariosMovimientoAbiertos]", parametros);

            return result;
        }
        public async Task<List<LineasDTO>> GetLineasDiario(string diario)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@JOURNALID",diario)
            };

            List<LineasDTO> result = await executeProcedure.ExecuteStoredProcedureList<LineasDTO>("[IM_ObtenerLineasDiario]", parametros);

            return result;
           
        }      
        public async Task<List<EtiquetaDTO>> GetDatosEtiquetaMovimiento(string diario, string IMBoxCode)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@diario", diario),
                new SqlParameter("@IMBOXCODE", IMBoxCode)
            };

            List<EtiquetaDTO> result = await executeProcedure.ExecuteStoredProcedureList<EtiquetaDTO>("[IM_Movimiento_Diario_Etiqueta]", parametros);

            return result;           
        }
        public async Task<string> GetImprimirEtiquetaMovimiento(string diario, string IMBoxCode, string PRINT)
        {
            var data = await GetDatosEtiquetaMovimiento(diario, IMBoxCode);

            //creacion de encabezado de la etiqueta
            string encabezado = "";
            encabezado += "^XA";
            encabezado += "^CF0,50";
            encabezado += "^FO280,50^FD" + diario + "^FS";
            encabezado += "^CF0,24";
            encabezado += "^FO50,115^FDFecha: " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "^FS";
            encabezado += "^FO50,140^FDSoliciante: " + data[0].Solicitante + "^FS";
            encabezado += "^FO50,165^FDCaja #" + data[0].Numero_caja + "^FS";
            encabezado += "^FO480,115^FDCentro Costos: " + data[0].IM_CENTRO_DE_COSTOS + "^FS";
            encabezado += "^FO480,165^FDTipo Diario: " + data[0].JOURNALNAMEID + "^FS";
            encabezado += "^FO50,190^GB700,3,3^FS";

            //creacion del pie de pagina de la etiqueta
            string pie = "";
            pie += "^BY2,2,100";
            pie += "^FO50,1060^BC^FD" + IMBoxCode + "^FS";

            pie += "^FO430,1060^FDEmpacador: " + data[0].Empacador + "^FS";
            pie += "^CF0,40";


            string etiqueta = encabezado;
            int totalUnidades = 0;

            var groupData = new Dictionary<string, List<EtiquetaDTO>>();

            foreach (var element in data)
            {
                totalUnidades += element.QTY;
                var key = $"{element.ITEMID}-{element.INVENTCOLORID}";

                if (!groupData.ContainsKey(key))
                {
                    groupData[key] = new List<EtiquetaDTO>();
                }

                groupData[key].Add(element);
            }

            var groupArray = groupData.Select(x => new EtiquetaGroupDTO
            {
                key = x.Key,
                items = x.Value
            }).ToArray();

            int lineaArticulo = 210;
            int total = 0;

            foreach (var element in groupArray)
            {
                etiqueta += $"^FO50,{lineaArticulo}^FD{element.items[0].ITEMID}*{element.items[0].INVENTCOLORID}^FS";
                etiqueta += $"^FO50,{lineaArticulo + 30}^FDTalla: ";
                element.items.ForEach(x =>
               {
                   for (int i = 1; i <= 5 - x.INVENTSIZEID.Length; i++)
                   {
                       etiqueta += "_";
                   }
                   etiqueta += x.INVENTSIZEID;

               });

                etiqueta += "^FS";
                etiqueta += $"^FO50,{lineaArticulo + 60}^FDQTY:  ";
                int totalLinea = 0;
                element.items.ForEach(x =>
                {
                    for (int i = 1; i <= 5 - x.QTY.ToString().Length; i++)
                    {
                        etiqueta += "_";
                    }
                    etiqueta += x.QTY;
                    total += x.QTY;
                    totalLinea += x.QTY;
                });

                etiqueta += $" ={totalLinea}^FS";


                if (lineaArticulo == 910)
                {

                    etiqueta += pie;
                    etiqueta += $"^FO430,1090^FDTotal: {total}/{totalUnidades}^FS";
                    etiqueta += "^XZ";

                    try
                    {
                        using (TcpClient client = new TcpClient(PRINT, 9100))
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);
                                stream.Write(bytes, 0, bytes.Length);

                            }

                        }

                    }
                    catch (Exception err)
                    {
                        return err.ToString();
                    }


                    etiqueta = encabezado;
                    lineaArticulo = 210;
                    total = 0;

                }
                else
                {
                    lineaArticulo += 100;
                }

            }

            if (lineaArticulo != 910)
            {
                etiqueta += pie;
                etiqueta += $"^FO430,1090^FDTotal: {total}/{totalUnidades}^FS";
                etiqueta += "^XZ";
            }



            try
            {
                using (TcpClient client = new TcpClient(PRINT, 9100))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);
                        stream.Write(bytes, 0, bytes.Length);

                    }

                }
                return "OK";
            }
            catch (Exception err)
            {
                return err.ToString();
            }
            //mandar imprimir 
            return etiqueta;


        }
        public async Task<List<ImpresoraDTO>> getImpresoras()
        {

            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter> { };

            List<ImpresoraDTO> result = await executeProcedure.ExecuteStoredProcedureList<ImpresoraDTO>("[IM_Impresoras_SanBernardo]", parametros);

            return result;
            
        }
        //Despacho de Tela
        public async Task<List<IM_WMS_Despacho_Tela_Detalle_AX>> GetIM_WMS_Despacho_Telas(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO)
        {

            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@TRANSFERIDFROM", TRANSFERIDFROM),
                new SqlParameter("@TRANSFERIDTO", TRANSFERIDTO),
                new SqlParameter("@INVENTLOCATIONIDTO", INVENTLOCATIONIDTO)
            };

            List<IM_WMS_Despacho_Tela_Detalle_AX> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Despacho_Tela_Detalle_AX>("[IM_WMS_Despacho_Tela_Detalle_AX]", parametros);

            return result;            
        }
        public async Task<List<IM_WMS_Despacho_Tela_Detalle_Rollo>> Get_Despacho_Tela_Detalle_Rollo(string INVENTSERIALID, string InventTransID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@INVENTSERIALID", INVENTSERIALID),
                new SqlParameter("@InventTransID", InventTransID)
            };

            List<IM_WMS_Despacho_Tela_Detalle_Rollo> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Despacho_Tela_Detalle_Rollo>("[IM_WMS_Despacho_Tela_Detalle_Rollo]", parametros);

            return result;            
        }       
        public async Task<List<DespachoTelasDetalleDTO>> GetDespacho_Telas(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO, string tipo)
        {
            var response = new List<DespachoTelasDetalleDTO>();
            var AX = await GetIM_WMS_Despacho_Telas(TRANSFERIDFROM, TRANSFERIDTO, INVENTLOCATIONIDTO);

            foreach (var element in AX)
            {
                var detalle = await Get_Despacho_Tela_Detalle_Rollo(element.INVENTSERIALID, element.TRANSFERID);
                if ((tipo == "PACKING" && detalle[0].Picking) || tipo == "PICKING" || tipo == "RECEIVE")
                {

                    DespachoTelasDetalleDTO tmp = new DespachoTelasDetalleDTO();
                    tmp.TRANSFERID = element.TRANSFERID;
                    tmp.INVENTSERIALID = element.INVENTSERIALID;
                    tmp.APVENDROLL = element.APVENDROLL;
                    tmp.QTYTRANSFER = element.QTYTRANSFER;
                    tmp.NAME = element.NAME;
                    tmp.CONFIGID = element.CONFIGID;
                    tmp.INVENTBATCHID = element.INVENTBATCHID;
                    tmp.ITEMID = element.ITEMID;
                    tmp.BFPITEMNAME = element.BFPITEMNAME;
                    tmp.Picking = detalle[0].Picking;
                    tmp.Packing = detalle[0].Packing;
                    tmp.receive = detalle[0].Receive;
                    tmp.wmslocationid = element.wmslocationid;
                    response.Add(tmp);
                }

            }
            
            return response;
        }
        public async Task<List<IM_WMS_Despacho_Tela_Detalle_Rollo>> GetDespacho_Tela_Picking_Packing(string INVENTSERIALID, string TIPO, string CAMION, string CHOFER, string InventTransID, string USER, int IDremision)
        {

            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@INVENTSERIALID", INVENTSERIALID),
                new SqlParameter("@Tipo", TIPO),
                new SqlParameter("@Camion", CAMION),
                new SqlParameter("@Chofer", CHOFER),
                new SqlParameter("@InventTransID", InventTransID),
                new SqlParameter("@User", USER),
                new SqlParameter("@ID", IDremision)
            };

            List<IM_WMS_Despacho_Tela_Detalle_Rollo> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Despacho_Tela_Detalle_Rollo>("[IM_WMS_Despacho_Tela_Picking_Packing]", parametros);

            return result;          
        }
        public async Task<string> postImprimirEtiquetaRollo(List<EtiquetaRolloDTO> data)
        {
            string etiqueta = "";

            etiqueta += "^XA^CF0,22^BY3,2,100";
            etiqueta += $"^FO200,50^BC^FD{data[0].INVENTSERIALID}^FS";
            etiqueta += $"^FO50,220^FDProveedor: {data[0].APVENDROLL}^FS";
            etiqueta += $"^FO500,220^FDCantidad: {data[0].QTYTRANSFER} {(data[0].ITEMID.Substring(0, 2) == "45" ? "lb" : "yd")}^FS";
            etiqueta += $"^FO50,250^FDTela: {data[0].ITEMID}^FS";
            etiqueta += $"^FO500,250^FDColor: {data[0].COLOR}^FS";
            etiqueta += $"^FO50,280^FDLote: {data[0].INVENTBATCHID}^FS";
            etiqueta += $"^FO500,280^FDConfiguracion: {data[0].CONFIGID}^FS^XZ";

            try
            {
                using (TcpClient client = new TcpClient(data[0].PRINT, 9100))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);
                        stream.Write(bytes, 0, bytes.Length);

                    }

                }
                return "OK";
            }
            catch (Exception err)
            {
                return err.ToString();
            }
        }
        public async Task<List<IM_WMS_TrasladosAbiertos>> getTrasladosAbiertos(string INVENTLOXATIONIDTO)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@location", INVENTLOXATIONIDTO)
            };

            List<IM_WMS_TrasladosAbiertos> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_TrasladosAbiertos>("[IM_WMS_TrasladosAbiertos]", parametros);

            return result;           
        }       
        public async Task<List<IM_WMS_EstadoTrasladosDTO>> getEstadotraslados(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@TRANSFERIDFROM", TRANSFERIDFROM),
                new SqlParameter("@TRANSFERIDTO", TRANSFERIDTO),
                new SqlParameter("@INVENTLOCATIONIDTO", INVENTLOCATIONIDTO)
        };

            List<IM_WMS_EstadoTrasladosDTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_EstadoTrasladosDTO>("[IM_WMS_EstadoTraslados]", parametros);

            return result;       
        }       
        public async Task<List<EstadoTrasladoTipoDTO>> gteEstadoTrasladoTipo(string TRANSFERIDFROM, string TRANSFERIDTO, string INVENTLOCATIONIDTO)
        {
            var response = new List<EstadoTrasladoTipoDTO>();
            var AX = await GetIM_WMS_Despacho_Telas(TRANSFERIDFROM, TRANSFERIDTO, INVENTLOCATIONIDTO);

            bool tipo = AX[0].ITEMID.Contains("45 0");


            foreach (var element in AX)
            {
                var detalle = await Get_Despacho_Tela_Detalle_Rollo(element.INVENTSERIALID, element.TRANSFERID);
                if (tipo)
                {

                    string tipoS = "";
                    tipoS = response.Find(elemen => elemen.Tipo == element.ITEMID.Substring(0, 8))?.Tipo;

                    if (tipoS == "" || tipoS == null)
                    {
                        EstadoTrasladoTipoDTO tmp = new EstadoTrasladoTipoDTO();
                        tmp.Tipo = element.ITEMID.Substring(0, 8);
                        response.Add(tmp);
                    }

                    int index = response.FindIndex(elemen => elemen.Tipo == element.ITEMID.Substring(0, 8));
                    if (detalle[0].Picking)
                    {
                        response[index].picking++;
                    }
                    if (detalle[0].Packing)
                    {
                        response[index].Enviado++;
                    }

                    if (detalle[0].Receive)
                    {
                        response[index].Recibido++;
                    }
                    response[index].Total++;


                }
                else
                {
                    string tipoS = "";
                    tipoS = response.Find(elemen => elemen.Tipo == element.BFPITEMNAME)?.Tipo;

                    if (tipoS == "" || tipoS == null)
                    {
                        EstadoTrasladoTipoDTO tmp = new EstadoTrasladoTipoDTO();
                        tmp.Tipo = element.BFPITEMNAME;
                        response.Add(tmp);
                    }

                    int index = response.FindIndex(elemen => elemen.Tipo == element.BFPITEMNAME);

                    if (detalle[0].Picking)
                    {
                        response[index].picking++;
                    }

                    if (detalle[0].Packing)
                    {
                        response[index].Enviado++;
                    }

                    if (detalle[0].Receive)
                    {
                        response[index].Recibido++;
                    }
                    response[index].Total++;



                }

            }

            return response;
        }
        public async Task<List<CrearDespachoDTO>> GetCrearDespacho(string RecIDTraslados, string Chofer, string camion)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@RecIDtraslado", RecIDTraslados),
                new SqlParameter("@chofer", Chofer),
                new SqlParameter("@camion", camion)
            };

            List<CrearDespachoDTO> result = await executeProcedure.ExecuteStoredProcedureList<CrearDespachoDTO>("[IM_WMS_CrearDespacho]", parametros);

            return result;            
        }     
        public async Task<List<CrearDespachoDTO>> GetObtenerDespachos(string RecIDTraslados)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@RecIDtraslado", RecIDTraslados)
            };

            List<CrearDespachoDTO> result = await executeProcedure.ExecuteStoredProcedureList<CrearDespachoDTO>("[IM_WMS_ObtenerDespachos]", parametros);

            return result;           
        }
        public async Task<List<CerrarDespachoDTO>> getCerrarDespacho(int id)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@ID", id)
            };

            List<CerrarDespachoDTO> result = await executeProcedure.ExecuteStoredProcedureList<CerrarDespachoDTO>("[IM_CerrarDespacho]", parametros);

            return result;           
        }        
        public async Task<string> getNotaDespacho(int DespachoID, string recid, string empleado, string camion)
        {
            string despacho = "";

            despacho += @"<!DOCTYPE html>
                            <html lang='en'>
                            <head>
                              <meta charset = 'UTF-8'/>
                               <style>
                                    h3,h4,td{
                                        text-align: center;
                                    }
                                    table,tr,th,td{
                                        border: 1px solid black;
                                        border-collapse: collapse;
                                    }
                                    .container{
                                        display: flex;
                                        justify-content: space-between;
                                    }
                                    .observacion {
                                        border-style: solid;
                                        border-width: 2px;
                                    }
                                </style>
                            </head>

                            <body>
                              <h3> Nota de Despacho</h3>
   
                                 <h4> Despacho #";
            //colocar el numero de despacho
            despacho += DespachoID.ToString().PadLeft(8, '0') + "</h4>";

            //colocar detalle del encabezado
            var encabezado = await getEncabezadoDespacho(empleado, recid);
            despacho += @"
                     <p>
                        <strong>Fecha: </strong>"+encabezado[0].fecha.ToString("dd/MM/yyyy HH:mm:ss") + @" <br>
  	                    <strong>Motorista: </strong>" + encabezado[0].Motorista +" / "+camion+ @" <br>
                        <strong>Traslado Inicial: </strong>" + encabezado[0].TRANSFERIDFROM + @" <br>
                        <strong>Traslado Final: </strong>" + encabezado[0].TRANSFERIDTO + @" <br>
                        <strong>Destino: </strong>" + encabezado[0].Destino + @" <br>
                      </p>
                   ";
            //obtener rollos que son parte del despacho
            var rollos = await getRolloDespacho(DespachoID);
            List<RollosDespachoDTO> data = new List<RollosDespachoDTO>();

            //colocar encabezado de la tabla correo
           
            string htmlCorreo = despacho + @"<table style='width: 100%'>
                               <thead> 
                                 <tr> 
                                   <th colspan = '7'> Detalle </th>  
                                  </tr>  
                                  <tr>  
                                    <th> # </th>
                                    <th> Rollo </th> 
                                    <th> Nombre Busqueda </th> 
                                    <th> Importacion </th> 
                                    <th> Color / Referencia </th> 
                                    <th> Ancho </th>  
                                    <th> Cantidad </th>  
                                    <th> Libras/Yardas </th>  
                                  </tr>  
                                  </thead>  
                                <tbody> ";
           

            
            //obtener informacionn de ax de los rollos
            foreach (var element in rollos)                
            {
                RollosDespachoDTO tmp = new RollosDespachoDTO();
                tmp.INVENTSERIALID = element.INVENTSERIALID;
                tmp.InventTransID = element.InventTransID;

                var RolloAX = await getRolloDespachoAX(tmp.InventTransID, tmp.INVENTSERIALID);

                int index = rollos.FindIndex(x => x.INVENTSERIALID == tmp.INVENTSERIALID);
                tmp.Config = RolloAX[0].Config;
                tmp.Color = RolloAX[0].Color;
                tmp.LibrasYardas = RolloAX[0].LibrasYardas;
                tmp.inventBatchId = RolloAX[0].inventBatchId;
                tmp.NameAlias = RolloAX[0].NameAlias;
                data.Add(tmp);               

            }

            var ordenar = data.Where(x => x.NameAlias != null && x.inventBatchId != null)
                            .OrderBy(x => x.inventBatchId).ThenBy(x=> x.NameAlias)
                            .Select(x => new RollosDespachoDTO{
                                Color = x.Color,
                                Config = x.Config,
                                inventBatchId = x.inventBatchId,
                                INVENTSERIALID = x.INVENTSERIALID,
                                InventTransID = x.InventTransID,
                                LibrasYardas = x.LibrasYardas,
                                NameAlias = x.NameAlias
                            });

            int cont = 1;
            decimal totalRolloLY = 0;
            foreach (var element in ordenar)
            {
                htmlCorreo += @"<tr>
                                <td>" + cont + @"</td>
                                <td>" + element.INVENTSERIALID + @"</td>
                                <td>" + element.NameAlias + @"</td>
                                <td>" + element.inventBatchId + @"</td>
                                <td>" + element.Color + @"</td>
                                <td>" + element.Config + @"</td>
                                <td>1</td>
                                <td>" + element.LibrasYardas + @"</td>      
                            </tr>";
                cont++;
                totalRolloLY += Convert.ToDecimal(element.LibrasYardas);
            }

            htmlCorreo += @"</tbody>
                            <tfoot>
                                <tr>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td>Total</td>
                                    <td>" + totalRolloLY + @"</td>      
                             </tr>
                            </tfoot>
                        </table></body></html>";

            //agrupar por color y configuracion la catnidad de rollos
            var resumen = data
                    .GroupBy(x => new { x.Color, x.Config })
                    .Select(g => new 
                    {
                        Color = g.Key.Color,
                        Config = g.Key.Config,
                        Cantidad = g.Sum(x => 1),
                        Total = g.Sum(a => Convert.ToDecimal(a.LibrasYardas))

                    });

            //colocar encabezado de la tabla
            despacho += @"<table style='width: 100%'>
                               <thead> 
                                 <tr> 
                                   <th colspan = '4'> Detalle </th>  
                                  </tr>  
                                  <tr>  
                                    <th> Color/Referencia </th>  
                                    <th> Ancho </th>  
                                    <th> Cantidad </th>  
                                    <th> Libras/Yardas </th>  
                                  </tr>  
                                  </thead>  
                                <tbody> ";
            int totalRollo = 0;
            decimal totalLY = 0;
            foreach(var element in resumen)
            {
                totalRollo += element.Cantidad;
                totalLY += element.Total;
                despacho += @"<tr>
                              <td>"+element.Color+@"</td>
                              <td>"+element.Config+@"</td>
                              <td>"+element.Cantidad+ @"</td>
                              <td>"+element.Total + @"</td>      
                            </tr>";
            }

            //linea que muestra el total de rollos enviados
            despacho += @"</tbody>
                            <tfoot>
                                <tr>
                                  <td></td>
                                  <td>Total</td>
                                  <td>" + totalRollo + @"</td>
                                  <td>"+totalLY+ @"</td>      
                             </tr>
                            </tfoot>
                        </table>";


           

            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(VariablesGlobales.Correo);

                var correos = await getCorreosDespacho();

                foreach( IM_WMS_Correos_Despacho correo in correos)
                {
                    mail.To.Add(correo.Correo);
                }
                mail.Subject = "Despacho No."+ DespachoID.ToString().PadLeft(8, '0');
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
            catch(Exception err)
            {
                string error = err.ToString();
            }


            //colocar la tabla

            despacho += @"<p>
      	                    <strong>Observaciones:</strong>
      	                    <div class='observacion' >
                              <br><br><br>
                            </div>
                        </p>
                         <br>
                        <br>
                        <br>
                        <div class='container'>
                            <div>
                                <p>____________________</p>
                                <p align = 'center'> Despachado </p>
                            </div>
                            <div>
                                <p> ____________________ </p>
                                <p align='center'>Motorista</p>
                            </div>
                            <div>
                                <p>____________________</p>
                                <p align = 'center'> Recibe </p>
                            </div>
                        </div>
                    </body>
                    </html> ";

            

           return despacho;
        }
        public async Task<List<EncabezadoNotaDespachoDTO>> getEncabezadoDespacho(string empleado, string recid)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@empleado", empleado),
                new SqlParameter("@recid", recid)
            };

            List<EncabezadoNotaDespachoDTO> result = await executeProcedure.ExecuteStoredProcedureList<EncabezadoNotaDespachoDTO>("[IM_WMS_Encabezado_Despacho]", parametros);

            return result;
        }
        public async Task<List<RollosDespachoDTO>> getRolloDespacho(int id)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@ID", id)
            };

            List<RollosDespachoDTO> result = await executeProcedure.ExecuteStoredProcedureList<RollosDespachoDTO>("[IM_WMS_RolloDespacho]", parametros);

            return result;           
        }
        public async Task<List<RollosDespachoDTO>> getRolloDespachoAX(string InventTransID,string INVENTSERIALID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@TRANSFERID", InventTransID),
                new SqlParameter("@INVENTSERIALID", INVENTSERIALID)
            };

            List<RollosDespachoDTO> result = await executeProcedure.ExecuteStoredProcedureList<RollosDespachoDTO>("[IM_WMS_NotaDespachoDetalleAX]", parametros);

            return result;         
        }     
        public async Task<List<IM_WMS_Correos_Despacho>> getCorreosDespacho()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>{};

            List<IM_WMS_Correos_Despacho> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Correos_Despacho>("[IM_WMS_ObtenerCorreosDespachotela]", parametros);

            return result;
        }
        public async Task<List<RolloDespachoDTO>> getRollosDespacho(int despachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter> 
            {
                new SqlParameter("@DespachoId", despachoID)
            };

            List<RolloDespachoDTO> result = await executeProcedure.ExecuteStoredProcedureList<RolloDespachoDTO>("[IM_ObtenerRollosDespacho]", parametros);

            return result;           
        }     
        //Reduccion de cajas
        public async Task<List<LineasDTO>> GetLineasReducionCajas(string IMBOXCODE)
        {

            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@IMBOXCODE",IMBOXCODE)
            };

            List<LineasDTO> result = await executeProcedure.ExecuteStoredProcedureList<LineasDTO>("[IM_ObtenerReduccionCajas]", parametros);

            return result;
        }
        public async Task<string> getImprimirEtiquetaReduccion(string IMBOXCODE, string ubicacion, string empacador, string PRINT)
        {
            DateTime hoy = DateTime.Now;

            var empleado = await getNombreEmpleado(empacador);
            var data = await GetDatosEtiquetaReduccion(IMBOXCODE);

            var groupData = new Dictionary<string, List<EtiquetaReduccionDTO>>();
            int totalUnidades = 0;

            foreach (var element in data)
            {
                totalUnidades += element.QTY;
                var key = $"{element.ITEMID}-{element.INVENTCOLORID}";

                if (!groupData.ContainsKey(key))
                {
                    groupData[key] = new List<EtiquetaReduccionDTO>();
                }

                groupData[key].Add(element);
            }

            var groupArray = groupData.Select(x => new EtiquetaReduccionGroupDTO
            {
                key = x.Key,
                items = x.Value
            }).ToArray();

            string encabezado = @"^XA^FO700,50^FWN^A0R,30,30^FDFecha: "+hoy+@"^FS^FO670,50^A0R,30,30^FDEmpacador: "+empleado.Nombre+@"^FS";

            string pie = @"^A0R,30,30^BY2,2,100^FO50,50^BC^FD"+IMBOXCODE+ @"^FS^FO50,700^A0R,40,40^FDUbicacion: "+ubicacion+ @"^FS^XZ";

            int cont = 0;
            string etiqueta = "";
            int position = 600;
            int subtotal = 0;

            foreach(var element in groupArray)
            {
                if(cont == 0)
                {
                    etiqueta = encabezado;
                    subtotal = 0;
                    position = 600;
                }
                if (element.items[0].NameAlias.Substring(0, 2) == "MB")
                {
                    etiqueta += $"^FO{position},50^A0R,45,45^FD{element.items[0].NameAlias} *{element.items[0].INVENTCOLORID}^FS";
                    position -= 45;
                    etiqueta += $"^FO{position},50^A0R,45,45^FD{element.items[0].ITEMID}^FS";
                    position -= 45;
                }
                else
                {
                    etiqueta += $"^FO{position},50^A0R,45,45^FD{element.items[0].ITEMID} *{element.items[0].INVENTCOLORID}^FS";
                    position -= 45;
                }
                
                etiqueta += $"^FO{position},50^A0R,45,45^FDTalla: ";

                element.items.ForEach( x=>
                {
                    for(int i = 1; i <= 5 - x.INVENTSIZEID.Length; i++)
                    {
                        etiqueta += "_";
                        
                    }
                    etiqueta += x.INVENTSIZEID;
                });
                etiqueta += "^FS";
                position -= 45;
                etiqueta += $"^FO{position},50^A0R,45,45^FDQTY: ";
                int totalLinea = 0;
                element.items.ForEach(x =>
                {
                    for (int i = 1; i <= 5 - x.QTY.ToString().Length; i++)
                    {
                        etiqueta += "_";

                    }
                    etiqueta += x.QTY;
                    totalLinea += x.QTY;
                });
                etiqueta += $"={totalLinea}^FS";
                subtotal += totalLinea;
                position -= 70;
                cont++;
                if(cont== 3)
                {
                    etiqueta += $"^FO100,700^A0R,40,40^FDTotal: {subtotal} / {totalUnidades}^FS";
                    etiqueta += pie;
                    cont = 0;

                    try
                    {
                        using (TcpClient client = new TcpClient(PRINT, 9100))
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);
                                stream.Write(bytes, 0, bytes.Length);
                            }
                        }                        
                    }
                    catch (Exception err)
                    {
                        return err.ToString();
                    };
                }
                
                
            }
            if (cont != 3 && cont != 0)
            {
                etiqueta += $"^FO100,700^A0R,40,40^FDTotal: {subtotal} / {totalUnidades}^FS";
                etiqueta += pie;
                cont = 0;

                try
                {
                    using (TcpClient client = new TcpClient(PRINT, 9100))
                    {
                        using (NetworkStream stream = client.GetStream())
                        {
                            byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);
                            stream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                catch (Exception err)
                {
                    return err.ToString();
                };
            }

                return "OK";


        }
        public async Task<EmpleadoDTO> getNombreEmpleado(string empleado)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter> 
            {
                new SqlParameter("@empleado", empleado)
            };

            EmpleadoDTO result = await executeProcedure.ExecuteStoredProcedure<EmpleadoDTO>("[IM_ObtenerNombreEmpleado]", parametros);

            return result;            
        }       
        public async Task<List<EtiquetaReduccionDTO>> GetDatosEtiquetaReduccion(string IMBoxCode)
        {

            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@IMBOXCODE", IMBoxCode)
            };

            List<EtiquetaReduccionDTO> result = await executeProcedure.ExecuteStoredProcedureList<EtiquetaReduccionDTO>("[IM_WMS_Obtener_Etiqueta_Reduccion]", parametros);

            return result;
        }
        //Despacho  PT
        public async Task<List<IM_WMS_Insert_Boxes_Despacho_PT_DTO>> GetInsert_Boxes_Despacho_PT(string ProdID, string userCreated, int Box)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@ProdID", ProdID),
                new SqlParameter("@userCreated", userCreated),
                new SqlParameter("@Box", Box)
            };

            List<IM_WMS_Insert_Boxes_Despacho_PT_DTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Insert_Boxes_Despacho_PT_DTO>("[IM_WMS_Insert_Boxes_Despacho_PT]", parametros);

            return result;            
        }
        public async Task<List<IM_WMS_Picking_Despacho_PT_DTO>> GetPicking_Despacho_PT(int Almacen)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Almacen", Almacen)
            };

            List<IM_WMS_Picking_Despacho_PT_DTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Picking_Despacho_PT_DTO>("[IM_WMS_Picking_Despacho_PT]", parametros);

            return result;            
        }
        public async Task<List<IM_WMS_Get_EstatusOP_PT_DTO>> get_EstatusOP_PT(int almacen)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@inventlocationid", almacen)
            };

            List<IM_WMS_Get_EstatusOP_PT_DTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Get_EstatusOP_PT_DTO>("[IM_WMS_Get_EstatusOP]", parametros);

            return result;           
        }
        public async Task<IM_WMS_Insert_Estatus_Unidades_OP_DTO> GetM_WMS_Insert_Estatus_Unidades_OPs(IM_WMS_Insert_Estatus_Unidades_OP_DTO data)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@prodID", data.ProdID),
                new SqlParameter("@size", data.size),
                new SqlParameter("@costura1", data.Costura1),
                new SqlParameter("@textil1", data.Textil1),
                new SqlParameter("@costura2", data.Costura2),
                new SqlParameter("@textil2", data.Textil2),
                new SqlParameter("@usuario", data.usuario)
            };

            IM_WMS_Insert_Estatus_Unidades_OP_DTO result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_Insert_Estatus_Unidades_OP_DTO>("[IM_WMS_Insert_Estatus_Unidades_OP]", parametros);

            return result;
        }
        public async Task<IM_WMS_Crear_Despacho_PT> GetCrear_Despacho_PT(string driver, string truck, string userCreated, int almacen)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@driver", driver),
                new SqlParameter("@truck", truck),
                new SqlParameter("@userCreated", userCreated),
                new SqlParameter("@almacen", almacen)
            };

            IM_WMS_Crear_Despacho_PT result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_Crear_Despacho_PT>("[IM_WMS_Crear_Despacho_PT]", parametros);

            return result;
        }     
        public async Task<List<IM_WMS_Get_Despachos_PT_DTO>> Get_Despachos_PT_DTOs(string estado, int almacen, int DespachoId)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Estado", estado),
                new SqlParameter("@almacen", almacen),
                new SqlParameter("@DespachoID", DespachoId)
            };

            List<IM_WMS_Get_Despachos_PT_DTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Get_Despachos_PT_DTO>("[IM_WMS_Get_Despachos_PT]", parametros);

            return result;        
        }
        public async Task<IM_WMS_Packing_DespachoPTDTO> GetPacking_DespachoPT(string ProdID, string userCreated, int Box,int DespachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@prodID", ProdID),
                new SqlParameter("@box", Box),
                new SqlParameter("@user", userCreated),
                new SqlParameter("@DespachoID", DespachoID)
            };

            IM_WMS_Packing_DespachoPTDTO result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_Packing_DespachoPTDTO>("[IM_WMS_Packing_DespachoPT]", parametros);

            return result;            
        }
        public async Task<List<IM_WMS_Picking_Despacho_PT_DTO>> GetDetalleDespachoPT(int DespachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", DespachoID)
            };

            List<IM_WMS_Picking_Despacho_PT_DTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Picking_Despacho_PT_DTO>("[IM_WMS_ObtenerDetalleDespachoPT]", parametros);

            return result;            
        }
        public async Task<List<DiariosAbiertosDTO>> getObtenerDiarioTransferir(string user, string filtro)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@user",user),
                new SqlParameter("@filtro", filtro)
            };

            List<DiariosAbiertosDTO> result = await executeProcedure.ExecuteStoredProcedureList<DiariosAbiertosDTO>("[IM_WMS_Obtener_Diarios_Transferir]", parametros);

            return result;
        }

        //obtenre informacion del detalle que se colocara en el archivo de excel Despacho PT Contratistas
        public async Task<IM_WMS_ObtenerSecuencia_PL_PT_DTO> getSecuencia_PL_PT(int despachoID, string user,int almacenTo)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", despachoID),
                new SqlParameter("@user", user),
                new SqlParameter("@AlmacenTO", almacenTo)


            };

            IM_WMS_ObtenerSecuencia_PL_PT_DTO result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_ObtenerSecuencia_PL_PT_DTO>("[IM_WMS_ObtenerSecuencia_PL_PT]", parametros);

            return result;
        }
        public async Task<List<IM_WMS_Detalle_Despacho_Excel>> getDetalle_Despacho_Excel(int DespachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", DespachoID)
            };

            List<IM_WMS_Detalle_Despacho_Excel> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Detalle_Despacho_Excel>("[IM_WMS_Detalle_Despacho_Excel]", parametros);

            return result;
        }
        public async Task<IM_WMS_EnviarDespacho> Get_EnviarDespachos(int DespachoID,string user, int cajasSegundas, int cajasTerceras)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", DespachoID),
                new SqlParameter("@cajasSegundas", cajasSegundas),
                new SqlParameter("@cajasTerceras", cajasTerceras)
            };

            IM_WMS_EnviarDespacho response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_EnviarDespacho>("[IM_WMS_EnviarDespacho]", parametros);           

            if(response.Descripcion == "Enviado")
            {
                
                var Despacho = await getDetalle_Despacho_Excel(DespachoID);
              

                var almacenes = Despacho.Select(x => x.InventLocation).Distinct().ToList();
                var encabezado = await GetEncabezadoDespachoExcel(user);
                int cont1 = 0;
                foreach (var almacen in almacenes)
                {
                   
                    var data = Despacho.Where(x=> x.InventLocation == almacen).ToList();

                    if (data.Count() > 0)
                    {
                        if (cont1 == 0)
                        {
                            var tmp = new IM_WMS_Detalle_Despacho_Excel();
                            tmp.CajasSegundas = cajasSegundas;
                            tmp.CajasTerceras = cajasTerceras;
                            data.Add(tmp);
                            cont1++;
                        }
                        else {
                            cont1++;
                        }
                        try
                        {
                            Byte[] fileContents;
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage package = new ExcelPackage())
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Hoja1");
                                int cont = 1;
                                int fila = 12;

                                var rangeEncabezado = worksheet.Cells[2, 1, 5, 1];
                                rangeEncabezado.Style.Font.Size = 20;
                                rangeEncabezado.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                rangeEncabezado.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                                //encabezado del libro
                               
                                for (int x = 2; x <= 5; x++)
                                {
                                    var rangeMerge = worksheet.Cells[x, 1, x, 26];
                                    rangeMerge.Merge = true;
                                    string texto = "";
                                    switch (x)
                                    {
                                        case 2:
                                            texto = encabezado.NAME;
                                            break;
                                        case 3:
                                            texto = "Direccion: " + encabezado.STREET;
                                            break;
                                        case 4:
                                            texto = "Telefono: " + encabezado.LOCATOR;
                                            break;
                                        case 5:
                                            texto = "RTN: " + encabezado.ORGNUMBER;
                                            break;
                                    }
                                    worksheet.Cells[x, 1].Value = texto;
                                }


                                worksheet.Row(fila).Height = 57;
                                var range = worksheet.Cells[12, 1, 12, 30];
                                range.Style.Font.Size = 20;
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.WrapText = true;

                                range = worksheet.Cells[8, 1, 11, 7];
                                range.Style.Font.Size = 20;
                                worksheet.Cells[8, 2].Value = "Cliente:";
                                worksheet.Cells[8, 3].Value = "INTERMODA";
                                worksheet.Cells[8, 6].Value = "Entrega A:";
                                worksheet.Cells[8, 7].Value = data[0].InventLocation;
                                worksheet.Cells[9, 2].Value = "Fecha:";
                                worksheet.Cells[9, 3].Value = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
                                worksheet.Cells[9, 6].Value = "Packing List: ";
                                var packing = await getSecuencia_PL_PT(DespachoID, user,cont1);
                                worksheet.Cells[9, 7].Value = packing.Secuencia + "."+packing.Anio;

                                //Encabezado de la tabla
                                worksheet.Cells[fila, 1].Value = "#";
                                worksheet.Column(1).Width = 9.33;

                                worksheet.Cells[fila, 2].Value = "Base";
                                worksheet.Column(2).Width = 12.56;

                                worksheet.Cells[fila, 3].Value = "Codigo de Articulo";
                                worksheet.Column(3).Width = 37.11;

                                worksheet.Cells[fila, 4].Value = "Nombre Producto";
                                worksheet.Column(4).Width = 27.67;

                                //Agregando si es Lateral o Tubular
                                worksheet.Cells[fila, 5].Value = "L/T";
                                worksheet.Column(5).Width = 6.67;

                                worksheet.Cells[fila, 6].Value = "Nombre Color";
                                worksheet.Column(6).Width = 43.11;

                                worksheet.Cells[fila, 7].Value = "Talla";
                                worksheet.Column(7).Width = 11.67;

                                worksheet.Cells[fila, 8].Value = "Numero Orden Produccion";
                                worksheet.Column(8).Width = 38.22;

                                worksheet.Cells[fila, 9].Value = "PC";
                                worksheet.Column(9).Width = 43.11;

                                worksheet.Cells[fila, 10].Value = "Unidades Planificadas";
                                worksheet.Column(10).Width = 21.67;

                                worksheet.Cells[fila, 11].Value = "Unidades Cortadas";
                                worksheet.Column(11).Width = 15.11;

                                worksheet.Cells[fila, 12].Value = "Und. De Primeras";
                                worksheet.Column(12).Width = 12.56;

                                worksheet.Cells[fila - 1, 13].Value = "Irregulares";
                                range = worksheet.Cells[fila - 1, 13, fila - 1, 14];
                                range.Merge = true;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                worksheet.Cells[fila, 13].Value = "Costura1";
                                worksheet.Column(13).Width = 10.56;

                                worksheet.Cells[fila, 14].Value = "Textil1";
                                worksheet.Column(14).Width = 8.56;

                                worksheet.Cells[fila - 1, 15].Value = "Terceras";
                                range = worksheet.Cells[fila - 1, 15, fila - 1, 16];
                                range.Merge = true;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);


                                worksheet.Cells[fila, 15].Value = "Costura2";
                                worksheet.Column(15).Width = 11.89;

                                worksheet.Cells[fila, 16].Value = "Textil2";
                                worksheet.Column(16).Width = 9.89;

                                worksheet.Cells[fila, 17].Value = "Total de Unidades por Orden";
                                worksheet.Column(17).Width = 18.56;

                                worksheet.Cells[fila, 18].Value = "Dif Prd vrs Plan";
                                worksheet.Column(18).Width = 13.67;

                                worksheet.Cells[fila, 19].Value = "Dif Cortado vrs Exportado";
                                worksheet.Column(19).Width = 20.11;

                                worksheet.Cells[fila - 1, 20].Value = "% de Irregular y Tercera";
                                range = worksheet.Cells[fila - 1, 20, fila - 1, 21];
                                range.Merge = true;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range = worksheet.Cells[fila - 1, 20, fila - 1, 25];
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                                worksheet.Cells[fila, 20].Value = "Por Costura";
                                worksheet.Column(20).Width = 14.78;

                                worksheet.Cells[fila, 21].Value = "Por Textil";
                                worksheet.Column(21).Width = 12.78;

                                worksheet.Cells[fila, 22].Value = "Irregulares Permitidos del 1% por Costura";
                                worksheet.Column(22).Width = 15.56;

                                worksheet.Cells[fila, 23].Value = "Irregulares arriba del 1% Por Costura";
                                worksheet.Column(23).Width = 15.56;

                                worksheet.Cells[fila, 24].Value = "Irregulares Permitidas del 1% por Defecto Textil";
                                worksheet.Column(24).Width = 15.56;

                                worksheet.Cells[fila, 25].Value = "Irregulares arriba del 1% por Defecto Textil";
                                worksheet.Column(25).Width = 15.56;

                                worksheet.Cells[fila, 26].Value = "Cajas de Primeras";
                                worksheet.Column(26).Width = 17.56;

                                worksheet.Cells[fila, 27].Value = "Cajas de Segundas";
                                worksheet.Column(27).Width = 17.56;

                                worksheet.Cells[fila, 28].Value = "Cajas de Terceras";
                                worksheet.Column(28).Width = 17.56;

                                worksheet.Cells[fila, 29].Value = "Total de Docenas";
                                worksheet.Column(29).Width = 14.78;

                                worksheet.Cells[fila, 30].Value = "Programa";
                                worksheet.Column(30).Width = 20.22;


                                fila++;

                                data.ForEach(element =>
                                {
                                    worksheet.Row(fila).Height = 36;
                                    worksheet.Cells[fila, 1].Value = cont;
                                    worksheet.Cells[fila, 2].Value = element.Base;
                                    worksheet.Cells[fila, 3].Value = element.ItemID;
                                    worksheet.Cells[fila, 4].Value = element.Nombre;
                                    worksheet.Cells[fila, 5].Value = element.Tl;
                                    worksheet.Cells[fila, 6].Value = element.Color;
                                    worksheet.Cells[fila, 7].Value = element.Size;
                                    worksheet.Cells[fila, 8].Value = element.ProdID;
                                    worksheet.Cells[fila, 9].Value = element.InventRefId;
                                    worksheet.Cells[fila, 10].Value = element.Planificado;
                                    worksheet.Cells[fila, 11].Value = element.Cortado;
                                    worksheet.Cells[fila, 12].Value = element.Primeras;
                                    worksheet.Cells[fila, 13].Value = element.Costura1;
                                    worksheet.Cells[fila, 14].Value = element.Textil1;
                                    worksheet.Cells[fila, 15].Value = element.Costura2;
                                    worksheet.Cells[fila, 16].Value = element.Textil2;
                                    worksheet.Cells[fila, 17].Value = element.TotalUnidades;
                                    worksheet.Cells[fila, 18].Value = element.DifPrdVrsPlan;
                                    worksheet.Cells[fila, 19].Value = element.DifCortVrsExport;
                                    worksheet.Cells[fila, 20].Value = Math.Round(element.PorCostura, 2) / 100;
                                    worksheet.Cells[fila, 20].Style.Numberformat.Format = "0.00%";
                                    worksheet.Cells[fila, 21].Value = Math.Round(element.PorTextil, 2) / 100;
                                    worksheet.Cells[fila, 21].Style.Numberformat.Format = "0.00%";
                                    worksheet.Cells[fila, 22].Value = element.Irregulares1PorcCostura;
                                    worksheet.Cells[fila, 23].Value = element.IrregularesCobrarCostura;
                                    worksheet.Cells[fila, 24].Value = element.Irregulares1PorcTextil;
                                    worksheet.Cells[fila, 25].Value = element.IrregularesCobrarTextil;
                                    worksheet.Cells[fila, 26].Value = element.Cajas;
                                    worksheet.Cells[fila, 27].Value = element.CajasSegundas;
                                    worksheet.Cells[fila, 28].Value = element.CajasTerceras;
                                    worksheet.Cells[fila, 29].Value = element.TotalDocenas;
                                    worksheet.Cells[fila, 30].Value = element.Programa;


                                    fila++;
                                    cont++;
                                });

                                var range2 = worksheet.Cells[13, 1, fila, 30];
                                range2.Style.Font.Size = 20;
                                range2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range2.Style.WrapText = true;

                                fila--;
                                var rangeTable = worksheet.Cells[12, 1, fila, 30];
                                var table = worksheet.Tables.Add(rangeTable, "MyTable");
                                table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;

                                int col = 12;
                                for (char c = 'L'; c <= 'Z'; c++)
                                {
                                    worksheet.Cells[fila + 1, col].Formula = "sum(" + c + "13:" + c + fila + ")";
                                    col++;
                                }

                                for (char c = 'A'; c <= 'C'; c++)
                                {
                                    worksheet.Cells[fila + 1, col].Formula = "sum(A" + c + "13:A" + c + fila + ")";
                                    col++;
                                }

                                range = worksheet.Cells[fila + 1, 11, fila + 1, col - 1];
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                //pie de pagina 
                                string fontSizeCode = "&20";
                                worksheet.HeaderFooter.OddFooter.LeftAlignedText = fontSizeCode + "___________________________\nFIRMA DEL AUDITOR INTERMODA";
                                worksheet.HeaderFooter.OddFooter.CenteredText = fontSizeCode + "___________________________\nFIRMA DEL TRANSPORTISTA";
                                worksheet.HeaderFooter.OddFooter.RightAlignedText = fontSizeCode + "___________________________\nFIRMA DE CONTROL DE INVENTARIO";
                                fila += 3;

                                range = worksheet.Cells[fila, 27, fila, 28];
                                range.Merge = true;
                                worksheet.Cells[fila, 27].Value = "Total Cajas Primera";
                                worksheet.Cells[fila, 29].Value = data.Sum(x => x.Cajas);

                                fila++;
                                range = worksheet.Cells[fila, 27, fila, 28];
                                range.Merge = true;
                                worksheet.Cells[fila, 27].Value = "Total Cajas Irregulares";
                                worksheet.Cells[fila, 29].Value = cajasSegundas;

                                fila++;
                                range = worksheet.Cells[fila, 27, fila, 28];
                                range.Merge = true;
                                worksheet.Cells[fila, 27].Value = "Total Cajas Terceras";
                                worksheet.Cells[fila, 29].Value = cajasTerceras;

                                fila++;
                                range = worksheet.Cells[fila, 27, fila, 28];
                                range.Merge = true;
                                worksheet.Cells[fila, 27].Value = "Total Cajas";
                                worksheet.Cells[fila, 29].Value = data.Sum(x => x.Cajas) + cajasTerceras + cajasSegundas;

                                range = worksheet.Cells[fila - 3, 27, fila, 29];
                                range.Style.Font.Size = 20;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Color.SetColor(Color.Black);

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Bottom.Color.SetColor(Color.Black);

                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Color.SetColor(Color.Black);

                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Color.SetColor(Color.Black);

                                fileContents = package.GetAsByteArray();
                                try
                                {
                                    MailMessage mail = new MailMessage();

                                    mail.From = new MailAddress(VariablesGlobales.Correo);

                                    var correos = await getCorreosDespachoPT(user);

                                    foreach (IM_WMS_Correos_DespachoPTDTO correo in correos)
                                    {
                                        mail.To.Add(correo.Correo);
                                    }

                                    //mail.To.Add("bavila@intermoda.com.hn");
                                    mail.Subject = "Despacho PT No." + DespachoID.ToString().PadLeft(8, '0') + ", " + packing.Secuencia + "." + packing.Anio;
                                    mail.IsBodyHtml = false;

                                    mail.Body = "Despacho de PT desde " + encabezado.NAME + " a " + data[0].InventLocation;

                                    using (MemoryStream ms = new MemoryStream(fileContents))
                                    {
                                        DateTime date = DateTime.Now;
                                        string fecha = date.Day + "_" + date.Month + "_" + date.Year;
                                        Attachment attachment = new Attachment(ms, "Despacho N" + DespachoID + "-" + fecha + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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
                                    response.Descripcion = err.ToString();
                                }


                            }
                        }
                        catch (Exception err)
                        {
                            response.Descripcion = err.ToString();
                        }
                    }
                }

                
            }

            return response;
        }
        public async Task<IM_WMS_EncabezadoDespachoExcelDTO> GetEncabezadoDespachoExcel(string user)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@user", user)
            };

            IM_WMS_EncabezadoDespachoExcelDTO response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_EncabezadoDespachoExcelDTO>("[IM_WMS_EncabezadoDespachoExcel]", parametros);
            return response;
        }  
        public async Task<List<IM_WMS_Get_Despachos_PT_DTO>> GetDespachosEstado(string estado)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@estado", estado)
            };

            List<IM_WMS_Get_Despachos_PT_DTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Get_Despachos_PT_DTO>("[IM_WMS_DespachosEstado]", parametros);

            return response;            
        }
        public async Task<List<IM_WMS_ObtenerDespachoPTEnviados>> GetObtenerDespachoPTEnviados(int despachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", despachoID)
            };

            List<IM_WMS_ObtenerDespachoPTEnviados> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_ObtenerDespachoPTEnviados>("[IM_WMS_ObtenerDespachoPTEnviados]", parametros);

            return response;
        }
        public async Task<IM_WMS_DespachoPT_RecibirDTO> GetRecibir_DespachoPT(string ProdID, string userCreated, int Box)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@prodID", ProdID),
                new SqlParameter("@box", Box),
                new SqlParameter("@user", userCreated)
            };

            IM_WMS_DespachoPT_RecibirDTO response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_DespachoPT_RecibirDTO>("[IM_WMS_DespachoPT_Recibir]", parametros);

            return response;
        }
        public async Task<List<IM_WMS_DespachoPT_CajasAuditarDTO>> getCajasAuditar(int despachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", despachoID)
            };

            List<IM_WMS_DespachoPT_CajasAuditarDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_DespachoPT_CajasAuditarDTO>("[IM_WMS_DespachoPT_CajasAuditar]", parametros);

            return response;           
        }
        public async Task<List<IM_WMS_Detalle_Auditoria_CajaDTO>> getDetalleAuditoriaCaja(string ProdID, int box)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@ProdID", ProdID),
                new SqlParameter("@Box", box)
            };

            List<IM_WMS_Detalle_Auditoria_CajaDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Detalle_Auditoria_CajaDTO>("[IM_WMS_Detalle_Auditoria_Caja]", parametros);

            return response;            
        }
        public async Task<List<IM_WMS_Get_Despachos_PT_DTO>> getDespachosPTEstado(int DespachoID)
        {

            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", DespachoID)
            };

            List<IM_WMS_Get_Despachos_PT_DTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Get_Despachos_PT_DTO>("[IM_WMS_DespachosPT]", parametros);

            return response;
        }
        public async Task<List<IM_WMS_Consulta_OP_DetalleDTO>> getConsultaOPDetalle(string Prodcutsheetid)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Prodcutsheetid", Prodcutsheetid)
            };

            List<IM_WMS_Consulta_OP_DetalleDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Consulta_OP_DetalleDTO>("[IM_WMS_Consulta_OP_Detalle]", parametros);

            return response;
        }      
        public async Task<List<IM_WMS_ConsultaOP_OrdenesDTO>> getConsultaOpOrdenes(string ProdCutSheetID, int DespachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Prodcutsheetid", ProdCutSheetID),
                new SqlParameter("@DespachoID", DespachoID)
            };

            List<IM_WMS_ConsultaOP_OrdenesDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_ConsultaOP_OrdenesDTO>("[IM_WMS_ConsultaOP_Ordenes]", parametros);

            return response;
        }
        public async Task<List<IM_WMS_Consulta_OP_Detalle_CajasDTO>> getConsultaOPDetalleCajas(string ProdCutSheetID, int DespachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Prodcutsheetid", ProdCutSheetID),
                new SqlParameter("@DespachoID", DespachoID)
            };

            List<IM_WMS_Consulta_OP_Detalle_CajasDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Consulta_OP_Detalle_CajasDTO>("[IM_WMS_Consulta_OP_Detalle_Cajas]", parametros);

            return response;         
        }
        public async Task<List<IM_WMS_Correos_DespachoPTDTO>> getCorreosDespachoPT(string user)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@user", user)
            };

            List<IM_WMS_Correos_DespachoPTDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Correos_DespachoPTDTO>("[IM_WMS_Correos_DespachoPTDTO]", parametros);

            return response;            
        }
        public async Task<IM_WMS_EnviarDiarioTransferirDTO> getEnviarDiarioTransferir(string JournalID, string userID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@JournalID", JournalID),
                new SqlParameter("@user", userID)
            };

            IM_WMS_EnviarDiarioTransferirDTO response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_EnviarDiarioTransferirDTO>("[IM_WMS_EnviarDiarioTransferir]", parametros);

            var detalle = await getDetalle_Diario_Transferir_Correo(JournalID);
            string html = @"<!DOCTYPE html>
                            <html lang='en'>
                            <head>
                              <meta charset = 'UTF-8'/>
                               <style>
                                    h3,h4,td{
                                        text-align: center;
                                    }
                                    table,tr,th,td{
                                        border: 1px solid black;
                                        border-collapse: collapse;
                                    }
                                    .container{
                                        display: flex;
                                        justify-content: space-between;
                                    }
                                    .observacion {
                                        border-style: solid;
                                        border-width: 2px;
                                    }
                                </style>
                            </head>

                            <body>
                              <h3> Diario Transferir</h3>
   
                                 <h4>"+ JournalID + @"</h4>";
            //colocar en encabezado el almacen hacia donde va
            var encabezado = await GetEncabezadoTransferir(JournalID);

            //obtener encabezado
            html += @"
                     <p>           
  	                    <strong>Desde Almacen: </strong>" + encabezado.INVENTLOCATIONID + @" <br>
                        <strong>Hasta Almacen: </strong>" + encabezado.IM_INVENTLOCATIONID_TO + @" <br>
                        <strong>Asignado a: </strong>" + encabezado.PERSONNELNUMBER + @" <br>
                        
                      </p>
                   ";
            //tabla

            html +=@"<table style='width: 100%'>
                               <thead> 
                                 <tr> 
                                   <th colspan = '7'> Detalle </th>  
                                  </tr>  
                                  <tr>  
                                    <th> Articulo </th> 
                                    <th> Color </th> 
                                    <th> Talla </th> 
                                    <th> QTY </th> 
                                      
                                  </tr>  
                                  </thead>  
                                <tbody> ";
            int total = 0;

            foreach (var element in detalle)
            {
                total += element.QTY;
                html += @"<tr>
                              <td>" + element.ITEMID + @"</td>
                              <td>" + element.INVENTCOLORID + @"</td>
                              <td>" + element.INVENTSIZEID + @"</td>
                              <td>" + element.QTY + @"</td>      
                            </tr>";
            }

            //linea que muestra el total de rollos enviados
            html += @"</tbody>
                            <tfoot>
                                <tr>
                                  <td></td>
                                  <td></td>
                                  <td>Total</td>
                                  <td>" + total + @"</td>      
                             </tr>
                            </tfoot>
                        </table></body></html>";



            try
            {
                MailMessage mail = new MailMessage();


                mail.From = new MailAddress(VariablesGlobales.Correo);

                var correos = await getCorreosDespachoTransferir();

                foreach (IM_WMS_Correos_Despacho correo in correos)
                {
                    mail.To.Add(correo.Correo);
                }
                mail.Subject = "Diario Transferir "+JournalID + " desde almacen "+ encabezado.INVENTLOCATIONID + " a "+ encabezado.IM_INVENTLOCATIONID_TO;
                mail.IsBodyHtml = true;

                mail.Body = html;

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

            }
            return response;
        }
        public async Task<IM_Encabezado_Diario_TransferirDTO> GetEncabezadoTransferir(string journalID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@JournalID", journalID)
            };

            IM_Encabezado_Diario_TransferirDTO response = await executeProcedure.ExecuteStoredProcedure<IM_Encabezado_Diario_TransferirDTO>("[IM_Encabezado_Diario_Transferir]", parametros);

            return response;            
        }       
        public async Task<List<IM_WMS_Correos_Despacho>> getCorreosDespachoTransferir()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {                
            };

            List<IM_WMS_Correos_Despacho> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Correos_Despacho>("[IM_WMS_Correos_Transferirsp]", parametros);

            return response;
        }
        public async Task<List<IM_WMS_Detalle_Diario_Transferir_CorreoDTO>> getDetalle_Diario_Transferir_Correo(string JournalID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@JournalID", JournalID)
            };

            List<IM_WMS_Detalle_Diario_Transferir_CorreoDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Detalle_Diario_Transferir_CorreoDTO>("[IM_WMS_Detalle_Diario_Transferir_Correo]", parametros);

            return response;            
        }

        
    }
}