using Core.DTOs.Despacho_PT.GenerarDespachoPorImportacionExcel;
using Core.DTOs.GeneracionPrecios;
using Core.Interfaces;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class IM_WMS_GeneracionPrecioYCodigosRepository : IIM_WMS_GeneracionPrecioYCodigosRepository
    {
        private readonly string _connectionString;
        public IM_WMS_GeneracionPrecioYCodigosRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("IMFinanzasDev");
        }

        public async Task<List<IM_WMS_ClientesGeneracionprecios>> GetClientesGeneracionprecios()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter> { };


            List<IM_WMS_ClientesGeneracionprecios> response = await executeProcedure.ExecuteStoredProcedureList<IM_WMS_ClientesGeneracionprecios>("[IM_WMS_ObtenerClientesGeneracionPrecio]", parametros);

            return response;
        }

        public ConfiguracionPrecioImportResultDto ParsearExcel(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var result = new ConfiguracionPrecioImportResultDto();

            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("El archivo Excel está vacío o no es válido.");

                result.Success = true;
                result.Message = "Archivo parseado exitosamente.";

                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var dataSet = reader.AsDataSet();

                        if (dataSet.Tables.Count == 0)
                            throw new Exception("El archivo Excel no contiene hojas de trabajo.");

                        DataTable table = dataSet.Tables[0];
                        string Categoria = "";
                        // Saltar encabezado
                        for (int row = 2; row < table.Rows.Count; row++)
                        {
                            DataRow dr = table.Rows[row];

                            if (dr.ItemArray.All(x =>
                                string.IsNullOrWhiteSpace(x?.ToString())))
                            {
                                continue;
                            }

                            decimal.TryParse(dr[7]?.ToString(), out decimal costo);
                            decimal.TryParse(dr[8]?.ToString(), out decimal precio);
                            decimal.TryParse(dr[9]?.ToString(), out decimal margen);
          
                            Categoria = string.IsNullOrEmpty(dr[0]?.ToString()?.Trim()) ? Categoria : dr[0]?.ToString()?.Trim();

                            result.Data.Add(new ConfiguracionPrecioDto
                            {
                                Categoria = Categoria,
                                Cuenta = dr[1]?.ToString()?.Trim() ?? "",
                                Coleccion = dr[2]?.ToString()?.Trim() ?? "",
                                Subcategoria = dr[3]?.ToString()?.Trim() ?? "",
                                Base = dr[4]?.ToString()?.Trim() ?? "",
                                IdColor = dr[5]?.ToString()?.Trim() ?? "",
                                Talla = dr[6]?.ToString()?.Trim() ?? "",
                                Costo = costo,
                                Precio = precio,
                                Margen = margen
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al procesar el archivo: {ex.Message}";
                result.Data.Clear();
            }

            return result;
        }
        public async Task<List<ConfiguracionPrecioDto>> GetConfiguracionPrecio()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            try
            {
                return await executeProcedure
                    .ExecuteStoredProcedureList<ConfiguracionPrecioDto>(
                        "[IM_WMS_GetConfiguracionPrecio]",
                        new List<SqlParameter>());
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<int> InsertConfiguracionPrecio(ConfiguracionPrecioDto item,int userCreated)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Categoria", item.Categoria ?? ""),
                new SqlParameter("@Cuenta", item.Cuenta ?? ""),
                new SqlParameter("@Coleccion", item.Coleccion ?? ""),
                new SqlParameter("@Subcategoria", item.Subcategoria ?? ""),
                new SqlParameter("@Base", item.Base ?? ""),
                new SqlParameter("@IdColor", item.IdColor ?? ""),
                new SqlParameter("@Talla", item.Talla ?? ""),
                new SqlParameter("@Costo", item.Costo),
                new SqlParameter("@Precio", item.Precio),
                new SqlParameter("@Margen", item.Margen),
                new SqlParameter("@UserCreated", userCreated)
            };

            try
            {
                SpResponseDTO result = await executeProcedure
                    .ExecuteStoredProcedure<SpResponseDTO>(
                        "[IM_WMS_Insert_ConfiguracionPrecio]",
                        parametros);

                return result.ID;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> UpdateConfiguracionPrecio(ConfiguracionPrecioDto item,int userModified)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Id", item.Id),
                new SqlParameter("@Categoria", item.Categoria ?? ""),
                new SqlParameter("@Cuenta", item.Cuenta ?? ""),
                new SqlParameter("@Coleccion", item.Coleccion ?? ""),
                new SqlParameter("@Subcategoria", item.Subcategoria ?? ""),
                new SqlParameter("@Base", item.Base ?? ""),
                new SqlParameter("@IdColor", item.IdColor ?? ""),
                new SqlParameter("@Talla", item.Talla ?? ""),
                new SqlParameter("@Costo", item.Costo),
                new SqlParameter("@Precio", item.Precio),
                new SqlParameter("@Margen", item.Margen),
                new SqlParameter("@UserModified", userModified)
            };

            try
            {
                SpResponseDTO result = await executeProcedure
                    .ExecuteStoredProcedure<SpResponseDTO>(
                        "[IM_WMS_Update_ConfiguracionPrecio]",
                        parametros);

                return result.ID;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<SpResponseDTO> DeleteConfiguracionPrecio(int id)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Id", id)
            };

            try
            {
                SpResponseDTO result = await executeProcedure
                    .ExecuteStoredProcedure<SpResponseDTO>(
                        "[IM_WMS_Delete_ConfiguracionPrecio]",
                        parametros);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IM_WMS_ObtenerDetalleGeneracionPrecios>> GetObtenerDetalleGeneracionPrecios(string pedido, string empresa)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var pedidos = pedido.Split(',');
            List<IM_WMS_ObtenerDetalleGeneracionPrecios> response = new List<IM_WMS_ObtenerDetalleGeneracionPrecios>();
            for (int i = 0; i < pedidos.Length; i++)
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
        public async Task<List<IM_WMS_HistoricoGeneracionPrecioCodigoDto>>GenerarHistoricoPrecios(string pedido,string empresa)
        {
            var detallePedidos =
                await GetObtenerDetalleGeneracionPrecios(
                    pedido,
                    empresa);

            if (!detallePedidos.Any())
                return new List<IM_WMS_HistoricoGeneracionPrecioCodigoDto>();

            await LimpiarHistoricoPedidos(pedido);

            foreach (var linea in detallePedidos)
            {
                await ProcesarLineaHistorico(linea);
            }

            return await ObtenerHistoricoPedidos(pedido);
        }
        private async Task ProcesarLineaHistorico(IM_WMS_ObtenerDetalleGeneracionPrecios linea)
        {
            //if (linea.Base == "PR947")
            //{
            //    Console.WriteLine("hola mundo");
            //}
            var configuracion =
                await ObtenerConfiguracionLinea(linea);

            bool tieneIrregularidad =
                   !configuracion.ExisteConfiguracion
                || Math.Round(linea.Costo, 4)
                   != Math.Round(configuracion.Costo, 4);

            await InsertarHistoricoLinea(
                linea,
                configuracion,
                tieneIrregularidad);
        }
        private async Task<ConfiguracionPrecioLineaDto>ObtenerConfiguracionLinea(IM_WMS_ObtenerDetalleGeneracionPrecios linea)
        {
            ExecuteProcedure executeProcedure =
                new ExecuteProcedure(_connectionString);
            try
            {

            return await executeProcedure
                .ExecuteStoredProcedure<ConfiguracionPrecioLineaDto>(
                    "WMS_GetConfiguracionPrecioLineaPedido",
                    new List<SqlParameter>
                    {
                    new("@Cuenta", linea.CuentaCliente),
                    new("@Categoria", linea.Categoria),
                    new("@Base", linea.Base),
                    new("@Color", linea.IDColor),
                    new("@Genero", linea.Departamento),
                    new("@Talla", linea.Talla),
                    new("@Coleccion", linea.Coleccion),
                    new("@SubCategoria", linea.SubCategoria)
                    });
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task InsertarHistoricoLinea(
        IM_WMS_ObtenerDetalleGeneracionPrecios linea,
        ConfiguracionPrecioLineaDto configuracion,
        bool tieneIrregularidad)
        {
            ExecuteProcedure executeProcedure =
                new ExecuteProcedure(_connectionString);

            await executeProcedure.ExecuteStoredProcedureDynamic(
                "WMS_InsertGeneracionPreciosDetalle",
                ConstruirParametrosInsert(
                    linea,
                    configuracion,
                    tieneIrregularidad));
        }
        private List<SqlParameter> ConstruirParametrosInsert(
        IM_WMS_ObtenerDetalleGeneracionPrecios linea,
        ConfiguracionPrecioLineaDto configuracion,
        bool tieneIrregularidad)
        {
            return new List<SqlParameter>
            {
                new("@TieneIrregularidad", tieneIrregularidad),

                new("@CuentaCliente", linea.CuentaCliente),
                new("@PedidoVenta", linea.Pedido),
                new("@CodigoBarra", linea.CodigoBarra),
                new("@Articulo", linea.Articulo),

                new("@Base",
                    (object?)linea.Base ?? DBNull.Value),

                new("@Estilo",
                    (object?)linea.Estilo ?? DBNull.Value),

                new("@IDColor",
                    (object?)linea.IDColor ?? DBNull.Value),

                new("@Referencia",
                    (object?)linea.Referencia ?? DBNull.Value),

                new("@Descripcion",
                    (object?)linea.Descripcion ?? DBNull.Value),

                new("@ColorDescripcion",
                    (object?)linea.ColorDescripcion ?? DBNull.Value),

                new("@Talla",
                    (object?)linea.Talla ?? DBNull.Value),

                new("@Descripcion2",
                    (object?)linea.Descripcion2 ?? DBNull.Value),

                new("@Categoria",
                    (object?)linea.Categoria ?? DBNull.Value),

                new("@Cantidad",
                    linea.Cantidad),

                new("@CostoAX",
                    linea.Costo),

                new("@CostoConfiguracionPrecio",
                    configuracion.Costo),

                new("@Precio",
                    configuracion.Precio),

                new("@Departamento",
                    (object?)linea.Departamento ?? DBNull.Value),

                new("@SubCategoria",
                    (object?)linea.SubCategoria ?? DBNull.Value),

                new ("@DeliveryName",
                    (object?)linea.DeliveryName ?? DBNull.Value),

                new ("@Coleccion",
                    (object?)linea.Coleccion ?? DBNull.Value),

            };
        }
        private async Task LimpiarHistoricoPedidos(string pedido)
        {
            ExecuteProcedure executeProcedure =
                new ExecuteProcedure(_connectionString);

            foreach (var pedidoActual in pedido.Split(','))
            {
                await executeProcedure.ExecuteStoredProcedureDynamic(
                    "WMS_DeleteGeneracionPreciosDetallePorPedido",
                    new List<SqlParameter>
                    {
                new("@PedidoVenta",
                    pedidoActual.Trim())
                    });
            }
        }
        public async Task<List<IM_WMS_HistoricoGeneracionPrecioCodigoDto>> ObtenerHistoricoPedidos(string pedido)
        {
            ExecuteProcedure executeProcedure =
                new ExecuteProcedure(_connectionString);

            var resultado =
                new List<IM_WMS_HistoricoGeneracionPrecioCodigoDto>();

            foreach (var pedidoActual in pedido.Split(','))
            {
                var tmp =
                    await executeProcedure
                    .ExecuteStoredProcedureList<IM_WMS_HistoricoGeneracionPrecioCodigoDto>(
                        "WMS_GetGeneracionPreciosDetallePorPedido",
                        new List<SqlParameter>
                        {
                    new("@PedidoVenta",
                        pedidoActual.Trim())
                        });

                resultado.AddRange(tmp);
            }

            return resultado;
        }
        public async Task<ConfirmacionGeneracionPrecioDto> ConfirmarGeneracionPrecioLinea(int id,string usuario,string hostName)
        {
            try
            {
                ExecuteProcedure executeProcedure =
                    new ExecuteProcedure(_connectionString);

                await executeProcedure.ExecuteStoredProcedureDynamic(
                    "WMS_ConfirmarGeneracionPrecioLinea",
                    new List<SqlParameter>
                    {
                new SqlParameter("@Id", id),
                new SqlParameter("@UsuarioConfirmacion", usuario),
                new SqlParameter("@HostNameConfirmacion", hostName)
                    });

                return new ConfirmacionGeneracionPrecioDto
                {
                    Success = true,
                    Message = "Línea confirmada correctamente."
                };
            }
            catch (Exception ex)
            {
                return new ConfirmacionGeneracionPrecioDto
                {
                    Success = false,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        public async Task<List<IM_WMS_DetalleImpresionEtiquetasPrecio>> GetDetalleImpresionEtiquetasPrecio(ImpresionEtiqueta parms)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            parms.Normalizar(parms);
            List<IM_WMS_DetalleImpresionEtiquetasPrecio> response;

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
        private string imprimirEtiquetaCajaXS(string caja, string impresora, int filaStartInt)
        {
            string etiqueta = @"^XA^FWN^PW1200^PR2";
            int fila = filaStartInt;
            fila -= 15;
            etiqueta += @"^FO" + fila + ",25";
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
            }
            ;

            return "OK";
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
            }
            ;

            return "OK";
        }
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

                etiqueta += @"^FO" + fila + "," + (element.Decimal || !string.IsNullOrEmpty(element.Moneda) ? "140" : "140");

                if (!string.IsNullOrEmpty(element.Moneda) || element.Decimal)

                {

                    etiqueta += @"^A0R,32,32";

                }

                else

                {

                    etiqueta += @"^A0R,38,38";

                }

                etiqueta += @"^FD" + (!string.IsNullOrEmpty(element.Moneda) ? element.Moneda : "") + (element.Decimal ? element.Precio.ToString("F2") : element.Precio.ToString("F0")) + "^FS";

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

                    }
                    ;

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

                }
                ;

            }

            return "OK";

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

                etiqueta += @"^FO" + fila + "," + (element.Decimal || !string.IsNullOrEmpty(element.Moneda) ? "210" : "210");
                etiqueta += @"^A0R,55,55";
                etiqueta += @"^FD" + (!string.IsNullOrEmpty(element.Moneda) ? element.Moneda : "") + (element.Decimal ? element.Precio.ToString("F2") : element.Precio.ToString("F0")) + "^FS";

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
                    }
                    ;
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
                }
                ;
            }
            return "OK";
        }

        public async Task<List<TallaConfiguracionPrecioDto>> GetTallasConfiguracionPrecio()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            try
            {
                return await executeProcedure
                    .ExecuteStoredProcedureList<TallaConfiguracionPrecioDto>(
                        "[dbo].[IM_WMS_TallasConfiguracionPrecio_Listar]",
                        new List<SqlParameter>());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RespuestaSP> InsertarTallaConfiguracionPrecio(TallaConfiguracionPrecioDto item)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Genero", item.Genero ?? ""),
                new SqlParameter("@Talla", item.Talla ?? ""),
                new SqlParameter("@Tipo", item.Tipo ?? "")
            };

            try
            {
                return await executeProcedure
                    .ExecuteStoredProcedure<RespuestaSP>(
                        "[dbo].[IM_WMS_TallasConfiguracionPrecio_Insertar]",
                        parametros);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RespuestaSP> ActualizarTallaConfiguracionPrecio(TallaConfiguracionPrecioDto item)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Id", item.Id),
                new SqlParameter("@Genero", item.Genero ?? ""),
                new SqlParameter("@Talla", item.Talla ?? ""),
                new SqlParameter("@Tipo", item.Tipo ?? "")
            };

            try
            {
                return await executeProcedure
                    .ExecuteStoredProcedure<RespuestaSP>(
                        "[dbo].[IM_WMS_TallasConfiguracionPrecio_Actualizar]",
                        parametros);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RespuestaSP> EliminarTallaConfiguracionPrecio(int id)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Id", id)
            };

            try
            {
                return await executeProcedure
                    .ExecuteStoredProcedure<RespuestaSP>(
                        "[dbo].[IM_WMS_TallasConfiguracionPrecio_Eliminar]",
                        parametros);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CategoriaPorClienteBaseDto>> ObtenerCategoriaPorClienteBase(string cuentaCliente, string baseArticulo, string empresa)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@CuentaCliente", cuentaCliente ?? ""),
                new SqlParameter("@Base", baseArticulo ?? ""),
                new SqlParameter("@Empresa", string.IsNullOrWhiteSpace(empresa) ? "IMHN" : empresa)
            };

            try
            {
                return await executeProcedure
                    .ExecuteStoredProcedureList<CategoriaPorClienteBaseDto>(
                        "[dbo].[IM_WMS_ObtenerCategoriaPorClienteBase]",
                        parametros);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
