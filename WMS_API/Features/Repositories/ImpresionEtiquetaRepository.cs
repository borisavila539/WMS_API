using Core.DTOs.ImpresionEtiquetas;
using Core.DTOs.RecepcionYUbicacionAX;
using Core.Interfaces;
using DevExpress.Xpo.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace WMS_API.Features.Repositories
{
    public class ImpresionEtiquetaRepository : IimpresionEtiquetaRepository
    {
        private readonly string _connectionString;



        public ImpresionEtiquetaRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzas");
           
        }

        public async Task<RespuestaGetRutaPickingDTO> GetRutaDeEmpaque(string numeroCaja)
        {
            RespuestaGetRutaPickingDTO result = null;
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@NumeroCaja", numeroCaja)
                };

                 result = await executeProcedure.ExecuteStoredProcedure<RespuestaGetRutaPickingDTO>(
                   "IM_WMS_GetRutaDeNumeroDeCaja",
                   parameters
               );
              
               return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public async Task<List<DatosEtiqueta>> GetItemsAsyncById2Async(
            string cmpCode,
            string saleOrderId,
            string routeId)
        {
            List<DatosEtiqueta> result = new List<DatosEtiqueta>();

            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                string ordenBusqueda = saleOrderId.Trim().ToLower() == "all" ? null : saleOrderId;


                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@cmpCode", cmpCode),
                    new SqlParameter("@saleOrderId", ordenBusqueda), 
                    new SqlParameter("@routeId", routeId)
                };

                result = await executeProcedure.ExecuteStoredProcedureList<DatosEtiqueta>(
                    "SP_ObtenerDatosEtiquetas_Validado",
                    parameters
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public string codigoASCII(string texto)
        {
            string result = "";

            foreach (var item in texto)
            {
                if (item == 'Á')
                {
                    result += "_b5";
                }
                else if (item == 'É')
                {
                    result += "_90";
                }
                else if (item == 'Í')
                {
                    result += "_a1";
                }
                else if (item == 'Ó')
                {
                    result += "_e0";
                }
                else if (item == 'ú')
                {
                    result += "_a3";
                }
                else if (item == 'á')
                {
                    result += "_a0";
                }
                else if (item == 'é')
                {
                    result += "_82";
                }
                else if (item == 'í')
                {
                    result += "_a1";
                }
                else if (item == 'ó')
                {
                    result += "_a2";
                }
                else if (item == 'ú')
                {
                    result += "_a3";
                }
                else if (item == 'Ñ')
                {
                    result += "_a5";
                }
                else if (item == 'ñ')
                {
                    result += "_a4";
                }
                else
                {
                    result += item;
                }
            }
            return result;
        }
        public async Task<List<DatosEtiqueta>> ExecuteLoadEtiquetasItemsCommand2Refactored(
        string receiveEmpresa,
        string listaEmpaque,
        string pedido,
        string ipPrinter)
        {
            try
            {
                int separador = 10;
                int sigLineaY = 80;
                int tamaTexto = 43;
                int tamaTextoD = 30;
                string contenidoEnviar = "";
                string empresa = "";
                string cajaAnterior = "";
                int contadorEnviados = 0;
                string[] ArregloContenidoEnviado;
                int indice = 0;
                int cantidadFilas = 0;
                int contaDigitos = 0;
                string ItemAnterior = "";
                string ColorAnterior = "";
                int limiteVertical = 1190;
                int cantidadPorReferencia = 0;
                string marcaAnterior = "";
                int marcaImpresa = 0;

                var items2 = await GetItemsAsyncById2Async(receiveEmpresa, pedido, listaEmpaque);

                foreach (var item in items2)
                {
                    if (item.NumeroCajaLocal.ToString() != cajaAnterior)
                    {
                        cantidadFilas++;
                        cajaAnterior = item.NumeroCajaLocal.ToString();
                    }
                }
                ArregloContenidoEnviado = new string[cantidadFilas];
                cajaAnterior = "";

                foreach (var item in items2)
                {
                    if (item.Marca != marcaAnterior)
                    {
                        marcaAnterior = item.Marca;
                        marcaImpresa = 1;
                    }

                    if (item.NumeroCajaLocal.ToString() == cajaAnterior)
                    {
                        if (item.ItemId == ItemAnterior && item.Color == ColorAnterior)
                        {
                            if (contaDigitos + (item.Size.ToString().Length + item.Cantidad.ToString().Length + 3) >= 55) //desbordan horizontalmente entonces hago un salto de linea
                            {
                                if ((sigLineaY + tamaTextoD + separador) > limiteVertical)  //se acabó el espacio vertical en una etiqueta
                                {
                                    contenidoEnviar += " ^FS^XZ ";
                                    sigLineaY = 90; //vuelve a comenzar el posicionamiento de linea en la etiqueta nueva

                                    contenidoEnviar += "^XA^LH0,0";
                                    contenidoEnviar += "^FO30,30^A0N,50,50^FD             ";
                                    if (item.Empresa.ToString() == "IMHN")
                                    {
                                        contenidoEnviar += "   INTERMODA ";
                                    }
                                    if (item.Empresa.ToString() == "IMGT")
                                    {
                                        contenidoEnviar += "   MODINTER SA ";
                                    }
                                    if (item.Empresa.ToString() == "IMCR")
                                    {
                                        contenidoEnviar += "   MODINTER MI ";
                                    }
                                    contenidoEnviar += "^FS";

                                    contenidoEnviar += "^FO30," + sigLineaY + "^A0N,50,50^FD DESTINO:  ^FS";
                                    sigLineaY += tamaTexto + 7 + separador;
                                    contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + tamaTexto + "," + tamaTexto + "^FH^FD" + codigoASCII(item.ClienteNombre) + " ^FS";
                                    sigLineaY += tamaTexto + separador;
                                    contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + (tamaTexto + 18) + "," + (tamaTexto + 12) + "^FD " + item.Pedido + ", " + item.ListaEmpaque + ",    " + item.NumeroCajaLocal + " ^FS";  //+ "/" + item.TotalCajas + " ^FS";
                                    sigLineaY += separador + 10 + 18;
                                    contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + tamaTexto + "," + tamaTexto + "^FD" + "--------------------------" + "^FS";
                                    sigLineaY += tamaTextoD + separador;

                                    if (marcaImpresa == 1)
                                    {
                                        contenidoEnviar += "^FO70," + sigLineaY + "^A0N," + (tamaTexto + 10) + "," + (tamaTexto + 10) + "^FD " + marcaAnterior + " ^FS";
                                        sigLineaY += tamaTextoD + separador + 10;
                                        marcaImpresa = 0;
                                    }

                                    contenidoEnviar += "^FO20," + sigLineaY + "^A0N," + tamaTextoD + "," + tamaTextoD + "^FD " + item.ItemId + "/" + codigoASCII(item.ColorDescription) + " " + item.Size + "(" + item.Cantidad + ") ";

                                    cantidadPorReferencia += Convert.ToInt32(item.Cantidad.ToString());

                                    contaDigitos = item.ItemId.ToString().Length + 1 + item.ColorDescription.ToString().Length + item.Size.ToString().Length + item.Cantidad.ToString().Length + 4; // 3 por los parentesis mas el espacio
                                    contadorEnviados = 1;
                                    //creamos un encabezado de la siguiente etiqueta de la misma caja/////////
                                }
                                else
                                {
                                    contenidoEnviar += " ^FS ";
                                    sigLineaY += tamaTextoD + separador;

                                    contenidoEnviar += "^FO40," + sigLineaY + "^A0N," + tamaTextoD + "," + tamaTextoD + "^FD " + item.Size + "(" + item.Cantidad + ") ";

                                    cantidadPorReferencia += Convert.ToInt32(item.Cantidad.ToString());

                                    contaDigitos = item.Size.ToString().Length + item.Cantidad.ToString().Length + 3;
                                }
                            }
                            else
                            {
                                contenidoEnviar += item.Size + "(" + item.Cantidad + ") ";

                                cantidadPorReferencia += Convert.ToInt32(item.Cantidad.ToString());

                                contaDigitos += item.Size.ToString().Length + item.Cantidad.ToString().Length + 3; // 3 por los parentesis mas el espacio
                            }
                        }
                        else
                        {
                            if ((contaDigitos + cantidadPorReferencia.ToString().Length + 2) <= 55)
                            {
                                contenidoEnviar += "= " + cantidadPorReferencia.ToString() + " ^FS ";
                                cantidadPorReferencia = 0;
                            }
                            else
                                if ((sigLineaY + tamaTextoD + separador) < limiteVertical)
                                {
                                    contenidoEnviar += " ^FS ";
                                    sigLineaY += tamaTextoD + separador;
                                    contenidoEnviar += "^FO20," + sigLineaY + "^A0N," + tamaTextoD + "," + tamaTextoD + "^FD = " + cantidadPorReferencia.ToString() + "^FS ";
                                    cantidadPorReferencia = 0;
                                }

                            if ((sigLineaY + tamaTextoD + separador) > limiteVertical)  //se acabó el espacio vertical en una etiqueta
                            {
                                contenidoEnviar += "^FS^XZ ";
                                sigLineaY = 90; //vuelve a comenzar el posicionamiento de linea en la etiqueta nueva

                                contenidoEnviar += "^XA^LH0,0";
                                contenidoEnviar += "^FO30,30^A0N,50,50^FD             ";
                                if (item.Empresa.ToString() == "IMHN")
                                {
                                    contenidoEnviar += "   INTERMODA ";
                                }
                                if (item.Empresa.ToString() == "IMGT")
                                {
                                    contenidoEnviar += "   MODINTER SA ";
                                }
                                if (item.Empresa.ToString() == "IMCR")
                                {
                                    contenidoEnviar += "   MODINTER MI ";
                                }
                                contenidoEnviar += "^FS";

                                contenidoEnviar += "^FO30," + sigLineaY + "^A0N,50,50^FD DESTINO:  ^FS";
                                sigLineaY += tamaTexto + 7 + separador;

                                contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + (tamaTexto + 10) + "," + (tamaTexto + 10) + "^FH^FD" + codigoASCII(item.ClienteNombre) + " ^FS";
                                sigLineaY += tamaTexto + 15 + separador;
                                contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + (tamaTexto) + "," + (tamaTexto) + "^FD " + item.Pedido + ", " + item.ListaEmpaque + ",    " + item.NumeroCajaLocal + " ^FS";  //+ "/" + item.TotalCajas + " ^FS";
                                sigLineaY += separador + 10;
                                contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + tamaTexto + "," + tamaTexto + "^FD" + "--------------------------" + "^FS";

                                if (cantidadPorReferencia > 0)
                                {
                                    sigLineaY += tamaTextoD + separador;                                                                                                                                                                                                            /////////////////////////////////////////////
                                    contenidoEnviar += "^FO20," + sigLineaY + "^A0N," + tamaTextoD + "," + tamaTextoD + "^FD " + " = " + cantidadPorReferencia.ToString() + "^FS";        /////////////////////////////////////////////
                                    cantidadPorReferencia = 0;
                                }

                                if (marcaImpresa == 1)
                                {
                                    contenidoEnviar += "^FO70," + sigLineaY + "^A0N," + (tamaTexto + 10) + "," + (tamaTexto + 10) + "^FD " + marcaAnterior + " ^FS";
                                    sigLineaY += tamaTextoD + separador + 10;
                                    marcaImpresa = 0;
                                }

                                sigLineaY += tamaTextoD + separador;
                                contenidoEnviar += "^FO20," + sigLineaY + "^A0N," + tamaTextoD + "," + tamaTextoD + "^FD " + item.ItemId + "/" + codigoASCII(item.ColorDescription) + " " + item.Size + "(" + item.Cantidad + ") ";

                                cantidadPorReferencia = Convert.ToInt32(item.Cantidad.ToString());                  //////////////////////////////////

                                ItemAnterior = item.ItemId;
                                ColorAnterior = item.Color;
                                contaDigitos = item.ItemId.ToString().Length + 1 + item.ColorDescription.ToString().Length + item.Size.ToString().Length + item.Cantidad.ToString().Length + 4; // 3 por los parentesis mas el espacio
                                contadorEnviados = 1;
                                //creamos un encabezado de la siguiente etiqueta de la misma caja/////////
                            }
                            else
                            {
                                ItemAnterior = item.ItemId;
                                ColorAnterior = item.Color;
                                sigLineaY += tamaTextoD + separador;
                                contenidoEnviar += "^FO20," + sigLineaY + "^A0N," + tamaTextoD + "," + tamaTextoD + "^FD " + item.ItemId + "/" + item.ColorDescription + " " + item.Size + "(" + item.Cantidad + ") ";

                                cantidadPorReferencia += Convert.ToInt32(item.Cantidad.ToString());          ///////////////////////////7

                                contaDigitos = item.ItemId.ToString().Length + 1 + item.ColorDescription.ToString().Length + item.Size.ToString().Length + item.Cantidad.ToString().Length + 4; // 3 por los parentesis mas el espacio
                            }
                        }
                    }
                    else
                    {
                        marcaImpresa = 1;
                        cajaAnterior = item.NumeroCajaLocal.ToString();
                        if (contadorEnviados == 1)  //se hace el cierre de la caja anterior
                        {
                            contadorEnviados = 0;
                            contaDigitos = 0;

                            if ((contaDigitos + cantidadPorReferencia.ToString().Length + 2) <= 55)    /////////////////////////////////
                            {
                                contenidoEnviar += "= " + cantidadPorReferencia.ToString() + " ^FS ";
                                cantidadPorReferencia = 0;
                            }
                            else
                                if ((sigLineaY + tamaTextoD + separador) < limiteVertical)
                                {
                                    contenidoEnviar += " ^FS ";
                                    sigLineaY += tamaTextoD + separador;
                                    contenidoEnviar += "^FO20," + sigLineaY + "^A0N," + tamaTextoD + "," + tamaTextoD + "^FD = " + cantidadPorReferencia.ToString() + "^FS ";
                                    cantidadPorReferencia = 0;
                                }                                                                                                                                                                                ///////////////////////////////// seria la suma de la ultima referecia antes de pasar a la siguiente caja

                            contenidoEnviar += "^FS^XZ ";
                            sigLineaY = 90;
                        }
                        if (contadorEnviados == 0)
                        {
                            contenidoEnviar += "^XA^LH0,0";
                            contenidoEnviar += "^FO30,30^A0N,50,50^FD             ";
                            if (item.Empresa.ToString() == "IMHN")
                            {
                                contenidoEnviar += "   INTERMODA ";
                            }
                            if (item.Empresa.ToString() == "IMGT")
                            {
                                contenidoEnviar += "   MODINTER SA ";
                            }
                            if (item.Empresa.ToString() == "IMCR")
                            {
                                contenidoEnviar += "   MODINTER MI ";
                            }
                            contenidoEnviar += "^FS";

                            contenidoEnviar += "^FO30," + sigLineaY + "^A0N,50,50^FD DESTINO:  ^FS";
                            sigLineaY += tamaTexto + 7 + separador;

                            // Nombre del cliente
                            contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + (tamaTexto + 10) + "," + (tamaTexto + 10) + "^FH^FD" + codigoASCII(item.ClienteNombre) + " ^FS";
                            sigLineaY += tamaTexto + 15 + separador;

                            // --- MODIFICACIÓN AQUÍ: Teléfono del cliente abajo del Nombre ---
                            string telefono = item.TelefonoCliente?.ToString() ?? "";
                            contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + tamaTexto + "," + (tamaTexto - 8) + "^FH^FD TEL: " + codigoASCII(telefono) + " ^FS";
                            sigLineaY += tamaTexto + 15 + separador;
                            // -----------------------------------------------------------------

                            switch (item.Empresa)
                            {
                                case "IMCR":
                                    empresa = "Costa Rica";
                                    break;
                                case "IMGT":
                                    empresa = "Guatemala";
                                    break;
                                case "IMHN":
                                    empresa = "Honduras";
                                    break;
                                default:
                                    empresa = "Honduras";
                                    break;
                            }
                            contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + (tamaTexto + 10) + "," + (tamaTexto + 10) + "^FH^FD" + empresa + ". " + codigoASCII(item.Ciudad) + ", " + codigoASCII(item.Departamento) + " ^FS";
                            sigLineaY += tamaTexto + 15 + separador;
                            if (item.Direccion.ToString().Length >= 55)
                            {
                                contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + tamaTexto + "," + (tamaTexto - 8) + "^FH^FD " + codigoASCII(item.Direccion.ToString().Substring(0, 54)) + " ^FS";
                                sigLineaY += tamaTexto + separador;
                                if (item.Direccion.ToString().Length > 55)
                                {
                                    contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + tamaTexto + "," + (tamaTexto - 8) + "^FH^FD " + codigoASCII(item.Direccion.ToString().Substring(55, item.Direccion.ToString().Length - 55)) + " ^FS";
                                    sigLineaY += tamaTexto + separador;
                                }
                            }
                            else
                            {
                                contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + tamaTexto + "," + (tamaTexto - 8) + "^FH^FD " + codigoASCII(item.Direccion.ToString()) + " ^FS";
                                sigLineaY += tamaTexto + separador;
                            }

                            contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + (tamaTexto + 25) + "," + (tamaTexto + 12) + "^FH^FD " + item.Pedido + ", " + item.ListaEmpaque + " ^FS";
                            sigLineaY += tamaTexto + separador + 25;

                            contenidoEnviar += "^FO20," + sigLineaY + "^A0N," + (tamaTexto + 25) + "," + (tamaTexto + 12) + "^FH^FD Caja: " + item.NumeroCajaLocal + "^FS";

                            contenidoEnviar += "^FO260," + sigLineaY + "^A0N," + (tamaTexto + 5) + "," + (tamaTexto + 5) + " ^BCN, 62, N, N ^FD>:" + item.NumeroCaja + "^FS";

                            /**/
                            int TotalUnidades = 0;
                            foreach (var itemb in items2)
                            {
                                if (itemb.NumeroCajaLocal.ToString() == cajaAnterior)
                                {
                                    TotalUnidades += Convert.ToInt32(itemb.Cantidad.ToString());
                                }
                            }
                            contenidoEnviar += "^FO400," + sigLineaY + "^A0N," + (tamaTexto + 25) + "," + (tamaTexto + 12) + "^FH^FD              T: " + TotalUnidades.ToString() + " ^FS";

                            sigLineaY += tamaTexto + separador + 20;
                            contenidoEnviar += "^FO70," + sigLineaY + "^A0N," + tamaTexto + "," + (tamaTexto - 8) + "^FD " + item.NumeroCaja + "      ASE: " + codigoASCII(item.Asesor) + " ^FS"; // "/" + item.TotalCajas + " ^FS";
                            sigLineaY += separador + 10;
                            contenidoEnviar += "^FO30," + sigLineaY + "^A0N," + tamaTexto + "," + tamaTexto + "^FD" + "----------------------------" + "^FS";
                            sigLineaY += tamaTextoD + separador;

                            if (marcaImpresa == 1)
                            {
                                contenidoEnviar += "^FO70," + sigLineaY + "^A0N," + (tamaTexto + 10) + "," + (tamaTexto + 10) + "^FD " + marcaAnterior + " ^FS";
                                sigLineaY += tamaTextoD + separador + 10;
                                marcaImpresa = 0;
                            }

                            contenidoEnviar += "^FO20," + sigLineaY + "^A0N," + tamaTextoD + "," + tamaTextoD + "^FD " + item.ItemId + "/" + codigoASCII(item.ColorDescription) + " " + item.Size + "(" + item.Cantidad + ") ";

                            cantidadPorReferencia = Convert.ToInt32(item.Cantidad.ToString()); ////////////////////////////////////////////////////////

                            ItemAnterior = item.ItemId;
                            ColorAnterior = item.Color;
                            contaDigitos = item.ItemId.ToString().Length + 1 + item.ColorDescription.ToString().Length + item.Size.ToString().Length + item.Cantidad.ToString().Length + 4; // 3 por los parentesis mas el espacio
                            contadorEnviados = 1;
                        }
                    }
                } //fin foreach

                if (contenidoEnviar.Length > 0)
                {
                    if ((contaDigitos + cantidadPorReferencia.ToString().Length + 2) <= 55)    /////////////////////////////////
                    {
                        contenidoEnviar += "= " + cantidadPorReferencia.ToString() + " ^FS ";
                        cantidadPorReferencia = 0;
                    }
                    else if ((sigLineaY + tamaTextoD + separador) < limiteVertical)
                    {
                        contenidoEnviar += " ^FS ";
                        sigLineaY += tamaTextoD + separador;
                        contenidoEnviar += "^FO20," + sigLineaY + "^A0N," + tamaTextoD + "," + tamaTextoD + "^FD = " + cantidadPorReferencia.ToString() + "^FS ";
                        cantidadPorReferencia = 0;
                    }                                                                                                                                                                                               /////////////////////////////////////////////

                    contenidoEnviar += "^FS^XZ ";
                }

                if (contenidoEnviar.Length > 0)
                {
                    await EnviarAImpresor(contenidoEnviar, ipPrinter);

                }
                return items2;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new List<DatosEtiqueta>();
            }

        }
       



        private async Task EnviarAImpresor(string etiqueta, string ipPrinter)
        {
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(ipPrinter, 9100);

                using (NetworkStream stream = client.GetStream())
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);

                    await stream.WriteAsync(bytes, 0, bytes.Length);
                    await stream.FlushAsync();
                }
            }
        }
    }
}
