using Azure;
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
using Core.DTOs.Reduccion_Cajas;
using Core.DTOs.TrackingPedidos;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class WMSRepository : IWMSRepository
    {
       
        private readonly string _connectionString;
        private readonly string _connectionStringPiso;
        private readonly string _ImpresoraDevolucion;

        public WMSRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzas");
            _connectionStringPiso = configuracion.GetConnectionString("IMAplicativos");
            _ImpresoraDevolucion = "192.168.10.128";
            //_ImpresoraDevolucion = "10.1.1.208";

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

            //if (lineaArticulo != 910)
            if(!etiqueta.EndsWith("^XZ"))
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
                        <strong>Fecha: </strong>" + encabezado[0].fecha.ToString("dd/MM/yyyy HH:mm:ss") + @" <br>
  	                    <strong>Motorista: </strong>" + encabezado[0].Motorista + " / " + camion + @" <br>
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
                            .OrderBy(x => x.inventBatchId).ThenBy(x => x.NameAlias)
                            .Select(x => new RollosDespachoDTO
                            {
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
            foreach (var element in resumen)
            {
                totalRollo += element.Cantidad;
                totalLY += element.Total;
                despacho += @"<tr>
                              <td>" + element.Color + @"</td>
                              <td>" + element.Config + @"</td>
                              <td>" + element.Cantidad + @"</td>
                              <td>" + element.Total + @"</td>      
                            </tr>";
            }

            //linea que muestra el total de rollos enviados
            despacho += @"</tbody>
                            <tfoot>
                                <tr>
                                  <td></td>
                                  <td>Total</td>
                                  <td>" + totalRollo + @"</td>
                                  <td>" + totalLY + @"</td>      
                             </tr>
                            </tfoot>
                        </table>";

            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(VariablesGlobales.Correo);

                var correos = await getCorreosDespacho();

                foreach (IM_WMS_Correos_Despacho correo in correos)
                {
                    mail.To.Add(correo.Correo);
                }
                mail.Subject = "Despacho No." + DespachoID.ToString().PadLeft(8, '0');
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
        public async Task<List<RollosDespachoDTO>> getRolloDespachoAX(string InventTransID, string INVENTSERIALID)
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

            var parametros = new List<SqlParameter> { };

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

            string encabezado = @"^XA^FO700,50^FWN^A0R,30,30^FDFecha: " + hoy + @"^FS^FO670,50^A0R,30,30^FDEmpacador: " + empleado.Nombre + @"^FS";

            string pie = @"^A0R,30,30^BY2,2,100^FO50,50^BC^FD" + IMBOXCODE + @"^FS^FO50,700^A0R,40,40^FDUbicacion: " + ubicacion + @"^FS^XZ";

            int cont = 0;
            string etiqueta = "";
            int position = 600;
            int subtotal = 0;

            foreach (var element in groupArray)
            {
                if (cont == 0)
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

                element.items.ForEach(x =>
               {
                   for (int i = 1; i <= 5 - x.INVENTSIZEID.Length; i++)
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
                if (cont == 3)
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
        public async Task<IM_WMS_Packing_DespachoPTDTO> GetPacking_DespachoPT(string ProdID, string userCreated, int Box, int DespachoID)
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
        public async Task<IM_WMS_ObtenerSecuencia_PL_PT_DTO> getSecuencia_PL_PT(int despachoID, string user, int almacenTo)
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
        public async Task<IM_WMS_EnviarDespacho> Get_EnviarDespachos(int DespachoID, string user, int cajasSegundas, int cajasTerceras)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", DespachoID),
                new SqlParameter("@cajasSegundas", cajasSegundas),
                new SqlParameter("@cajasTerceras", cajasTerceras)
            };

            IM_WMS_EnviarDespacho response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_EnviarDespacho>("[IM_WMS_EnviarDespacho]", parametros);

            if (response.Descripcion == "Enviado")
            {

                var Despacho = await getDetalle_Despacho_Excel(DespachoID);


                var almacenes = Despacho.Select(x => x.InventLocation).Distinct().ToList();
                var encabezado = await GetEncabezadoDespachoExcel(user);
                int cont1 = 0;
                foreach (var almacen in almacenes)
                {

                    var data = Despacho.Where(x => x.InventLocation == almacen).ToList();

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
                        else
                        {
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
                                var packing = await getSecuencia_PL_PT(DespachoID, user, cont1);
                                worksheet.Cells[9, 7].Value = packing.Secuencia + "." + packing.Anio;

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

        public async Task<IM_WMS_insertDetalleAdutoriaDenim> getInsertAuditoriaCajaTP(string ProdID, int Box, int IDUnico, int QTY)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@ProdID", ProdID),
                new SqlParameter("@Box", Box),
                new SqlParameter("@IDUnico", IDUnico),
                new SqlParameter("@QTY", QTY)
            };

            IM_WMS_insertDetalleAdutoriaDenim response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_insertDetalleAdutoriaDenim>("[IM_WMS_InsertAuditoriaTP]", parametros);

            return response;
        }
        public async Task<string> getEnviarCorreoAuditoriaTP(int DespachoID, string usuario)
        {
            var data = await getCajasAuditar(DespachoID);

            DateTime FechaVacia = new DateTime(1900, 01, 01);
            int diferencias = 0;
            int cajas = 0;
            int unidades = 0;
            DateTime fechaINI = new DateTime();
            DateTime FechaFin = new DateTime();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            //Todo
            var worksheet0 = package.Workbook.Worksheets.Add("Todo");
            int fila = 1;

            worksheet0.Cells[fila, 1].Value = "OP";
            worksheet0.Cells[fila, 2].Value = "Numero de Caja";
            worksheet0.Cells[fila, 3].Value = "Articulo";
            worksheet0.Cells[fila, 4].Value = "Talla";
            worksheet0.Cells[fila, 5].Value = "Color";
            worksheet0.Cells[fila, 6].Value = "Recibido";
            worksheet0.Cells[fila, 7].Value = "Auditado";
            worksheet0.Cells[fila, 8].Value = "Diferencia";
            worksheet0.Cells[fila, 9].Value = "Fecha Inicio";
            worksheet0.Cells[fila, 10].Value = "Fecha Final";

            fila++;

            data.ForEach(element => {
                worksheet0.Cells[fila, 1].Value = element.ProdID;
                worksheet0.Cells[fila, 2].Value = element.Box;
                worksheet0.Cells[fila, 3].Value = element.ItemID;
                worksheet0.Cells[fila, 4].Value = element.Size;
                worksheet0.Cells[fila, 5].Value = element.Color;
                worksheet0.Cells[fila, 6].Value = element.QTY;
                worksheet0.Cells[fila, 7].Value = element.Auditado;
                worksheet0.Cells[fila, 8].Value = element.QTY - element.Auditado;
                worksheet0.Cells[fila, 9].Value = element.FechaIni == FechaVacia ? "" : element.FechaIni;
                worksheet0.Cells[fila, 10].Value = element.FechaFin == FechaVacia ? "" : element.FechaFin;
                diferencias += element.QTY - element.Auditado;

                if (fila == 2)
                {
                    fechaINI = element.FechaIni;
                    FechaFin = element.FechaFin;
                }
                else
                {
                    if (fechaINI > element.FechaIni && element.FechaIni != FechaVacia)
                    {
                        fechaINI = element.FechaIni;
                    }

                    if (FechaFin < element.FechaFin)
                    {
                        FechaFin = element.FechaFin;
                    }
                }

                if (element.Auditado != 0)
                {
                    cajas++;
                }
                unidades += element.Auditado;

                fila++;
            });
            worksheet0.Cells[fila, 7].Value = "Diferencias";
            worksheet0.Cells[fila, 7].Style.Font.Bold = true;
            worksheet0.Cells[fila, 8].Formula = $"SUM(H2:H{fila - 1})";
            worksheet0.Cells[fila, 8].Style.Font.Bold = true;
            worksheet0.Cells[1, 9, fila - 1, 10].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";

            var rangeTable0 = worksheet0.Cells[1, 1, fila - 1, 10];
            var table0 = worksheet0.Tables.Add(rangeTable0, "Todo");
            table0.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;
            worksheet0.Cells.AutoFitColumns();




            Byte[] fileContents = package.GetAsByteArray();
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(VariablesGlobales.Correo);

                var correos = await getCorreosRecepcionUbicacionCajas();

                foreach (IM_WMS_Correos_DespachoPTDTO correo in correos)
                {
                    mail.To.Add(correo.Correo);
                }

                //mail.To.Add("bavila@intermoda.com.hn");

                mail.Subject = "Auditoria TP Despacho " + DespachoID;
                mail.IsBodyHtml = false;
               
                mail.Body = "Auditoria TP Despacho: " + DespachoID + " usuario: " + usuario + " Diferencia: " + diferencias + " Cajas: " + cajas + " unidades: " + unidades + " Inicio: " + fechaINI + " Fin: " + FechaFin;


                using (MemoryStream ms = new MemoryStream(fileContents))
                {
                    DateTime date = DateTime.Now;
                    string fechah = date.Day + "_" + date.Month + "_" + date.Year;
                    Attachment attachment = new Attachment(ms, "AuditoriaTP_Despacho:" + DespachoID + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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

                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parametros = new List<SqlParameter> {
                new SqlParameter("@DespachoID",DespachoID)
                };
                List<IM_WMS_Get_Despachos_PT_DTO> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Get_Despachos_PT_DTO>("[IM_WMS_EnviarAuditoriaTP]", parametros);


            }
            catch (Exception err)
            {
                return err.ToString();
            }

            return "OK";


            
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
        //transferir
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
   
                                 <h4>" + JournalID + @"</h4>";
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

            html += @"<table style='width: 100%'>
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
                mail.Subject = "Diario Transferir " + JournalID + " desde almacen " + encabezado.INVENTLOCATIONID + " a " + encabezado.IM_INVENTLOCATIONID_TO;
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
        public async Task<IM_WMS_InsertTransferirCajaDetalle> GetInsertTransferirCajaDetalle(string journalID, string ItemBarcode, string BoxNum, string Proceso)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@JournalID", journalID),
                new SqlParameter("@itemBarCode", ItemBarcode),
                new SqlParameter("@BoxCode", BoxNum),
                new SqlParameter("@proceso", Proceso),

            };

            IM_WMS_InsertTransferirCajaDetalle response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_InsertTransferirCajaDetalle>("[IM_WMS_InsertTransferirCajaDetalle]", parametros);

            return response;
        }
        public async Task<List<LineasDTO>> GetLineasDiarioTransferir(string diario)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@journalID", diario)
            };

            List<LineasDTO> response = await executeProcedure.ExecuteStoredProcedureList<LineasDTO>("[IM_WMS_GetLineasDiarioTransferir]", parametros);

            return response;
        }
        public async Task<List<EtiquetaTransferir>> GetDatosEtiquetaTransferir(string diario, string BoxCode)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@JournalID", diario),
                new SqlParameter("@BoxCode", BoxCode)
            };

            List<EtiquetaTransferir> response = await executeProcedure.ExecuteStoredProcedureList<EtiquetaTransferir>("[IM_WMS_EtiquetaDiarioTransferir]", parametros);

            return response;
        }
        public async Task<string> GetImprimirEtiquetaTransferir(string diario, string IMBoxCode, string PRINT)
        {
            var data = await GetDatosEtiquetaTransferir(diario, IMBoxCode);

            //creacion de encabezado de la etiqueta
            string encabezado = "";
            encabezado += "^XA";
            encabezado += "^CF0,50";
            encabezado += "^FO280,50^FD" + diario + "^FS";
            encabezado += "^CF0,24";
            encabezado += "^FO50,115^FDFecha: " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "^FS";
            encabezado += "^FO50,140^FDSoliciante: " + data[0].Solicitante + "^FS";
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

            var groupData = new Dictionary<string, List<EtiquetaTransferir>>();

            foreach (var element in data)
            {
                totalUnidades += element.QTY;
                var key = $"{element.ITEMID}-{element.INVENTCOLORID}";

                if (!groupData.ContainsKey(key))
                {
                    groupData[key] = new List<EtiquetaTransferir>();
                }

                groupData[key].Add(element);
            }

            var groupArray = groupData.Select(x => new EtiquetaGrupoTransferir
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

            if (!etiqueta.EndsWith("^XZ"))
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

        //

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

        public async Task<List<IM_WMS_BusquedaRollosAXDTO>> GetBusquedaRollosAX(string INVENTLOCATIONID, string INVENTSERIALID, string INVENTBATCHID, string INVENTCOLORID, string WMSLOCATIONID, string REFERENCE)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            INVENTLOCATIONID = INVENTLOCATIONID == "-" ? "" : INVENTLOCATIONID;
            INVENTSERIALID = INVENTSERIALID == "-" ? "" : INVENTSERIALID;
            INVENTBATCHID = INVENTBATCHID == "-" ? "" : INVENTBATCHID;
            INVENTCOLORID = INVENTCOLORID == "-" ? "" : INVENTCOLORID;
            WMSLOCATIONID = WMSLOCATIONID == "-" ? "" : WMSLOCATIONID;
            REFERENCE = REFERENCE == "-" ? "" : REFERENCE;

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@INVENTLOCATIONID", INVENTLOCATIONID),
                new SqlParameter("@INVENTSERIALID", INVENTSERIALID),
                new SqlParameter("@INVENTBATCHID", INVENTBATCHID),
                new SqlParameter("@INVENTCOLORID", INVENTCOLORID),
                new SqlParameter("@WMSLOCATIONID", WMSLOCATIONID),
                new SqlParameter("@REFERENCE", REFERENCE)
            };

            List<IM_WMS_BusquedaRollosAXDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_BusquedaRollosAXDTO>("[IM_WMS_BusquedaRollosAX]", parametros);

            return response;
        }

        public async Task<List<IM_WMS_DespachosRecibidosLiquidacionDTO>> GetDespachosRecibidosLiquidacion(int despachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", despachoID)
            };

            List<IM_WMS_DespachosRecibidosLiquidacionDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_DespachosRecibidosLiquidacionDTO>("[IM_WMS_DespachosRecibidosLiquidacion]", parametros);

            return response;
        }

        public async Task<List<IM_WMS_DespachoPT_OrdenesRecibidasDepachoDTO>> GetOrdenesRecibidasDepacho(int despachoID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", despachoID)
            };

            List<IM_WMS_DespachoPT_OrdenesRecibidasDepachoDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_DespachoPT_OrdenesRecibidasDepachoDTO>("[IM_WMS_DespachoPT_OrdenesRecibidasDepacho]", parametros);

            return response;
        }

        public async Task<List<IM_WMS_DespachoPT_DetalleOrdenRecibidaLiquidacionDTO>> GetDetalleOrdenRecibidaLiquidacion(int despachoID, string ProdCutSheetID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", despachoID),
                new SqlParameter("@ProdCutSheetID", ProdCutSheetID)

            };

            List<IM_WMS_DespachoPT_DetalleOrdenRecibidaLiquidacionDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_DespachoPT_DetalleOrdenRecibidaLiquidacionDTO>("[IM_WMS_DespachoPT_DetalleOrdenRecibidaLiquidacion]", parametros);

            return response;
        }

        //Inventario cicliclo de telas
        public async Task<List<IM_WMS_InventarioCiclicoTelasDiariosAbiertos>> GetInventarioCiclicoTelasDiariosAbiertos()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            { };

            List<IM_WMS_InventarioCiclicoTelasDiariosAbiertos> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_InventarioCiclicoTelasDiariosAbiertos>("[IM_WMS_InventarioCiclicoTelasDiariosAbiertos]", parametros);

            return response;
        }

        public async Task<List<IM_WMS_InventarioCilicoTelaDiario>> Get_InventarioCilicoTelaDiarios(string JournalID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@JournalID", JournalID)
            };

            List<IM_WMS_InventarioCilicoTelaDiario> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_InventarioCilicoTelaDiario>("[IM_WMS_InventarioCilicoTelaDiario]", parametros);

            return response;
        }

        public async Task<IM_WMS_InventarioCilicoTelaDiario> GetInventarioCilicoTelaDiario(string JournalID, string InventSerialID, string user, decimal QTY)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@JournalID", JournalID),
                new SqlParameter("@inventSerialID", InventSerialID),
                new SqlParameter("@UserID", user),
                new SqlParameter("@QTY", QTY)
            };

            IM_WMS_InventarioCilicoTelaDiario response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_InventarioCilicoTelaDiario>("[IM_WMS_InventarioCiclicoTelaExist]", parametros);

            return response;
        }

        public async Task<IM_WMS_InventarioCilicoTelaDiario> Get_AgregarInventarioCilicoTelaDiario(string JournalID, string InventSerialID, string ubicacion, decimal QTY)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@JournalID", JournalID),
                new SqlParameter("@inventSerialID", InventSerialID),
                new SqlParameter("@Ubicacion", ubicacion),
                new SqlParameter("@QTY", QTY)
            };

            IM_WMS_InventarioCilicoTelaDiario response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_InventarioCilicoTelaDiario>("[IM_WMS_AgregarInventarioCiclicoTela]", parametros);

            return response;
        }

        public async Task<List<IM_WMS_Correos_DespachoPTDTO>> getCorreoCiclicoTela()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter> { };

            List<IM_WMS_Correos_DespachoPTDTO> result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Correos_DespachoPTDTO>("[IM_WMS_ObtenerCorreosCiclicotela]", parametros);

            return result;
        }
        public async Task<IM_WMS_InventarioCilicoTelaDiario> UpdateQTYInventSerialID(string InventserialID, decimal QTY)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter> {
                new SqlParameter("@inventSerialID", InventserialID),
                new SqlParameter("@QTY", QTY)

            };

            IM_WMS_InventarioCilicoTelaDiario result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_InventarioCilicoTelaDiario>("[IM_WMS_Update_QTY_InventSerialID]", parametros);

            return result;
        }

        //Recepcion y ubicacion cajas

        public async Task<SP_GetBoxesReceived> getBoxesReceived(string opBoxNum, string ubicacion, string Tipo)
        {
            
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@opBoxNum", opBoxNum),
                new SqlParameter("@ubicacion", ubicacion)
            };
            SP_GetBoxesReceived response;
            if (Tipo == "DENIM")
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionStringPiso);
                response = await executeProcedure.ExecuteStoredProcedure<SP_GetBoxesReceived>("[SP_GetBoxesReceived]", parametros);

            }
            else if( Tipo == "TP")
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                response = await executeProcedure.ExecuteStoredProcedure<SP_GetBoxesReceived>("[IM_WMS_TP_InsertBox]", parametros);

            }
            else
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionStringPiso);
                response = await executeProcedure.ExecuteStoredProcedure<SP_GetBoxesReceived>("[SP_GetBoxesReceivedMB]", parametros);
            }

            return response;
        }

        public async Task<List<SP_GetAllBoxesReceived>> getAllBoxesReceived(Filtros filtro)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure((filtro.Tipo == "MB" || filtro.Tipo == "TP") ? _connectionString: _connectionStringPiso);

            var parametros = new List<SqlParameter> {
                new SqlParameter("@Lote", filtro.Lote),
                new SqlParameter("@Orden", filtro.Orden),
                new SqlParameter("@articulo", filtro.Articulo),
                new SqlParameter("@talla", filtro.Talla),
                new SqlParameter("@color", filtro.Color),
                new SqlParameter("@ubicacion", filtro.Ubicacion),
                new SqlParameter("@page", filtro.page),
                new SqlParameter("@size", filtro.size)
            };
            List<SP_GetAllBoxesReceived> result;
            if (filtro.Tipo == "DENIM")
            {
                result = await executeProcedure.ExecuteStoredProcedureList<SP_GetAllBoxesReceived>("[SP_GetAllBoxesReceived_V2]", parametros);

            }
            else if(filtro.Tipo == "TP")
            {
                result = await executeProcedure.ExecuteStoredProcedureList<SP_GetAllBoxesReceived>("[IM_WMS_TP_DetalleCajas]", parametros);

            }
            else
            {
                result = await executeProcedure.ExecuteStoredProcedureList<SP_GetAllBoxesReceived>("[IM_WMS_MB_DetalleCajas]", parametros);

            }


            return result;
        }

        public async Task<List<SP_GetAllBoxesReceived>> getAllBoxesReceived(string TIPO)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionStringPiso);

            var parametros = new List<SqlParameter> { };
            List<SP_GetAllBoxesReceived> result;
            if (TIPO == "DENIM")
            {
                result = await executeProcedure.ExecuteStoredProcedureList<SP_GetAllBoxesReceived>("[SP_GetAllBoxesReceived]", parametros);
            }
            else //if(TIPO == "TP")
            {
                result = await executeProcedure.ExecuteStoredProcedureList<SP_GetAllBoxesReceived>("[SP_GetAllBoxesReceivedTP]", parametros);

            }
            /*else
            {
                result = await executeProcedure.ExecuteStoredProcedureList<SP_GetAllBoxesReceived>("[SP_GetAllBoxesReceivedMB]", parametros);

            }*/
            return result;
        }

        public async Task<string> postEnviarRecepcionUbicacionCajas(List<Ubicaciones> data)
        {
            await getAllBoxesReceived("DENIM");
            List<SP_GetAllBoxesReceived> list = new List<SP_GetAllBoxesReceived>();
            List<IM_ObtenerTrasladoRecepcion> traslados = new List<IM_ObtenerTrasladoRecepcion>();

            DateTime date = DateTime.Now;
            string fecha = date.Day + "-" + date.Month + "-" + date.Year;

            data.ForEach(element =>
           {
               for(int i= 0; i < element.Ordenes.Length;i++)
               {
                   var orden = element.Ordenes[i].Split(",");
                   Filtros filtro = new Filtros();
                   filtro.Ubicacion = "";
                   filtro.size = 200000;
                   filtro.Articulo = "";
                   filtro.Lote = "";
                   filtro.Orden = orden[0];
                   filtro.Talla = "";
                   filtro.Color = "";
                   filtro.page = 0;
                   filtro.Tipo = "DENIM";
                   var datos = getAllBoxesReceived(filtro).Result;
                   datos = datos.FindAll(x => element.Ordenes[i].Contains(x.OP + "," + x.NumeroDeCaja));
                   datos.ForEach(x =>
                   {
                       string fechatmp = x.FechaDeRecepcion.Day + "-" + x.FechaDeRecepcion.Month + "-" + x.FechaDeRecepcion.Year;
                       if (fecha == fechatmp && element.Ordenes.Contains(x.OP + "," + x.NumeroDeCaja))
                       {
                           if (!list.Any(l => l.OP == x.OP && l.NumeroDeCaja == x.NumeroDeCaja))
                           {
                               list.Add(x);
                           }
                           var TrasTmp = this.gettrasladosRecepcion(x.OP, x.NumeroDeCaja).Result;
                           if (traslados.Find(el => el.TransferIdAx1 == TrasTmp.TransferIdAx1 && el.TransferIdAx1 == TrasTmp.TransferIdAx1)?.TransferIdAx1 == null)
                           {
                               traslados.Add(TrasTmp);
                           }
                       }
                   });
               }  
           });

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
                    list = list.Distinct().ToList();
                    list.ForEach(x =>
                    {
                        worksheet.Cells[fila, 1].Value = x.Lote;
                        worksheet.Cells[fila, 2].Value = x.OP;
                        worksheet.Cells[fila, 3].Value = x.Articulo;
                        worksheet.Cells[fila, 4].Value = x.NumeroDeCaja;
                        worksheet.Cells[fila, 5].Value = x.Talla;
                        worksheet.Cells[fila, 6].Value = x.CantidadEnCaja;
                        worksheet.Cells[fila, 7].Value = x.FechaDeEnvio;
                        worksheet.Cells[fila, 8].Value = x.FechaDeRecepcion;
                        worksheet.Cells[fila, 9].Value = x.Color;
                        worksheet.Cells[fila, 10].Value = x.ubicacion;
                        fila++;

                    });
                    var rangeTable = worksheet.Cells[1, 1, fila, 10];
                    var table = worksheet.Tables.Add(rangeTable, "MyTable");
                    table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;

                    //sacar resumen
                    var listagrupada = list
                        .GroupBy(caja => caja.Lote)
                        .Select(grupo => new SP_GetAllBoxesReceived
                        {
                            Lote = grupo.Key,
                            CantidadEnCaja = grupo.Sum(caja => caja.CantidadEnCaja)
                        }).ToList();

                    ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add("Hoja2");
                    worksheet2.Cells[1, 1].Value = "Lote";
                    worksheet2.Cells[1, 2].Value = "Cantidad";
                    int fila2 = 2;
                    listagrupada.ForEach(x =>
                    {
                        worksheet2.Cells[fila2, 1].Value = x.Lote;
                        worksheet2.Cells[fila2, 2].Value = x.CantidadEnCaja;
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


                        TimeSpan tiempo = list[0].FechaDeRecepcion - list[list.Count - 1].FechaDeRecepcion;


                        mail.Subject = "Recepcion Producto Terminado DENIM " + fecha;
                        mail.IsBodyHtml = true;

                        mail.Body = "<p>Recepcion Producto Terminado DENIM" + " Camion: " + data[0].Camion + " Usuario: " + data[0].Usuario + ", Descargado en " + tiempo.Hours + " horas y " + tiempo.Minutes + " Minutos " + tiempo.Seconds + " Segundos</p><h2>Traslados:</h2>";
                        traslados.ForEach(x =>
                        {
                            if(x.TransferIdAx1 != "" || x.TransferIdAx2 != "")
                            {
                                mail.Body += "<p>Traslado Primeras: " + x.TransferIdAx1 + " Traslado Segundas/Terceras: " + x.TransferIdAx2 + "</p>";

                            }
                        });
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
            }
            catch (Exception err)
            {
                return err.ToString();
            }

            return "OK";
        }

        public async Task<List<IM_WMS_TP_DetalleCajasResumen>> getResumenCajasUnidadesTP(Filtros filtro)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            List<IM_WMS_TP_DetalleCajasResumen> result = new List<IM_WMS_TP_DetalleCajasResumen>();
            var parametros = new List<SqlParameter> {
                new SqlParameter("@Lote", filtro.Lote),
                new SqlParameter("@Orden", filtro.Orden),
                new SqlParameter("@articulo", filtro.Articulo),
                new SqlParameter("@talla", filtro.Talla),
                new SqlParameter("@color", filtro.Color),
                new SqlParameter("@ubicacion", filtro.Ubicacion)
               
            };

            result = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_TP_DetalleCajasResumen>("[IM_WMS_TP_DetalleCajasResumen]", parametros);

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
        public async Task<IM_ObtenerTrasladoRecepcion> gettrasladosRecepcion(string WorkOrderID, string BoxNum)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionStringPiso);

            var parametros = new List<SqlParameter> {
                new SqlParameter("@WorkOrderID",WorkOrderID),
                new SqlParameter("@BoxNum",BoxNum)

            };

            IM_ObtenerTrasladoRecepcion result = await executeProcedure.ExecuteStoredProcedure<IM_ObtenerTrasladoRecepcion>("[IM_ObtenerTrasladoRecepcion]", parametros);

            return result;
        }

        public async Task<SP_GetBoxesReceived> getBoxesReserved(string opBoxNum, string ubicacion)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionStringPiso);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@opBoxNum", opBoxNum),
                new SqlParameter("@ubicacion", ubicacion)
            };
            SP_GetBoxesReceived response;

            response = await executeProcedure.ExecuteStoredProcedure<SP_GetBoxesReceived>("[SP_GetBoxesReserved]", parametros);

            return response;
        }

        public async Task<List<SP_GetAllBoxesReserved_V2>> GetAllBoxesReserved_V2(FiltroDeclaracionEnvio data)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionStringPiso);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@caja", data.Caja),
                new SqlParameter("@pais", data.Pais),
                new SqlParameter("@cuentaCliente", data.CuentaCliente),
                new SqlParameter("@nombreCliente", data.NombreCliente),
                new SqlParameter("@pedidoVenta", data.PedidoVenta),
                new SqlParameter("@ListaEmpaque", data.ListaEmpaque),
                new SqlParameter("@Albaran", data.Albaran),
                new SqlParameter("@Ubicacion", data.Ubicacion),
                new SqlParameter("@Factura", data.Factura),
                new SqlParameter("@page", data.Page),
                new SqlParameter("@size", data.Size),
                new SqlParameter("@fecha", data.fecha)

            };
            List<SP_GetAllBoxesReserved_V2> response;

            response = await executeProcedure.ExecuteStoredProcedureList<SP_GetAllBoxesReserved_V2>("[SP_GetAllBoxesReserved_V2]", parametros);

            return response;
        }

        public async Task<List<SP_GetAllBoxesReserved_V2>> GetAllBoxesReserved()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionStringPiso);
            var parametros = new List<SqlParameter> { };
            List<SP_GetAllBoxesReserved_V2> response;

            response = await executeProcedure.ExecuteStoredProcedureList<SP_GetAllBoxesReserved_V2>("[SP_GetAllBoxesReserved]", parametros);

            return response;
        }

        public async Task<List<IMDeclaracionEnvio>> GetDeclaracionEnvio(string pais, string ubicacion, string fecha)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionStringPiso);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@pais", pais),
                new SqlParameter("@ubicacion", ubicacion),
                new SqlParameter("@fecha", fecha)

            };
            List<IMDeclaracionEnvio> response = await executeProcedure.ExecuteStoredProcedureList<IMDeclaracionEnvio>("[IMDeclaracionEnvio]", parametros);

            return response;
        }

        //control cajas etiquetado

        public async Task<IM_WMS_Insert_Control_Cajas_Etiquetado> GetControl_Cajas_Etiquetado(string caja, string empleado)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Boxnum", caja),
                new SqlParameter("@Employee", empleado)
            };
            IM_WMS_Insert_Control_Cajas_Etiquetado response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_Insert_Control_Cajas_Etiquetado>("[IM_WMS_Insert_Control_Cajas_Etiquetado]", parametros);

            return response;
        }

        public async Task<List<IM_WMS_Control_Cajas_Etiquetado_Detalle>> Get_Control_Cajas_Etiquetado_Detalles(IM_WMS_Control_Cajas_Etiquetado_Detalle_Filtro filtro)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@pedido", filtro.Pedido),
                new SqlParameter("@ruta", filtro.Ruta),
                new SqlParameter("@Boxnum", filtro.BoxNum),
                new SqlParameter("@lote", filtro.Lote),
                new SqlParameter("@empleado", filtro.Empleado),
                new SqlParameter("@page", filtro.Page),
                new SqlParameter("@size", filtro.Size),
                new SqlParameter("@Fecha", filtro.Fecha)

            };

            List<IM_WMS_Control_Cajas_Etiquetado_Detalle> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Control_Cajas_Etiquetado_Detalle>("[IM_WMS_Control_Cajas_Etiquetado_Detalle]", parametros);

            return response;
        }
        //generacion de precios y codigos
        public async Task<List<IM_WMS_ObtenerDetalleGeneracionPrecios>> GetObtenerDetalleGeneracionPrecios(string pedido, string empresa)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var pedidos =pedido.Split(',');
            List<IM_WMS_ObtenerDetalleGeneracionPrecios> response =  new List<IM_WMS_ObtenerDetalleGeneracionPrecios>();
            for (int i = 0; i< pedidos.Length; i++)
            {
                var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@pedido", pedidos[i]),
                    new SqlParameter("@empresa", empresa),

                };
                List<IM_WMS_ObtenerDetalleGeneracionPrecios> tmp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_ObtenerDetalleGeneracionPrecios>("[IM_WMS_ObtenerDetalleGeneracionPrecios]", parametros);
                tmp.ForEach(ele =>
                {
                    response.Add(ele);

                });
            }  
            return response;
        }

        public async Task<List<IM_WMS_ObtenerPreciosCodigos>> GetObtenerPreciosCodigos(string cuentaCliente)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@cuentaCliente", cuentaCliente),

            };

            List<IM_WMS_ObtenerPreciosCodigos> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_ObtenerPreciosCodigos>("[IM_WMS_ObtenerPreciosCodigos]", parametros);

            return response;
        }

        public async Task<IM_WMS_ObtenerPreciosCodigos> postInsertUpdatePrecioCodigos(IM_WMS_ObtenerPreciosCodigos data)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@CuentaCliente", data.CuentaCliente),
                new SqlParameter("@Base", data.Base),
                new SqlParameter("@IDColor", data.IDColor),
                new SqlParameter("@Costo", data.Costo),
                new SqlParameter("@precio", data.Precio)
            };

            IM_WMS_ObtenerPreciosCodigos response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_ObtenerPreciosCodigos>("[IM_WMS_InsertUpdatePreciosCodigos]", parametros);

            return response;
        }

        public async Task<List<IM_WMS_DetalleImpresionEtiquetasPrecio>> GetDetalleImpresionEtiquetasPrecio(ImpresionEtiqueta parms)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            parms.Normalizar(parms);
            List<IM_WMS_DetalleImpresionEtiquetasPrecio> response;

            if (parms.EsGeneracionLibre)
            {
                var parametro = new List<SqlParameter>
                {
                    new SqlParameter("@SalesID", parms.Pedido),
                    new SqlParameter("@Ruta",parms.Ruta)
                };

                response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_DetalleImpresionEtiquetasPrecio>("[IM_WMS_DetalleImpresionEtiquetasPrecioSinEmpaque]", parametro);

                var query = response.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(parms.CodigoArticulo))
                    query = query.Where(x => x.Articulo.Contains(parms.CodigoArticulo));

                if (!string.IsNullOrWhiteSpace(parms.Talla))
                    query = query.Where(x => x.Talla == parms.Talla);

                if (!string.IsNullOrWhiteSpace(parms.Color))
                    query = query.Where(x => x.IDColor == parms.Color);

                response = query.ToList();


                return response;

            }

            var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@SalesID", parms.Pedido),
                    new SqlParameter("@Ruta", parms.Ruta),
                    new SqlParameter("@boxCode",parms.Caja)
                };

            response = await executeProcedure.
                ExecuteStoredProcedureList<IM_WMS_DetalleImpresionEtiquetasPrecio>("[IM_WMS_DetalleImpresionEtiquetasPrecio]", parametros);

            return response;
        }

        public string imprimirEtiquetaCajaDividir(string caja, string impresora)
        {

            string ipPrintTela = "10.1.1.114";

            if (ipPrintTela != impresora)
            {
                int filaStartInt = impresora == "10.1.1.86" ? 615 : 700;
                return imprimirEtiquetaCajaXS(caja, impresora, filaStartInt);
            }
            else
            {
                return imprimirEtiquetaCajaNormal(caja, impresora);
            }
        }

        private string imprimirEtiquetaCajaNormal(string caja, string impresora)
        {
            string etiqueta = @"^XA^FWN^PW1200^PR2";

            etiqueta += @"^FO915,25";
            etiqueta += @"^A0R,50,50";
            etiqueta += @"^FD" + caja + "^FS";

            etiqueta += @"^XZ";

            try
            {
                using (TcpClient client = new TcpClient(impresora, 9100))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(etiqueta);
                        stream.Write(bytes, 0, bytes.Length);
                        Thread.Sleep(500);

                    }
                }
            }
            catch (Exception err)
            {
                return err.ToString();
            };

            return "OK";
        }

        private string imprimirEtiquetaCajaXS(string caja, string impresora, int filaStartInt)
        {
            string etiqueta = @"^XA^FWN^PW1200^PR2";
            int fila = filaStartInt;
            fila -= 15;
            etiqueta += @"^FO"+ fila +",25";
            etiqueta += @"^A0R,30,30";
            etiqueta += @"^FD" + caja + "^FS";

            etiqueta += @"^XZ";

            try
            {
                using (TcpClient client = new TcpClient(impresora, 9100))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(etiqueta);
                        stream.Write(bytes, 0, bytes.Length);
                        Thread.Sleep(500);

                    }
                }
            }
            catch (Exception err)
            {
                return err.ToString();
            };

            return "OK";
        }

        //imprimir etiqutas precios 2
        public string imprimirEtiquetaprecios2(List<IM_WMS_DetalleImpresionEtiquetasPrecio> data, string fecha, string impresora)
        {
            string ipPrintTela = "10.1.1.114";

            if (ipPrintTela != impresora)
            {
                int filaStartInt = impresora == "10.1.1.86" ? 615 : 700;//

                return imprimirEtiquetaXs(data, fecha, impresora, filaStartInt);
            }
            else
            {
                return imprimirEtiquetaNormal(data, fecha, impresora);
            }
            
        }

        private string imprimirEtiquetaNormal(List<IM_WMS_DetalleImpresionEtiquetasPrecio> data, string fecha, string impresora)
        {
            int cont = 1;
            int fila = 915;
            string etiqueta = "";

            foreach (var element in data)
            {
                if (cont == 1)
                {
                    etiqueta = @"^XA^FWN^PW1200^PR2";
                }

                etiqueta += @"^FO" + fila + ",175";
                etiqueta += @"^A0R,50,50";
                etiqueta += @"^FD" + element.Nombre + "^FS";
                fila -= 25;

                etiqueta += @"^FO" + fila + ",30";
                etiqueta += @"^A0R,25,25";
                etiqueta += @"^FD" + element.Estilo + "^FS";

                if (element.Talla.Length > 2)
                {
                    etiqueta += @"^FO" + fila + ",300";
                    etiqueta += @"^A0R,25,25";
                    etiqueta += @"^FD" + element.Talla + "^FS";
                }
                else
                {
                    etiqueta += @"^FO" + fila + ",375";
                    etiqueta += @"^A0R,50,50";
                    etiqueta += @"^FD" + element.Talla + "^FS";
                }

                fila -= 25;
                etiqueta += @"^FO" + fila + ",30";
                etiqueta += @"^A0R,20,20";
                etiqueta += @"^FD" + element.Articulo + "^FS";


                fila -= 25;

                etiqueta += @"^FO" + fila + ",30";
                etiqueta += @"^A0R,20,20";
                etiqueta += @"^FD" + element.Descripcion + "^FS";

                fila -= 60;

                etiqueta += @"^BY4,2,60";
                etiqueta += @"^FO" + fila + ",60^BER,N,N";
                etiqueta += @"^FD" + element.CodigoBarra + "^FS";

                fila -= 36;

                etiqueta += @"^FO" + fila + ",100";
                etiqueta += "^A0R,23,50";
                etiqueta += "^FD" + element.CodigoBarra + "^FS";

                fila -= 34;

                etiqueta += @"^FO" + fila + ",50";
                etiqueta += @"^A0R,30,30";
                etiqueta += @"^FD" + element.IDColor + "^FS";

                fila -= 25;

                etiqueta += @"^FO" + fila + ",50";
                etiqueta += @"^A0R,25,25";
                etiqueta += @"^FDIVA incluido^FS";

                etiqueta += @"^FO" + fila + ",400";
                etiqueta += @"^A0R,30,30";
                var dia = DateTime.Now;
                string fechatxt = dia.Month.ToString() + dia.Year.ToString().Substring(2, 2);

                etiqueta += @"^FD" + (fecha.Length != 1 ? fecha : fechatxt) + "^FS";

                etiqueta += @"^FO" + fila + "," + (element.Decimal || element.Moneda != "" ? "190" : "210");
                etiqueta += @"^A0R,55,55";
                etiqueta += @"^FD" + (element.Moneda != "" ? element.Moneda : "") + (element.Decimal ? element.Precio.ToString("F2") : element.Precio.ToString("F0")) + "^FS";

                fila -= 105;



                if (cont == 3)
                {
                    etiqueta += @"^XZ";
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
                    };
                    //imprimir
                    fila = 915;
                    cont = 1;
                }
                else
                {
                    cont++;
                }

            }

            if (cont != 1)
            {
                etiqueta += @"^XZ";
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
                };
            }
            return "OK";
        }

        private string imprimirEtiquetaXs(List<IM_WMS_DetalleImpresionEtiquetasPrecio> data, string fecha, string impresora, int filaStartInt)

        {

            int cont = 1;

            int fila = filaStartInt;

            string etiqueta = "";

            foreach (var element in data)

            {

                if (cont == 1)

                {

                    etiqueta = @"^XA^MD5^PRC^FWN";

                }

                etiqueta += @"^FO" + fila + ",100";

                etiqueta += @"^A0R,30,30";

                etiqueta += @"^FD" + element.Nombre + "^FS";

                fila -= 10;

                etiqueta += @"^FO" + fila + ",20";

                etiqueta += @"^A0R,15,15";

                etiqueta += @"^FD" + element.Estilo + "^FS";

                if (element.Talla.Length > 2)

                {

                    etiqueta += @"^FO" + (fila) + ",220";

                    etiqueta += @"^A0R,20,20";

                    etiqueta += @"^FD" + element.Talla.Replace("-", "") + "^FS";

                }

                else

                {

                    etiqueta += @"^FO" + fila + ",260";

                    etiqueta += @"^A0R,35,35";

                    etiqueta += @"^FD" + element.Talla + "^FS";

                }

                fila -= 16;

                etiqueta += @"^FO" + fila + ",20";

                etiqueta += @"^A0R,15,15";

                etiqueta += @"^FD" + element.Articulo + "^FS";


                fila -= 16;

                etiqueta += @"^FO" + fila + ",20";

                etiqueta += @"^A0R,15,15";

                etiqueta += @"^FD" + element.Descripcion + "^FS";

                fila -= 36;

                etiqueta += @"^BY2,5,54";

                etiqueta += @"^FO" + fila + ",60";

                etiqueta += @"^BER,35,S,S";

                etiqueta += @"^FD" + element.CodigoBarra + "^FS";

                //fila -= 42;

                //etiqueta += @"^FO" +fila +",40";

                //etiqueta += "^A0R,28,35";

                //etiqueta += "^FD" + element.CodigoBarra + "^FS";

                fila -= 62;

                etiqueta += @"^FO" + fila + ",20";

                etiqueta += @"^A0R,18,18";

                etiqueta += @"^FD" + element.IDColor + "^FS";

                fila -= 20;

                etiqueta += @"^FO" + fila + ",20";

                etiqueta += @"^A0R,18,18";

                etiqueta += @"^FDIVA incluido^FS";

                etiqueta += @"^FO" + fila + ",270";

                etiqueta += @"^A0R,20,20";

                var dia = DateTime.Now;

                string fechatxt = dia.Month.ToString() + dia.Year.ToString().Substring(2, 2);

                etiqueta += @"^FD" + (fecha.Length != 1 ? fecha : fechatxt) + "^FS";

                etiqueta += @"^FO" + fila + "," + (element.Decimal || element.Moneda != "" ? "110" : "140");

                if (element.Moneda != "" || element.Decimal)

                {

                    etiqueta += @"^A0R,32,32";

                }

                else

                {

                    etiqueta += @"^A0R,38,38";

                }

                etiqueta += @"^FD" + (element.Moneda != "" ? element.Moneda : "") + (element.Decimal ? element.Precio.ToString("F2") : element.Precio.ToString("F0")) + "^FS";

                fila -= 67;


                if (cont == 3)

                {

                    etiqueta += @"^XZ";

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

                    };

                    //imprimir

                    fila = filaStartInt;

                    cont = 1;

                }

                else

                {

                    cont++;

                }

            }

            if (cont != 1)

            {

                etiqueta += @"^XZ";

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

                };

            }

            return "OK";

        }



        public string imprimirEtiquetaprecios(IM_WMS_DetalleImpresionEtiquetasPrecio data, int multiplo, int faltante,string fecha,string impresora)
        {
            int fila = 915;
            string etiqueta = @"^XA^FWN^PW1200^PR2";
            for (int i = 1; i <= 3; i++)
            {
                etiqueta += @"^FO" + fila + ",175";
                etiqueta += @"^A0R,50,50";
                etiqueta += @"^FD"+data.Nombre+"^FS";
                fila -= 25;

                etiqueta += @"^FO" + fila + ",30";
                etiqueta += @"^A0R,25,25";
                etiqueta += @"^FD" + data.Estilo + "^FS";

                if (data.Talla.Length > 2)
                {
                    etiqueta += @"^FO" + fila + ",300";
                    etiqueta += @"^A0R,25,25";
                    etiqueta += @"^FD" + data.Talla + "^FS";
                }
                else
                {
                    etiqueta += @"^FO" + fila + ",375";
                    etiqueta += @"^A0R,50,50";
                    etiqueta += @"^FD" + data.Talla + "^FS";
                }

                fila -= 25;
                etiqueta += @"^FO" + fila + ",30";
                etiqueta += @"^A0R,20,20";
                etiqueta += @"^FD" + data.Articulo + "^FS";            
                

                fila -= 25;

                etiqueta += @"^FO" + fila + ",30";
                etiqueta += @"^A0R,20,20";
                etiqueta += @"^FD" + data.Descripcion + "^FS";

                fila -= 60;

                etiqueta += @"^BY4,2,60";
                etiqueta += @"^FO" + fila + ",60^BER";
                etiqueta += @"^FD" + data.CodigoBarra + "^FS";

                fila -= 70;

                etiqueta += @"^FO" + fila + ",50";
                etiqueta += @"^A0R,30,30";
                etiqueta += @"^FD" + data.IDColor + "^FS";

                fila -= 25;

                etiqueta += @"^FO" + fila + ",50";
                etiqueta += @"^A0R,25,25";
                etiqueta += @"^FDIVA incluido^FS";

                etiqueta += @"^FO" + fila + ",400";
                etiqueta += @"^A0R,30,30";
                var dia = DateTime.Now;
                string fechatxt = dia.Month.ToString() + dia.Year.ToString().Substring(2,2);

                etiqueta += @"^FD"+(fecha.Length != 1 ? fecha:fechatxt) +"^FS";                

                etiqueta += @"^FO" + fila + ","+(data.Decimal || data.Moneda != ""? "190": "210");
                etiqueta += @"^A0R,55,55";
                etiqueta += @"^FD" + (data.Moneda !="" ?data.Moneda:"")+(data.Decimal ? data.Precio.ToString("F2"):data.Precio.ToString("F0")) + "^FS";

                fila -= 105;

                if (faltante != 0 && faltante == i)
                {
                    i = 5;
                }
            }
            if (multiplo > 0)
            {
                etiqueta += @"^PQ" + multiplo;
            }

            etiqueta += @"^XZ";

            try
            {
                using (TcpClient client = new TcpClient(impresora, 9100))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(etiqueta);
                        
                        stream.Write(bytes, 0, bytes.Length);
                        Thread.Sleep((multiplo+faltante)*800);

                    }
                }
            }
            catch (Exception err)
            {
                return err.ToString();
            };
            return "OK";
        }

        public async Task<List<IM_WMS_ClientesGeneracionprecios>> GetClientesGeneracionprecios()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> { };
            

            List<IM_WMS_ClientesGeneracionprecios> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_ClientesGeneracionprecios>("[IM_WMS_ObtenerClientesGeneracionPrecio]", parametros);

            return response;
        }

        public async Task<IM_WMS_ClientesGeneracionprecios> postClienteGeneracionPrecio(IM_WMS_ClientesGeneracionprecios data)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@CuentaCliente", data.CuentaCliente),
                new SqlParameter("@Nombre", data.Nombre),
                new SqlParameter("@Moneda", data.Moneda),
                new SqlParameter("@Decimal", data.Decimal)
            };

            IM_WMS_ClientesGeneracionprecios response = await executeProcedure.ExecuteStoredProcedure<IM_WMS_ClientesGeneracionprecios>("[IM_WMS_UpdateClientesGeneracionPrecio]", parametros);

            return response;
        }

        //Tracking Pedido
        public async Task<string> getEnviarCorreoTrackingPedidos(string fecha)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@fecha", fecha)

            };
            DateTime FechaVacia = new DateTime(1900, 01, 01);

            List<IM_WMS_GenerarDetalleFacturas> Pedidos = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_GenerarDetalleFacturas>("[IM_WMS_GenerarDetalleFacturas]", parametros);

            //variblea para resumen
            string AlbaranTableHTML = "<table border='2'><caption><h2>Albaranado y No Facturado</h2></caption><thead><th>Pedido Venta</th><th>Cliente</th><th>Responsable</th><th>Lista Empaque</th><th>Albaran</th><th>Fecha Albaran</th><th>Piezas</th><th>Dias sin Factura</th></thead><tbody>";
            int AlbaranQTY = 0, AlbaranUnidades = 0;
            List<string> ClientesAlbaran = new List<string>();

            string ListaCompletadaTableHTML = "<table border='2'><caption><h2>Lista Empaque Completada y no Albaranado</h2></caption><thead><th>Pedido Venta</th><th>Cliente</th><th>Responsable</th><th>Lista Empaque</th><th>Fecha Completada</th><th>Piezas</th><th>Dias sin Albaran</th></thead><tbody>";
            int ListaCompletadaQTY = 0, ListaCompletadaUnidades = 0;
            List<string> ClientesListaCompletada = new List<string>();


            string ListaNoCompletadaTableHTML = "<table border='2'><caption><h2>Lista Empaque pendiente</h2></caption><thead><th>Pedido Venta</th><th>Cliente</th><th>Responsable</th><th>Lista Empaque</th><th>Fecha Iniciado</th><th>Piezas</th><th>Dias sin Albaran</th></thead><tbody>";
            int ListaNoCompletadaQTY = 0, ListaNoCompletadaUnidades = 0;
            List<string> ClientesListaNoCompletada = new List<string>();


            TimeSpan diferencia;
            DateTime hoy = DateTime.Now;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            //Todo
            var worksheet0 = package.Workbook.Worksheets.Add("Todo");
            int fila = 1;

            worksheet0.Cells[fila, 1].Value = "Cuenta Cliente";
            worksheet0.Cells[fila, 2].Value = "Nombre Cliente";
            worksheet0.Cells[fila, 3].Value = "Fecha Ingreso Pedido";
            worksheet0.Cells[fila, 4].Value = "Pedido Venta";
            worksheet0.Cells[fila, 5].Value = "Estado Pedido";
            worksheet0.Cells[fila, 6].Value = "Lista Empaque";
            worksheet0.Cells[fila, 7].Value = "Fecha Generacion Lista Empaque";
            worksheet0.Cells[fila, 8].Value = "Fecha Lista Empaque Completa";
            worksheet0.Cells[fila, 9].Value = "Albaran";
            worksheet0.Cells[fila, 10].Value = "Fecha Albaran";
            worksheet0.Cells[fila, 11].Value = "Factura";
            worksheet0.Cells[fila, 12].Value = "Fecha Factura";
            worksheet0.Cells[fila, 13].Value = "Fecha Recepcion CD";
            worksheet0.Cells[fila, 14].Value = "Fecha Despacho";
            worksheet0.Cells[fila, 15].Value = "Ubicacion";
            worksheet0.Cells[fila, 16].Value = "Cajas";
            worksheet0.Cells[fila, 17].Value = "Cantidad";
            worksheet0.Cells[fila, 18].Value = "Responsable";
            worksheet0.Cells[fila, 19].Value = "Lote";
            worksheet0.Cells[fila, 20].Value = "Tienda";

            fila++;

            Pedidos.ForEach(element => {
                    worksheet0.Cells[fila, 1].Value = element.CuentaCliente;
                    worksheet0.Cells[fila, 2].Value = element.NombreCliente;
                    worksheet0.Cells[fila, 3].Value = element.FechaIngresoPedido == FechaVacia ? "" : element.FechaIngresoPedido;
                    worksheet0.Cells[fila, 4].Value = element.PedidoVenta;
                    worksheet0.Cells[fila, 5].Value = element.EstadoPedido;
                    worksheet0.Cells[fila, 6].Value = element.ListaEmpaque;
                    worksheet0.Cells[fila, 7].Value = element.FechaGeneracionListaEmpaque == FechaVacia ? "" : element.FechaGeneracionListaEmpaque;
                    worksheet0.Cells[fila, 8].Value = element.FechaListaEmpaqueCompletada == FechaVacia ? "" : element.FechaListaEmpaqueCompletada;
                    worksheet0.Cells[fila, 9].Value = element.Albaran;
                    worksheet0.Cells[fila, 10].Value = element.FechaAlbaran == FechaVacia ? "" : element.FechaAlbaran;
                    worksheet0.Cells[fila, 11].Value = element.Factura;
                    worksheet0.Cells[fila, 12].Value = element.FechaFactura == FechaVacia ? "" : element.FechaFactura;
                    worksheet0.Cells[fila, 13].Value = element.FechaRececpcionCD == FechaVacia ? "" : element.FechaRececpcionCD;
                    worksheet0.Cells[fila, 14].Value = element.FechaDespacho == FechaVacia ? "" : element.FechaDespacho;
                    worksheet0.Cells[fila, 15].Value = element.Ubicacion;
                    worksheet0.Cells[fila, 16].Value = element.Cajas;
                    worksheet0.Cells[fila, 17].Value = element.QTY;
                    worksheet0.Cells[fila, 18].Value = element.Responsable;
                    worksheet0.Cells[fila, 19].Value = element.BFPSEASONID;
                    worksheet0.Cells[fila, 20].Value = element.Tienda;
                fila++;
                });

            worksheet0.Cells[1, 3, fila - 1, 3].Style.Numberformat.Format = "dd/MM/yyyy";
            worksheet0.Cells[1, 7, fila - 1, 8].Style.Numberformat.Format = "dd/MM/yyyy";
            worksheet0.Cells[1, 10, fila - 1, 10].Style.Numberformat.Format = "dd/MM/yyyy";
            worksheet0.Cells[1, 12, fila - 1, 14].Style.Numberformat.Format = "dd/MM/yyyy";

            var rangeTable0 = worksheet0.Cells[1, 1, fila - 1, 20];
            var table0 = worksheet0.Tables.Add(rangeTable0, "Todo");
            table0.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;
            worksheet0.Cells.AutoFitColumns();

            //pendientes
            var worksheet = package.Workbook.Worksheets.Add("Pendiente");
            fila = 1;

            worksheet.Cells[fila, 1].Value = "Cuenta Cliente";
            worksheet.Cells[fila, 2].Value = "Nombre Cliente";
            worksheet.Cells[fila, 3].Value = "Fecha Ingreso Pedido";
            worksheet.Cells[fila, 4].Value = "Pedido Venta";
            worksheet.Cells[fila, 5].Value = "Estado Pedido";

            fila++;


            Pedidos.FindAll(x => x.ListaEmpaque == "" && x.Albaran == ""&& x.Factura == ""&& x.Ubicacion == "")
                .ForEach(element =>{
                    worksheet.Cells[fila, 1].Value = element.CuentaCliente;
                    worksheet.Cells[fila, 2].Value = element.NombreCliente;
                    worksheet.Cells[fila, 3].Value = element.FechaIngresoPedido == FechaVacia ? "": element.FechaIngresoPedido;
                    worksheet.Cells[fila, 4].Value = element.PedidoVenta;
                    worksheet.Cells[fila, 5].Value = element.EstadoPedido;
                    fila++;
                });

            worksheet.Cells[1, 3, fila - 1, 3].Style.Numberformat.Format = "dd/MM/yyyy";

            var rangeTable = worksheet.Cells[1, 1, fila-1, 5];
            var table = worksheet.Tables.Add(rangeTable, "Pendiente");
            table.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;
            worksheet.Cells.AutoFitColumns();

            //Lista Empaque
            var worksheet2 = package.Workbook.Worksheets.Add("ListaEmpaque");
             fila = 1;

            worksheet2.Cells[fila, 1].Value = "Cuenta Cliente";
            worksheet2.Cells[fila, 2].Value = "Nombre Cliente";
            worksheet2.Cells[fila, 3].Value = "Fecha Ingreso Pedido";
            worksheet2.Cells[fila, 4].Value = "Pedido Venta";
            worksheet2.Cells[fila, 5].Value = "Estado Pedido";
            worksheet2.Cells[fila, 6].Value = "Lista Empaque";
            worksheet2.Cells[fila, 7].Value = "Fecha Generacion Lista Empaque";
            worksheet2.Cells[fila, 8].Value = "Fecha Lista Empaque Completa";

            fila++;

            Pedidos.FindAll(x => x.ListaEmpaque != "" && x.Albaran == "" && x.Factura == "" && x.Ubicacion == "")
                .ForEach(element => {
                    worksheet2.Cells[fila, 1].Value = element.CuentaCliente;
                    worksheet2.Cells[fila, 2].Value = element.NombreCliente;
                    worksheet2.Cells[fila, 3].Value = element.FechaIngresoPedido == FechaVacia ? "" : element.FechaIngresoPedido;
                    worksheet2.Cells[fila, 4].Value = element.PedidoVenta;
                    worksheet2.Cells[fila, 5].Value = element.EstadoPedido;
                    worksheet2.Cells[fila, 6].Value = element.ListaEmpaque;
                    worksheet2.Cells[fila, 7].Value = element.FechaGeneracionListaEmpaque == FechaVacia ? "" : element.FechaGeneracionListaEmpaque;
                    worksheet2.Cells[fila, 8].Value = element.FechaListaEmpaqueCompletada == FechaVacia ? "" : element.FechaListaEmpaqueCompletada;
                    fila++;

                    if(element.FechaListaEmpaqueCompletada == FechaVacia)
                    {
                        diferencia = hoy - element.FechaGeneracionListaEmpaque ;
                        if(diferencia.TotalHours > 72)
                        {
                            ListaNoCompletadaTableHTML += "<tr><td>" + element.PedidoVenta + "</td><td>" + element.CuentaCliente + " " + element.NombreCliente + "</td><td>" + element.Responsable + "</td><td>" + element.ListaEmpaque + "</td><td>" + element.FechaGeneracionListaEmpaque + "</td><td>" + element.QTY + "</td><td>" + Convert.ToInt32(diferencia.TotalDays) + "</td></tr>";
                            ListaNoCompletadaQTY++;
                            ListaNoCompletadaUnidades += element.QTY;
                            if (!ClientesListaNoCompletada.Contains(element.CuentaCliente))
                            {
                                ClientesListaNoCompletada.Add(element.CuentaCliente);
                            }
                        }
                    }
                    else
                    {
                        diferencia = hoy - element.FechaListaEmpaqueCompletada;
                        ListaCompletadaTableHTML += "<tr><td>"+ element.PedidoVenta + "</td><td>" + element.CuentaCliente + " " + element.NombreCliente + "</td><td>" + element.Responsable + "</td><td>" + element.ListaEmpaque + "</td><td>" + element.FechaListaEmpaqueCompletada + "</td><td>" + element.QTY + "</td><td>" + Convert.ToInt32(diferencia.TotalDays) + "</td></tr>";
                        ListaCompletadaQTY++;
                        ListaCompletadaUnidades += element.QTY;

                        if (!ClientesListaCompletada.Contains(element.CuentaCliente))
                        {
                            ClientesListaCompletada.Add(element.CuentaCliente);
                        }
                    }
                });

            worksheet2.Cells[1, 3, fila - 1, 3].Style.Numberformat.Format = "dd/MM/yyyy";
            worksheet2.Cells[1, 7, fila - 1, 8].Style.Numberformat.Format = "dd/MM/yyyy";


            var rangeTable2 = worksheet2.Cells[1, 1, fila - 1, 8];
            var table2 = worksheet2.Tables.Add(rangeTable2, "ListaEmpaque");
            table2.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;
            worksheet2.Cells.AutoFitColumns();

            //Albaran
            var worksheet3 = package.Workbook.Worksheets.Add("Albaran");
            fila = 1;

            worksheet3.Cells[fila, 1].Value = "Cuenta Cliente";
            worksheet3.Cells[fila, 2].Value = "Nombre Cliente";
            worksheet3.Cells[fila, 3].Value = "Fecha Ingreso Pedido";
            worksheet3.Cells[fila, 4].Value = "Pedido Venta";
            worksheet3.Cells[fila, 5].Value = "Estado Pedido";
            worksheet3.Cells[fila, 6].Value = "Lista Empaque";
            worksheet3.Cells[fila, 7].Value = "Fecha Generacion Lista Empaque";
            worksheet3.Cells[fila, 8].Value = "Fecha Lista Empaque Completa";
            worksheet3.Cells[fila, 9].Value = "Albaran";
            worksheet3.Cells[fila, 10].Value = "Fecha Albaran";

            fila++;

            Pedidos.FindAll(x => x.Albaran != "" && x.Factura == "" && x.Ubicacion == "")
                .ForEach(element => {
                    worksheet3.Cells[fila, 1].Value = element.CuentaCliente;
                    worksheet3.Cells[fila, 2].Value = element.NombreCliente;
                    worksheet3.Cells[fila, 3].Value = element.FechaIngresoPedido == FechaVacia ? "" : element.FechaIngresoPedido;
                    worksheet3.Cells[fila, 4].Value = element.PedidoVenta;
                    worksheet3.Cells[fila, 5].Value = element.EstadoPedido;
                    worksheet3.Cells[fila, 6].Value = element.ListaEmpaque;
                    worksheet3.Cells[fila, 7].Value = element.FechaGeneracionListaEmpaque == FechaVacia ? "" : element.FechaGeneracionListaEmpaque;
                    worksheet3.Cells[fila, 8].Value = element.FechaListaEmpaqueCompletada == FechaVacia ? "" : element.FechaListaEmpaqueCompletada;
                    worksheet3.Cells[fila, 9].Value = element.Albaran;
                    worksheet3.Cells[fila, 10].Value = element.FechaAlbaran == FechaVacia ? "" : element.FechaAlbaran;
                    fila++;

                    diferencia = hoy - element.FechaAlbaran;
                    AlbaranTableHTML += "<tr><td>" + element.PedidoVenta + "</td><td>" + element.CuentaCliente + " " + element.NombreCliente + "</td><td>" + element.Responsable + "</td><td>" + element.ListaEmpaque + "</td><td>" + element.Albaran + "</td><td>" + element.FechaAlbaran + "</td><td>" + element.QTY + "</td><td>" + Convert.ToInt32(diferencia.TotalDays) + "</td></tr>";
                    AlbaranQTY++;
                    AlbaranUnidades += element.QTY;

                    if (!ClientesAlbaran.Contains(element.CuentaCliente))
                    {
                        ClientesAlbaran.Add(element.CuentaCliente);
                    }
                });

            worksheet3.Cells[1, 3, fila - 1, 3].Style.Numberformat.Format = "dd/MM/yyyy";
            worksheet3.Cells[1, 7, fila - 1, 8].Style.Numberformat.Format = "dd/MM/yyyy";
            worksheet3.Cells[1, 10, fila - 1, 10].Style.Numberformat.Format = "dd/MM/yyyy";



            var rangeTable3 = worksheet3.Cells[1, 1, fila - 1, 10];
            var table3 = worksheet3.Tables.Add(rangeTable3, "Albaran");
            table3.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;
            worksheet3.Cells.AutoFitColumns();

            //Factura
            var worksheet4 = package.Workbook.Worksheets.Add("Factura");
            fila = 1;

            worksheet4.Cells[fila, 1].Value = "Cuenta Cliente";
            worksheet4.Cells[fila, 2].Value = "Nombre Cliente";
            worksheet4.Cells[fila, 3].Value = "Fecha Ingreso Pedido";
            worksheet4.Cells[fila, 4].Value = "Pedido Venta";
            worksheet4.Cells[fila, 5].Value = "Estado Pedido";
            worksheet4.Cells[fila, 6].Value = "Lista Empaque";
            worksheet4.Cells[fila, 7].Value = "Fecha Generacion Lista Empaque";
            worksheet4.Cells[fila, 8].Value = "Fecha Lista Empaque Completa";
            worksheet4.Cells[fila, 9].Value = "Albaran";
            worksheet4.Cells[fila, 10].Value = "Fecha Albaran";
            worksheet4.Cells[fila, 11].Value = "Factura";
            worksheet4.Cells[fila, 12].Value = "Fecha Factura";

            fila++;

            Pedidos.FindAll(x => x.Factura != "" && x.Ubicacion == "")
                .ForEach(element => {
                    worksheet4.Cells[fila, 1].Value = element.CuentaCliente;
                    worksheet4.Cells[fila, 2].Value = element.NombreCliente;
                    worksheet4.Cells[fila, 3].Value = element.FechaIngresoPedido == FechaVacia ? "" : element.FechaIngresoPedido;
                    worksheet4.Cells[fila, 4].Value = element.PedidoVenta;
                    worksheet4.Cells[fila, 5].Value = element.EstadoPedido;
                    worksheet4.Cells[fila, 6].Value = element.ListaEmpaque;
                    worksheet4.Cells[fila, 7].Value = element.FechaGeneracionListaEmpaque == FechaVacia ? "" : element.FechaGeneracionListaEmpaque;
                    worksheet4.Cells[fila, 8].Value = element.FechaListaEmpaqueCompletada == FechaVacia ? "" : element.FechaListaEmpaqueCompletada;
                    worksheet4.Cells[fila, 9].Value = element.Albaran;
                    worksheet4.Cells[fila, 10].Value = element.FechaAlbaran == FechaVacia ? "" : element.FechaAlbaran;
                    worksheet4.Cells[fila, 11].Value = element.Factura;
                    worksheet4.Cells[fila, 12].Value = element.FechaFactura == FechaVacia ? "" : element.FechaFactura;
                    fila++;
                });

            worksheet4.Cells[1, 3, fila - 1, 3].Style.Numberformat.Format = "dd/MM/yyyy";
            worksheet4.Cells[1, 7, fila - 1, 8].Style.Numberformat.Format = "dd/MM/yyyy";
            worksheet4.Cells[1, 10, fila - 1, 10].Style.Numberformat.Format = "dd/MM/yyyy";
            worksheet4.Cells[1, 12, fila - 1, 12].Style.Numberformat.Format = "dd/MM/yyyy";

            var rangeTable4 = worksheet4.Cells[1, 1, fila - 1, 12];
            var table4 = worksheet4.Tables.Add(rangeTable4, "Factura");
            table4.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;
            worksheet4.Cells.AutoFitColumns();

            Byte[]  fileContents = package.GetAsByteArray();
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(VariablesGlobales.Correo);

                var correos = await getCorreoCiclicoTelaTrackingPedido();

                foreach (IM_WMS_Correos_DespachoPTDTO correo in correos)
                {
                    mail.To.Add(correo.Correo);
                }

                //mail.To.Add("bavila@intermoda.com.hn");
                mail.Subject = "Tracking Pedidos";
                mail.IsBodyHtml = true;

                string body = "<h1>Resumen</h1>";
                
                

                body += "<table border='2'><thead><tr><th>Resumen</th><th>Clientes</th><th>Cantidad</th><th>Unidades</th></tr><thead>";
                body += "<tbody>";
                body += "<tr><td>Albaranes Pendientes de Facturar</td><td>" + ClientesAlbaran.Count() + "</td><td>" + AlbaranQTY + "</td><td>" + AlbaranUnidades + "</td></tr>";
                body += "<tr><td>Lista Empaque pendientes de albaranar y facturar</td><td>" + ClientesListaCompletada.Count() + "</td><td>" + ListaCompletadaQTY + "</td><td>" + ListaCompletadaUnidades + "</td></tr>";
                body += "<tr><td>Lista Empaque Pendiente de empacar mayor a 3 dias</td><td>" + ClientesListaNoCompletada.Count() + "</td><td>" + ListaNoCompletadaQTY+ "</td><td>" + ListaNoCompletadaUnidades + "</td></tr>";
                body += "</tbody></table>";
                if(AlbaranQTY > 0)
                {
                    body += AlbaranTableHTML + "</table>";
                }
                if (ListaCompletadaQTY > 0)
                {
                    body += ListaCompletadaTableHTML + "</table>";
                }
                if (ListaNoCompletadaQTY > 0)
                {
                    body += ListaNoCompletadaTableHTML + "</table>";
                }


                mail.Body = body;

                using (MemoryStream ms = new MemoryStream(fileContents))
                {
                    DateTime date = DateTime.Now;
                    string fechah = date.Day + "_" + date.Month + "_" + date.Year;
                    Attachment attachment = new Attachment(ms, "TrackingPedidos-" + fechah + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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

            return "OK";
        } 
        public async Task<List<IM_WMS_Correos_DespachoPTDTO>> getCorreoCiclicoTelaTrackingPedido()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>{};        

            List<IM_WMS_Correos_DespachoPTDTO> correos = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Correos_DespachoPTDTO>("[IM_WMS_ObtenerCorreosTrackingPedido]", parametros);
            return correos;
        }

        public async Task<List<IM_WMS_GenerarDetalleFacturas>> getObtenerDetalletrackingPedidos(TrackingPedidosFiltro filtro)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                new SqlParameter("@cuentaCliente",filtro.CuentaCliente),
                new SqlParameter("@pedido",filtro.Pedido),
                new SqlParameter("@ListaEmpaque",filtro.ListaEmpaque),
                new SqlParameter("@albaran",filtro.Albaran),
                new SqlParameter("@factura",filtro.Factura),
                new SqlParameter("@page",filtro.page),
                new SqlParameter("@size",filtro.size)

            };

            List<IM_WMS_GenerarDetalleFacturas> correos = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_GenerarDetalleFacturas>("[IM_WMS_ObtenerDetalleTrackingPedidos]", parametros);
            return correos;
        }

        public async Task<List<IM_WMS_ObtenerDetalleAdutoriaDenim>> Get_ObtenerDetalleAdutoriaDenims(string OP, int Caja, string Ubicacion, string Usuario)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                new SqlParameter("@OP",OP=="-"?"":OP),
                new SqlParameter("@Caja",Caja),
                new SqlParameter("@Ubicacion",Ubicacion=="-"?"":Ubicacion),
                new SqlParameter("@usuario",Usuario=="-"?"":Usuario)            

            };

            List<IM_WMS_ObtenerDetalleAdutoriaDenim> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_ObtenerDetalleAdutoriaDenim>("[IM_WMS_ObtenerDetalleAdutoriaDenim]", parametros);
            return resp;
        }

        public async Task<IM_WMS_insertDetalleAdutoriaDenim> GetInsertDetalleAdutoriaDenim(int ID, int AuditoriaID)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                new SqlParameter("@id",ID),
                new SqlParameter("@auditoriaID",AuditoriaID),
            };

            IM_WMS_insertDetalleAdutoriaDenim resp = await executeProcedure.ExecuteStoredProcedure<IM_WMS_insertDetalleAdutoriaDenim>("[IM_WMS_insertDetalleAdutoriaDenim]", parametros);
            return resp;
        }

        public async Task<string> getEnviarCorreoAuditoriaDenim(string Ubicacion,string usuario)
        {
            var data = await Get_ObtenerDetalleAdutoriaDenims("-", 0, Ubicacion, usuario);

            DateTime FechaVacia = new DateTime(1900, 01, 01);
            int diferencias = 0;
            int cajas = 0;
            int unidades = 0;
            DateTime fechaINI = new DateTime();
            DateTime FechaFin = new DateTime();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            //Todo
            var worksheet0 = package.Workbook.Worksheets.Add("Todo");
            int fila = 1;

            worksheet0.Cells[fila, 1].Value = "OP";
            worksheet0.Cells[fila, 2].Value = "Numero de Caja";
            worksheet0.Cells[fila, 3].Value = "Articulo";
            worksheet0.Cells[fila, 4].Value = "Talla";
            worksheet0.Cells[fila, 5].Value = "Color";
            worksheet0.Cells[fila, 6].Value = "Recibido";
            worksheet0.Cells[fila, 7].Value = "Auditado";
            worksheet0.Cells[fila, 8].Value = "Diferencia";
            worksheet0.Cells[fila, 9].Value = "Fecha Inicio";
            worksheet0.Cells[fila, 10].Value = "Fecha Final";
            worksheet0.Cells[fila, 11].Value = "Lote";
            
            fila++;

            data.ForEach(element => {
                worksheet0.Cells[fila, 1].Value = element.OP;
                worksheet0.Cells[fila, 2].Value = element.NumeroCaja;
                worksheet0.Cells[fila, 3].Value = element.Articulo;
                worksheet0.Cells[fila, 4].Value = element.Talla;
                worksheet0.Cells[fila, 5].Value = element.COlor;
                worksheet0.Cells[fila, 6].Value = element.Cantidad;
                worksheet0.Cells[fila, 7].Value = element.Auditado;
                worksheet0.Cells[fila, 8].Value = element.Cantidad - element.Auditado;
                worksheet0.Cells[fila, 9].Value = element.FechaInicio == FechaVacia ? "" : element.FechaInicio;
                worksheet0.Cells[fila, 10].Value = element.FechaFin == FechaVacia ? "" : element.FechaFin;
                worksheet0.Cells[fila, 11].Value = element.Lote;
                diferencias += element.Cantidad - element.Auditado;

                if (fila == 2)
                {
                    fechaINI = element.FechaInicio;
                    FechaFin = element.FechaFin;
                }
                else
                {
                    if(fechaINI > element.FechaInicio && element.FechaInicio != FechaVacia)
                    {
                        fechaINI = element.FechaInicio;
                    }

                    if (FechaFin < element.FechaFin)
                    {
                        FechaFin = element.FechaFin;
                    }
                }
                if(element.Auditado != 0)
                {
                    cajas++;
                }
                unidades += element.Auditado;
                fila++;
            });
            worksheet0.Cells[fila, 7].Value = "Diferencias";
            worksheet0.Cells[fila, 7].Style.Font.Bold = true;
            worksheet0.Cells[fila, 8].Formula = $"SUM(H2:H{fila - 1})";
            worksheet0.Cells[fila, 8].Style.Font.Bold = true;
            worksheet0.Cells[1, 9, fila - 1, 10].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";

            var rangeTable0 = worksheet0.Cells[1, 1, fila - 1, 11];
            var table0 = worksheet0.Tables.Add(rangeTable0, "Todo");
            table0.TableStyle = OfficeOpenXml.Table.TableStyles.Light11;
            worksheet0.Cells.AutoFitColumns();

           


            Byte[] fileContents = package.GetAsByteArray();
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(VariablesGlobales.Correo);

                var correos = await getCorreosRecepcionUbicacionCajas();

                foreach (IM_WMS_Correos_DespachoPTDTO correo in correos)
                {
                    mail.To.Add(correo.Correo);
                }

                //mail.To.Add("bavila@intermoda.com.hn");

                mail.Subject = "Auditoria Denim " + Ubicacion ;
                mail.IsBodyHtml = false;
                
                mail.Body = "Auditoria Denim " + Ubicacion + " usuario: " + usuario+ " Diferencias: "+diferencias + " Cajas: "+cajas + " unidades: "+unidades +" Inicio: "+fechaINI+ " Fin: " +FechaFin;                
               

                using (MemoryStream ms = new MemoryStream(fileContents))
                {
                    DateTime date = DateTime.Now;
                    string fechah = date.Day + "_" + date.Month + "_" + date.Year;
                    Attachment attachment = new Attachment(ms, "AuditoriaDenim_"+ Ubicacion+".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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

                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parametros = new List<SqlParameter> {                
                new SqlParameter("@Ubicacion",Ubicacion)
                };
                List<IM_WMS_ObtenerDetalleAdutoriaDenim> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_ObtenerDetalleAdutoriaDenim>("[IM_WMS_EnviarAuditoriaDenim]", parametros);
                

            }
            catch (Exception err)
            {
                return err.ToString();
            }

            return "OK";
        }

        //Reciclaje de cajas
        public async Task<List<IM_WMS_CentroCostoReciclajeCajas>> GetCentroCostoReciclajeCajas()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> { };

            List<IM_WMS_CentroCostoReciclajeCajas> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_CentroCostoReciclajeCajas>("[IM_WMS_CentroCostoReciclajeCajas]", parametros);
            return resp;
        }

        public async Task<IM_WMS_InsertCajasRecicladashistorico> GetInsertCajasRecicladashistorico(string Camion, string Chofer, string CentroCostos, int QTY, string usuario, string diario)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                new SqlParameter("@Camion",Camion),
                new SqlParameter("@Chofer",Chofer),
                new SqlParameter("@CentroCostos",CentroCostos),
                new SqlParameter("@QTY",QTY),
                new SqlParameter("@usuario",usuario),
                new SqlParameter("@Diario",diario),

            };

            IM_WMS_InsertCajasRecicladashistorico resp = await executeProcedure.ExecuteStoredProcedure<IM_WMS_InsertCajasRecicladashistorico>("[IM_WMS_InsertCajasRecicladashistorico]", parametros);
            return resp;
        }

        public async Task<List<IM_WMS_InsertCajasRecicladashistorico>> GetCajasRecicladasPendiente()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> { };

            List<IM_WMS_InsertCajasRecicladashistorico> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_InsertCajasRecicladashistorico>("[IM_WMS_DespachoCajasReciclajePendiente]", parametros);
            return resp;
        }

        //Devoluciones
        public async Task<List<IM_WMS_Devolucion_Busqueda>> getDevolucionesEVA(string filtro, int page, int size,int estado)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                new SqlParameter("@Filtro",filtro),
                new SqlParameter("@page",page),
                new SqlParameter("@size",size),
                new SqlParameter("@estado",estado)

            };

            List<IM_WMS_Devolucion_Busqueda> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Devolucion_Busqueda>("[IM_WMS_Devolucion_Busqueda]", parametros);
            return resp;
        }

        public async Task<List<IM_WMS_Devolucion_Detalle_RecibirPlanta>> getDevolucionDetalle(int id)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                new SqlParameter("@id",id)

            };

            List<IM_WMS_Devolucion_Detalle_RecibirPlanta> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Devolucion_Detalle_RecibirPlanta>("[IM_WMS_Devolucion_Detalle_RecibirPlanta]", parametros);
            return resp;
        }

        public async Task<List<IM_WMS_Devolucion_Detalle_RecibirPlanta>> getInsertDevolucionRecibidoEnviado(int id, int qty, string tipo)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                new SqlParameter("@id",id),
                new SqlParameter("@qty",qty),
                new SqlParameter("@tipo",tipo)


            };

            List<IM_WMS_Devolucion_Detalle_RecibirPlanta> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Devolucion_Detalle_RecibirPlanta>("[IM_WMS_Devolucion_Recibido_Enviado_QTY]", parametros);
            return resp;
        }

        public async Task<IM_WMS_Devolucion_Busqueda> getActualizarEstadoDevolucion(int id, string estado,string usuario,string camion)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                new SqlParameter("@id",id),
                new SqlParameter("@estado",estado),
                new SqlParameter("@usuario",usuario),
                new SqlParameter("@camion",camion)


            };

            IM_WMS_Devolucion_Busqueda resp = await executeProcedure.ExecuteStoredProcedure<IM_WMS_Devolucion_Busqueda>("[IM_WMS_UpdateEstadoDevolucion]", parametros);

            if(resp.Descricpcion== "Rechazado" || resp.Descricpcion == "Recibido en Planta")
            {
                string html = "<h3>Estimado/a "+resp.Asesor+",</h3>";
                if(resp.Descricpcion == "Recibido en Planta")
                {
                    html += "<p>Nos complace informarle que la devolución " + resp.NumDevolucion +":" + resp.NumeroRMA + " ha sido " + resp.Descricpcion + ".</p>";
                }
                else
                {
                    html += "<p>Lamentamos informarle que la devolución " + resp.NumDevolucion + ":" + resp.NumeroRMA + " ha sido " + " ha sido rechazada debido a que las unidades recibidas no concuerdan con las unidades declaradas en la devolución.</p>";
                }
                html += "<table border='2'><caption><h2>Detalle Devolucion</h2></caption><thead><th>Articulo</th><th>Talla</th><th>Color</th><th>Cantidad</th><th>Recibida</th><th>Diferencia</th></thead><tbody>";
                var datos = await getDevolucionDetalle(resp.ID);
                datos.ForEach(element =>
                {
                    if((element.RecibidaPlanta - element.Cantidad) !=0)
                    {
                        html += "<tr style='background-color:#FF6600;'>";
                    }else{
                        html += "<tr>";
                    }

                    html +="<td>"+ element.Articulo +"</td>";
                    html += "<td>" + element.Talla + "</td>";
                    html += "<td>" + element.Color + "</td>";
                    html += "<td>" + element.Cantidad + "</td>";
                    html += "<td>" + element.RecibidaPlanta + "</td>";
                    html += "<td>" + (element.RecibidaPlanta - element.Cantidad ) + "</td>";
                    html += "</tr>";
                });

                html += "</tbody></table>";
                try
                {
                    MailMessage mail = new MailMessage();

                    mail.From = new MailAddress(VariablesGlobales.Correo);

                    var correos = await getCorreosDevolucion(id);

                     foreach (IM_WMS_Correos_DespachoPTDTO correo in correos)
                     {
                         mail.To.Add(correo.Correo);
                     }

                    //mail.To.Add("bavila@intermoda.com.hn");

                    if (resp.Descricpcion == "Recibido en Planta")
                    {
                        mail.Subject = "Confirmación de recepción de la devolución "+resp.NumDevolucion + ":" + resp.NumeroRMA + " ha sido " + " en Planta";
                        html += "<p>Saludos</p>";

                    }
                    else
                    {
                        mail.Subject = "Notificación de la devolución "+resp.NumDevolucion + ":" + resp.NumeroRMA + " ha sido " + " rechazada: Discrepancia en unidades";
                        html += "<p>Por favor, revise los detalles y comuníquese con nosotros si necesita más información o desea discutir esta situación. Estamos disponibles para ayudarle con cualquier aclaración.</p><p>Saludos</p>";
                    }

                    
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
                    return null;
                }
            }

            if(resp.Descricpcion == "Recibido CD")
            {
                string html = "<h3>Estimado/a " + resp.Asesor + ",</h3>";
                html += "<p>Nos complace informarle que la devolución " + resp.NumDevolucion + ":" + resp.NumeroRMA + " ha sido " + resp.Descricpcion + " .</p>";
                html += "<p>Saludos</p>";


                try
                {
                    MailMessage mail = new MailMessage();

                    mail.From = new MailAddress(VariablesGlobales.Correo);

                    var correos = await getCorreosDevolucion(id);

                    foreach (IM_WMS_Correos_DespachoPTDTO correo in correos)
                    {
                        mail.To.Add(correo.Correo);
                    }

                    //mail.To.Add("bavila@intermoda.com.hn");

                    mail.Subject = "Confirmación de recepción de la devolución " + resp.NumDevolucion + " en OfiBodegas SB";
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
                    return null;
                }
            }
            return resp;
        }

        public async Task<List<IM_WMS_Correos_DespachoPTDTO>> getCorreosDevolucion(int id)
        {
            
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@id",id)
            };

            List<IM_WMS_Correos_DespachoPTDTO> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Correos_DespachoPTDTO>("[IM_WMS_ObtenerCorreosDevolucion]", parametros);

            return response;
            
        }
        public async Task<List<IM_WMS_Devolucion_Detalle_RecibirPlanta>> getDetalleDevolucionAuditoria(int id)
        {
            List<IM_WMS_Devolucion_Detalle_RecibirPlanta> lista = new List<IM_WMS_Devolucion_Detalle_RecibirPlanta>();

            var datos = await getDevolucionDetalle(id);

            
            foreach(IM_WMS_Devolucion_Detalle_RecibirPlanta element in datos)
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parametros = new List<SqlParameter> {
                    new SqlParameter("@id",element.ID),
                    new SqlParameter("@qty",element.Cantidad)
                };

                List<DefectosDevolucion> resp = await executeProcedure.ExecuteStoredProcedureList<DefectosDevolucion>("[IM_WMS_ObteneDetalleDefectosDevolucion]", parametros);
                element.Defecto = resp.ToArray();
                lista.Add(element);
            }

            return lista;



        }

        public async Task<List<DefectosAuditoria>> GetObtenerDefectosDevolucions(int id)
        {
            List<DefectosAuditoria> areas = new List<DefectosAuditoria>();
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                new SqlParameter("@id",id),
            };

            List<selectList> resp = await executeProcedure.ExecuteStoredProcedureList<selectList>("[IM_WMS_obtenerAreasAuditoriaDevolucion]", parametros);
            foreach (var area in resp)
                //resp.ForEach(async (area) =>
            {
                DefectosAuditoria tmp = new DefectosAuditoria();
                tmp.id = area.id;
                tmp.key = area.key;
                tmp.value = area.value;

                parametros = new List<SqlParameter> {
                    new SqlParameter("@idArea",area.id),
                };
                List<selectList> operaciones = await executeProcedure.ExecuteStoredProcedureList<selectList>("[IM_WMS_obtenerOperacionesAuditoriaDevolucion]", parametros);
                tmp.Operacion = operaciones.ToArray() ;

                parametros = new List<SqlParameter> {
                    new SqlParameter("@idArea",area.id),
                };
                List<selectList> defectos = await executeProcedure.ExecuteStoredProcedureList<selectList>("[IM_WMS_obtenerDefectosAuditoriaDevolucion]", parametros);
                tmp.Defecto = defectos.ToArray();

                areas.Add(tmp);

            }

            return areas;
        }

        public async Task<IM_WMS_UpdateDetalleDefectoDevolucion> getActualizarDetalleDefectoDevolucion(int id, int idDefecto, string tipo,bool Reparacion,int operacion,int qty)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                    new SqlParameter("@id",id),
                    new SqlParameter("@idDefecto",idDefecto),
                    new SqlParameter("@tipo",tipo),
                    new SqlParameter("@reparacion",Reparacion),
                    new SqlParameter("@idOperacion",operacion),
                    new SqlParameter("@QTY",qty)

            };

            IM_WMS_UpdateDetalleDefectoDevolucion resp = await executeProcedure.ExecuteStoredProcedure<IM_WMS_UpdateDetalleDefectoDevolucion>("[IM_WMS_UpdateDetalleDefectoDevolucion]", parametros);
            return resp;
        }

        public async Task<List<IM_WMS_Devolucion_Busqueda>> getObtenerDevolucionTracking(string filtro, int page, int size)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                    new SqlParameter("@filtro",filtro),
                    new SqlParameter("@page",page),
                    new SqlParameter("@size",size)
            };

            List<IM_WMS_Devolucion_Busqueda> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Devolucion_Busqueda>("[IM_WMS_DevolucionTracking]", parametros);
            return resp;
        }

        public async Task<string> getImprimirEtiquetasDevolucion(int id, string NumDevolucion, int CajaPrimera, int CajaIrregular,string usuario)
        {
            var devolucion =await  getDevolucionesEVA(NumDevolucion, 1, 1, 2);
            
            

            for (int i = 1; i <= CajaPrimera; i++)
            {
                IngresarCaja(id, i, "PRIMERA");
                await imprimirEtiquetaDevolucion(devolucion[0], i, "PRIMERA",CajaPrimera+ CajaIrregular,usuario);
            }

            for (int i = 1; i <= CajaIrregular; i++)
            {
                IngresarCaja(id, CajaPrimera+ i, "IRREGULAR");
                await imprimirEtiquetaDevolucion(devolucion[0], CajaPrimera + i, "IRREGULAR", CajaPrimera + CajaIrregular,usuario);
            }
            string OK = "OK";
            return OK;

        }

        public async void IngresarCaja(int id, int caja,string tipo)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                    new SqlParameter("@id",id),
                    new SqlParameter("@caja",caja),
                    new SqlParameter("@tipo",tipo)
            };

            IM_WMS_CrearCajaDevolucion resp = await executeProcedure.ExecuteStoredProcedure<IM_WMS_CrearCajaDevolucion>("[IM_WMS_CrearCajaDevolucion]", parametros);
            
        }

        public async Task<string> imprimirEtiquetaDevolucion(IM_WMS_Devolucion_Busqueda datos,int caja, string Tipo,int total,string usuario)
        {
            var empleado = await getNombreEmpleado(usuario);
            string etiqueta = "";
            etiqueta += "^XA^CF0,40";
            etiqueta += "^FO50,50^FDDevolucion: "+datos.NumDevolucion+"^FS";
            etiqueta += "^FO50,100^FDRMA: "+datos.NumeroRMA+"^FS";
            etiqueta += "^FO50,150^FDCantidad: "+datos.TotalUnidades+"^FS";
            etiqueta += "^FO50,200^FDAsesor: "+datos.Asesor+"^FS";
            etiqueta += "^CF0,250";
            etiqueta += "^FO125,400^FD"+caja.ToString("D2") + "/" + total.ToString("D2") + "^FS";
            etiqueta += "^BY3,2,120^FO125,700^BC^FD"+(datos.NumDevolucion.Length > 0 ? datos.NumDevolucion  : datos.NumeroRMA)+ ","+caja+"^FS";
            etiqueta += "^CF0,60^FO280,900^FD"+Tipo+"^FS";
            etiqueta += "^CF0,40^FO50,1100^FDAuditor: "+empleado.Nombre+"^FS"; //colocar al empleado
            etiqueta += "^FO50,1150^FDFecha: " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "^FS"; //colocar fecha correcta
            etiqueta += "^XZ";

            try
            {
                using (TcpClient client = new TcpClient(_ImpresoraDevolucion, 9100))//colocarIPImpresora
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

            return "OK";
        }

        public async Task<IM_WMS_CrearCajaDevolucion> getInsertarCajasDevolucion(string NumDevolucion, string usuario, int Caja)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                    new SqlParameter("@NumDevolucion",NumDevolucion),
                    new SqlParameter("@caja",Caja),
                    new SqlParameter("@usuario",usuario)
            };

            IM_WMS_CrearCajaDevolucion resp = await executeProcedure.ExecuteStoredProcedure<IM_WMS_CrearCajaDevolucion>("[IM_WMS_DevolucionPackingCaja]", parametros);
            return resp;
        }

        public async Task<List<IM_WMS_DevolucionCajasPacking>> getDevolucionCajasPacking()
        {

            List<IM_WMS_DevolucionCajasPacking> lista = new List<IM_WMS_DevolucionCajasPacking>();

            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> { };

            List<IM_WMS_Devolucion_Busqueda> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Devolucion_Busqueda>("[IM_WMS_obtenerDevolucionPacking]", parametros);
            foreach (var element in resp)
            {
                IM_WMS_DevolucionCajasPacking tmp = new IM_WMS_DevolucionCajasPacking();

                tmp.ID = element.ID;
                tmp.NumDevolucion = element.NumDevolucion;
                tmp.NumeroRMA = element.NumeroRMA;
                tmp.FechaCrea = element.FechaCrea;
                tmp.FechaCreacionAX = element.FechaCreacionAX;
                tmp.Asesor = element.Asesor;
                tmp.Descricpcion = element.Descricpcion ?? "";
                tmp.TotalUnidades = element.TotalUnidades;
                tmp.camion = element.Camion;
                
                var parametros2 = new List<SqlParameter> {
                    new SqlParameter("@id",element.ID)
                };

                List<IM_WMS_CrearCajaDevolucion> resp2 = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_CrearCajaDevolucion>("[IM_WMS_ObtenerCajasDevolucionPacking]", parametros2);
                tmp.cajas = resp2.ToArray();
                lista.Add(tmp);
            }
            
            return lista;
        }

        public async Task<string> postEnviarCorreoPackig(List<IM_WMS_Devolucion_Busqueda> data)
        {     
            string html = "<table border='2'><caption><h2>Despacho Devoluciones a Ofibodegas SB Camion: "+data[0].Camion+ "</h2></caption><thead><th>Asesor</th><th>Devolucion</th><th>RMA</th><th>Total Unidades</th></thead><tbody>";
            
            data.ForEach(element =>
            {
                html += "<tr>";         
                html += "<td>" + element.Asesor + "</td>";
                html += "<td>" + element.NumDevolucion + "</td>";
                html += "<td>" + element.NumeroRMA + "</td>";
                html += "<td>" + element.TotalUnidades + "</td>";                
                html += "</tr>";
            });

            html += "</tbody></table>";
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(VariablesGlobales.Correo);

                var correos = await getCorreosDevolucion(0);

                 foreach (IM_WMS_Correos_DespachoPTDTO correo in correos)
                 {
                     mail.To.Add(correo.Correo);
                 }

                //mail.To.Add("bavila@intermoda.com.hn");

                mail.Subject = "Despacho Devolucion Planta a CD";
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
                return "Error";
            }
            return "OK";
        }
        public async Task<List<IM_WMS_DevolucionCajasPacking>> getDevolucionCajasEnviadasCD()
        {

            List<IM_WMS_DevolucionCajasPacking> lista = new List<IM_WMS_DevolucionCajasPacking>();

            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> { };

            List<IM_WMS_Devolucion_Busqueda> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Devolucion_Busqueda>("[IM_WMS_obtenerDevolucionEnviadasCD]", parametros);
            foreach (var element in resp)
            {
                IM_WMS_DevolucionCajasPacking tmp = new IM_WMS_DevolucionCajasPacking();

                tmp.ID = element.ID;
                tmp.NumDevolucion = element.NumDevolucion;
                tmp.NumeroRMA = element.NumeroRMA;
                tmp.FechaCrea = element.FechaCrea;
                tmp.FechaCreacionAX = element.FechaCreacionAX;
                tmp.Asesor = element.Asesor;
                tmp.Descricpcion = element.Descricpcion ?? "";
                tmp.TotalUnidades = element.TotalUnidades;
                tmp.camion = element.Camion;

                var parametros2 = new List<SqlParameter> {
                    new SqlParameter("@id",element.ID)
                };

                List<IM_WMS_CrearCajaDevolucion> resp2 = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_CrearCajaDevolucion>("[IM_WMS_ObtenerCajasDevolucionPacking]", parametros2);
                tmp.cajas = resp2.ToArray();
                lista.Add(tmp);
            }

            return lista;
        }

        public async Task<IM_WMS_CrearCajaDevolucion> getInsertarCajasDevolucionRecibir(string NumDevolucion, string usuario, int Caja)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                    new SqlParameter("@NumDevolucion",NumDevolucion),
                    new SqlParameter("@caja",Caja),
                    new SqlParameter("@usuario",usuario)
            };

            IM_WMS_CrearCajaDevolucion resp = await executeProcedure.ExecuteStoredProcedure<IM_WMS_CrearCajaDevolucion>("[IM_WMS_DevolucionRecibirCaja]", parametros);
            return resp;
        }

        public async Task<string> postDevolucionConsolidada(List<DevolucionConsolidada> data)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                    new SqlParameter("@user",data[0].Usuario)
                };

            IM_WMS_Crear_Devolucion_Consolidada resp = await executeProcedure.ExecuteStoredProcedure<IM_WMS_Crear_Devolucion_Consolidada>("[IM_WMS_Crear_Devolucion_Consolidada]", parametros);

            data.ForEach(async(element) =>
            {
                var parametros2 = new List<SqlParameter>
                {
                    new SqlParameter("@numDevolucion",element.NumDevolucion),
                    new SqlParameter("@idConsolidado",resp.ID)

                };
                IM_WMS_Devolucion_Busqueda resp2 = await executeProcedure.ExecuteStoredProcedure<IM_WMS_Devolucion_Busqueda>("[IM_WMS_Agregar_Devolucion_Consolidada]", parametros2);

            });

            var empleado = await getNombreEmpleado(data[0].Usuario);

            int cont = 1;
            string encabezado = "^XA^CF0,30";

            string pie  = "^BY2,2,100^FO125,960^BC^FDCONSOLIDADO," + resp.ID + "^FS";
            pie += "^CF0,40^FO50,1110^FDAuditor: "+ empleado .Nombre+ "^FS"; 
            pie += "^FO50,1150^FDFecha: " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "^FS"; //colocar fecha correcta
            pie += "^XZ";
            int fila = 50;
            string cuerpo = "";

            
           
            foreach (var element in data)
            {
                var devolucion = await getDevolucionesEVA(element.NumDevolucion, 1, 1, 0);
                IngresarCaja(devolucion[0].ID, 1, "PRIMERA");

                cuerpo += "^FO50," + fila + "^FDDevolucion: "+element.NumDevolucion+"^FS";
                cuerpo += "^FO450," + fila + "^FDRMA: "+ devolucion[0].NumeroRMA+ "^FS";
                fila += 30;
                cuerpo += "^FO50," + fila + "^FDAsesor: " + devolucion[0].Asesor + "^FS";
                fila += 30;
                cuerpo += "^FO50," + fila + "^FDCantidad: " + devolucion[0].TotalUnidades + "^FS";
                fila += 30;
                cuerpo += "^FO50," + fila + "^GB700,3,3^FS";
                fila += 10;
                if(cont == 9)
                {
                    cont = 1;
                    fila = 50;
                    try
                    {
                        using (TcpClient client = new TcpClient(_ImpresoraDevolucion, 9100))//colocarIPImpresora
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                byte[] bytes = Encoding.ASCII.GetBytes(encabezado + cuerpo + pie);
                                stream.Write(bytes, 0, bytes.Length);
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        
                    }

                }
                else
                {
                    cont++;
                }

            }

            if(fila > 50 && cont > 1 )
            {
                try
                {
                    using (TcpClient client = new TcpClient(_ImpresoraDevolucion, 9100))//colocarIPImpresora
                    {
                        using (NetworkStream stream = client.GetStream())
                        {
                            byte[] bytes = Encoding.ASCII.GetBytes(encabezado + cuerpo + pie);
                            stream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                catch (Exception err)
                {

                }
            }

            

            





            return "OK";
        }
        
        public async Task<List<IM_WMS_Devolucion_Busqueda>> getDevolucionesConsolidar()
        {
            List<IM_WMS_Devolucion_Busqueda> lista = new List<IM_WMS_Devolucion_Busqueda>();

            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> { };

            List<IM_WMS_Devolucion_Busqueda> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_Devolucion_Busqueda>("[IM_WMS_ObtenerDevolucionesConsolidar]", parametros);
            return resp;
        }
        public async Task<List<IM_WMS_CrearCajaDevolucion>> getPackingRecibirCajaConsolidada(int id, string usuario,string tipo)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> {
                    new SqlParameter("@id",id),
                    new SqlParameter("@usuario",usuario),
                    new SqlParameter("@tipo",tipo),
            };

            List<IM_WMS_CrearCajaDevolucion> resp = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_CrearCajaDevolucion>("[IM_WMS_PackingCajaConsolidado]", parametros);
            return resp;
        }





        public async Task<string> imprimirTodasEtiquetasPendientes(string journalID)
        {
            
            var resp = await Get_InventarioCilicoTelaDiarios(journalID);
            resp.ForEach(ele =>
            {
                if(ele.Exist == false)
                {
                        List<EtiquetaRolloDTO> tmp = new List<EtiquetaRolloDTO>();
                        EtiquetaRolloDTO tmpR = new EtiquetaRolloDTO();
                        tmpR.INVENTSERIALID = ele.InventSerialID;
                        tmpR.APVENDROLL = ele.ApvendRoll;
                        tmpR.QTYTRANSFER = ele.InventOnHand.ToString();
                        tmpR.ITEMID = ele.ItemID;
                        tmpR.COLOR = ele.ColorName;
                        tmpR.INVENTBATCHID = ele.InventBatchID;
                        tmpR.CONFIGID = ele.ConfigID;
                        tmpR.PRINT = "10.1.1.164";

                        tmp.Add(tmpR);

                        var resp = postImprimirEtiquetaRollo(tmp);
                }
                


            });

            return "OK";
        }

        
    }
   
}