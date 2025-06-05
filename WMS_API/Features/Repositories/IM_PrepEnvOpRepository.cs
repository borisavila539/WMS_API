using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using WMS_API.Features.Utilities;
using System.Data.SqlClient;
using Core.DTOs.IM_PrepEnvOp;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WMS_API.Features.Repositories
{
    public class IM_PrepEnvOpRepository: IIM_PrepEnvOpRepository
    {
        private readonly string _connectionString;

        public IM_PrepEnvOpRepository(IConfiguration configuracion)
        {
            _connectionString = configuracion.GetConnectionString("IMFinanzas");
        }

        public async Task<ListadoDeOpResponseDTO> GetListadoDeOp(DateTime fechaInicioSemana, DateTime fechaFinSemana, string? area)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@FechaInicioSemana",fechaInicioSemana),
                new SqlParameter("@FechaFinSemana",fechaFinSemana),
                new SqlParameter("@areaFilter",area)
            };

            var result = await executeProcedure.ExecuteStoredProcedureList<IM_PrepEnvOp_ListadoDeOpDTO>("[IM_PrepEnvOp_ListadoDeOp]", parametros);
            var estilosUnicos = result
            .Select(r => r.Estilo)
            .Where(e => !string.IsNullOrEmpty(e))
            .Distinct()
            .ToList();

            var firstWeek = result[0].Semana;

            var agrupado = result
                .GroupBy(r => r.Semana) // Primero agrupas por Semana
                .SelectMany(gSemana => gSemana
                    .GroupBy(r => r.Estilo) // Luego agrupas por Estilo dentro de la semana
                    .Select(gEstilo => new EstiloAgrupadoDTO
                    {
                        Estilo = gEstilo.Key,
                        // Aqui necesito la semana
                        Semana = gSemana.Key,
                        Ordenes = gEstilo
                            .GroupBy(r => r.OrdenTrabajo) // Después por Orden de trabajo
                            .Select(gOrden => new OrdenDTO
                            {
                                OrdenTrabajo = gOrden.Key,
                                sumCantidadTransferida = gOrden.Sum(m => m.CantidadTransferida),
                                Materiales = gOrden
                                    .Select(m => new MaterialDTO
                                    {
                                        IdOpPreparada = m.IdOpPreparada,
                                        OrdenTrabajo = m.OrdenTrabajo,
                                        CodigoArticulo = m.CodigoArticulo,
                                        NombreArticulo = m.NombreArticulo,
                                        Color = m.Color,
                                        EstadoOp = m.EstadoOp,
                                        Articulo = m.Articulo,
                                        Lote = m.Lote,
                                        Estilo = m.Estilo,
                                        Base = m.Base,
                                        NoTraslado = m.NoTraslado,
                                        CantidadTransferida = m.CantidadTransferida,
                                        NoPropuesta = m.NoPropuesta,
                                        Area = m.Area,
                                        Transferid = m.Transferid,
                                        FechaTraslado = m.FechaTraslado,
                                        Semana = m.Semana,
                                        Year = m.Year,
                                        Empacado = m.Empacado,
                                        Datoempac = m.Datoempac,
                                        FechaEmpacado = m.FechaEmpacado,
                                        EmpacadoPor = m.EmpacadoPor,
                                        Enviado = m.Enviado,
                                        DatoEnv = m.DatoEnv,
                                        FechaEnvio = m.FechaEnvio,
                                        EnviadoPor = m.EnviadoPor,
                                        DesdeAlmacen = m.DesdeAlmacen,
                                        Almacen = m.Almacen,
                                        IsComplete = m.IsComplete,
                                        IsHiddenWeek = firstWeek == m.Semana,
                                        FechaActualizado = m.FechaActualizado,
                                        IsEmpaquetada = m.IsEmpaquetada,
                                        ActualizadoPor = m.ActualizadoPor,
                                        IdDetalleOpEnviada = m.IdDetalleOpEnviada
                                    }).ToList()
                            }).ToList()
                    })
                ).ToList();


            return new ListadoDeOpResponseDTO
            {
                ListaDeEstilos = estilosUnicos,
                Agrupados = agrupado
            };
        }
    
    
        public async Task<IM_PrepEnvOp_UpdateOpPreparadaDTO> UpdateOpPreparada(int idOpPreparada, string userCode)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@idOpPreparada",idOpPreparada),
                new SqlParameter("@userCode",userCode),
            };

            var result = await executeProcedure.ExecuteStoredProcedure<IM_PrepEnvOp_UpdateOpPreparadaDTO>("[IM_PrepEnvOp_UpdateOpPreparada]", parametros);
            return result;
        }


        public async Task<List<IM_PrepEnvOp_CorreosHabilitadosDTO>> CorreosHabilitados()
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>{};

            var result = await executeProcedure.ExecuteStoredProcedureList<IM_PrepEnvOp_CorreosHabilitadosDTO>("[IM_PrepEnvOp_CorreosHabilitados]", parametros);
            return result;
        }

        public async Task<List<IM_PrepEnvOp_ListaOpPorEnviarDTO>> ListaOpPorEnviar(DateTime fechaInicioSemana, DateTime fechaFinSemana)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@FechaInicioSemana",fechaInicioSemana),
                new SqlParameter("@FechaFinSemana",fechaFinSemana),
            };

            var result = await executeProcedure.ExecuteStoredProcedureList<IM_PrepEnvOp_ListaOpPorEnviarDTO>("[IM_PrepEnvOp_ListaOpPorEnviar]", parametros);
            return result;
        }

        public async Task<List<IM_PrepEnvOp_UpdateOpPreparadaLikeEmpaquetadaDTO>> UpdateOpPreparadaEmpaquetada(UpdateOpPreparadaEmpaquetadaRequestDTO data)
        {
            var response = new List<IM_PrepEnvOp_UpdateOpPreparadaLikeEmpaquetadaDTO>();

            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            for (int i = 0; i < data.materiales.Count; i++)
            {
                var item = data.materiales[i];

                for (int j = 0; j < item.ordenes.Count; j++)
                {
                    var row = item.ordenes[j];

                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("@idOpPreparada", row.idOpPreparada),
                        new SqlParameter("@userCode", data.userCode),
                    };

                    var result = await executeProcedure.ExecuteStoredProcedure<IM_PrepEnvOp_UpdateOpPreparadaLikeEmpaquetadaDTO>(
                        "[IM_PrepEnvOp_UpdateOpPreparadaLikeEmpaquetada]",
                        parametros
                    );

                    response.Add(result);
                }
            }

            return response;
        }



        public async Task<IM_PrepEnvOp_PostDetalleOpEnviadaDTO> PostDetalleOpEnviada(PostDetalleOpEnviadaResponseDTO response)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@nombreRecibidaPor",response.NombreRecibidaPor),
                new SqlParameter("@fechaDeEntrega",response.FechaDeEntrega),
                new SqlParameter("@firmaBase64",response.FirmaBase64),
                new SqlParameter("@creadoPor",response.CreadoPor),
            };

            var detalleOpEnviada = await executeProcedure.ExecuteStoredProcedure<IM_PrepEnvOp_PostDetalleOpEnviadaDTO>("[IM_PrepEnvOp_PostDetalleOpEnviada]", parametros);

            foreach (var item in response.ListaOpPorEnviar)
            {
                var parametrosByOpAndEstilo = new List<SqlParameter>
                {
                    new SqlParameter("@estilo",item.Estilo),
                    new SqlParameter("@ordenTrabajo",item.OrdenTrabajo),
                    new SqlParameter("@idDetalleOpEnviada",detalleOpEnviada.IdDetalleOpEnviada),
                    new SqlParameter("@area",item.Area),
                    new SqlParameter("@noTraslado",item.NoTraslado)
                };

                await executeProcedure.ExecuteStoredProcedureList<IM_PrepEnvOp_UpdateOpPreByEstiloAndOpDTO>("[IM_PrepEnvOp_UpdateOpPreByEstiloAndOp]", parametrosByOpAndEstilo);

            }

            string convertBase64 = Convert.ToBase64String(response.FirmaBase64);
            response.FirmaBase64 = Convert.FromBase64String(convertBase64);


            await EnvioDeCorreo(response, convertBase64);

            return detalleOpEnviada;
        }

        public async Task<string> EnvioDeCorreo(PostDetalleOpEnviadaResponseDTO response, string firmaBase64)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(VariablesGlobales.Correo);

                var correos = await CorreosHabilitados();
                var correosFiltrados = new List<string>();


                foreach (var op in response.ListaOpPorEnviar)
                {
                    foreach (var correo in correos)
                    {
                        if (op.MetalicosCount > 0 && correo.Area == "METALICOS")
                        {
                            correosFiltrados.Add(correo.Correo.Trim());
                        }

                        if (op.EmpaqueCount > 0 && correo.Area == "EMPAQUE")
                        {
                            correosFiltrados.Add(correo.Correo.Trim());
                        }
                    }
                }

                // Eliminar correos duplicados
                var correosUnicos = correosFiltrados.Distinct().ToList();

                foreach (var correo in correosUnicos)
                {
                    mail.To.Add(correo);
                }


                mail.Subject = "Preparacion y envio de ordenes metalicos y empaque" ;
                mail.IsBodyHtml = true;


                StringBuilder htmlTable = new StringBuilder();
                htmlTable.Append("<html>");
                htmlTable.Append("<body>");
                htmlTable.Append("<div style='font-family: Arial, sans-serif; font-size: 14px; color: #333;'>");
                htmlTable.Append("<h2>Control de Producción</h2>");
                htmlTable.Append("<p>Sistema de Preparacion y Envio de Órdenes</p>");
                htmlTable.Append("<p>Se ha registrado un nuevo envío de órdenes de producción con la siguiente información:</p>");
                
                htmlTable.Append("</br>");

                // Tabla detalle
                htmlTable.Append("<table style='width: 100%; border-collapse: collapse; margin-bottom: 16px;'>");
                htmlTable.Append("<tr style='background-color: #f2f2f2;'>");
                htmlTable.Append($"<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'><strong>Recibido por:</strong> {response.NombreRecibidaPor}</td>");
                htmlTable.Append($"<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'><strong>Fecha de entrega:</strong> {response.FechaDeEntrega:dd/MM/yyyy}</td>");
                htmlTable.Append("</tr>");
                htmlTable.Append("</table>");

                htmlTable.Append("</br>");
                htmlTable.Append("</br>");


                // Tabla de ordenes enviadas
                htmlTable.Append("<h3>Órdenes Enviadas</h3>");
                htmlTable.Append("<table style='width: 100%; border-collapse: collapse;'>");
                htmlTable.Append("<tr style='background-color: #f2f2f2;'>");
                htmlTable.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Estilo</th>");
                htmlTable.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>OP</th>");
                htmlTable.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Estado</th>");
                htmlTable.Append("</tr>");

                var combinacionesUnicas = new HashSet<string>();

                foreach (var op in response.ListaOpPorEnviar)
                {
                    string clave = $"{op.Estilo}-{op.OrdenTrabajo}";

                    if (!combinacionesUnicas.Contains(clave))
                    {
                        combinacionesUnicas.Add(clave);

                        htmlTable.Append("<tr>");
                        htmlTable.Append($"<td style='border: 1px solid #ddd; padding: 8px;'>{op.Estilo}</td>");
                        htmlTable.Append($"<td style='border: 1px solid #ddd; padding: 8px;'>{op.OrdenTrabajo}</td>");
                        htmlTable.Append($"<td style='border: 1px solid #ddd; padding: 8px;'>{op.Estado}</td>");
                        htmlTable.Append("</tr>");
                    }
                }

                htmlTable.Append("</table>");
                htmlTable.Append("</br>");

                htmlTable.Append($"<p><strong>Firma de recepción:</strong></p>");
                htmlTable.Append($"<img src='data:image/png;base64,{firmaBase64}' alt='Firma' style='max-width: 200px;' />");
                htmlTable.Append($"<p>{response.NombreRecibidaPor}</p>");
                htmlTable.Append("<p style='font-size: 12px; color: #666;'>Este correo fue generado automáticamente. Por favor no responda a este mensaje.</p>");
                htmlTable.Append("</div>");
                htmlTable.Append("</body>");
                htmlTable.Append("</html>");

                mail.Body = htmlTable.ToString();

                SmtpClient oSmtpClient = new SmtpClient("smtp.office365.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(VariablesGlobales.Correo, VariablesGlobales.Correo_Password)
                };

                oSmtpClient.Send(mail);
                oSmtpClient.Dispose();
                
                
                return "ok";
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<string> PostPrintEtiquetasMateriales(List<ArticuloDTO> data, string? ipImpresora)
        {
           ipImpresora = string.IsNullOrEmpty(ipImpresora) ? "10.1.1.164" : ipImpresora;

            var listaDeEtiquetas = TransformarEtiquetas(data);

            try
            {
                // Procesar etiquetas en bloques de 3
                for (int i = 0; i < listaDeEtiquetas.Count; i += 3)
                {
                    string etiqueta = "^XA";
                    etiqueta += "^MD5^PRB^FWN";
                    etiqueta += "^PW1015";

                    // Posiciones para 3 etiquetas por línea
                    int[] posicionesX = new int[] { 700, 470, 240 };

                    for (int j = 0; j < 3; j++)
                    {
                        if (i + j >= listaDeEtiquetas.Count)
                            break;

                        var item = listaDeEtiquetas[i + j];
                        etiqueta += GenerarEtiquetaZPL(item, posicionesX[j]);
                    }

                    etiqueta += "^XZ";

                    using (TcpClient client = new TcpClient(ipImpresora, 9100))
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(etiqueta);
                        stream.Write(bytes, 0, bytes.Length);
                        Thread.Sleep(1000); // 1 segundo
                    }
                }

                return "OK";
            }
            catch (Exception err)
            {
                return err.ToString();
            }
        }




        private List<EtiquetaMaterialesDTO> TransformarEtiquetas(List<ArticuloDTO> data)
        {
            var etiquetas = data
                .SelectMany(articulo => articulo.ordenes.Select(orden => new EtiquetaMaterialesDTO
                {
                    codigoArticulo = articulo.codigoArticulo,
                    nombreArticulo = articulo.nombreArticulo,
                    color = articulo.color,
                    area = articulo.area,
                    semana = orden.semana,
                    year = orden.year,
                    ordenTrabajo = orden.ordenTrabajo,
                    cantidadTransferida = orden.cantidadTransferida
                }))
                .ToList();

            return etiquetas;
        }

        private string GenerarEtiquetaZPL(EtiquetaMaterialesDTO orden, int posX)
        {
            var zpl = "";

            // OP
            zpl += $"^FO{posX},20";
            zpl += "^A0R,42,42";
            zpl += $"^FD{orden.ordenTrabajo}^FS";

            // Nombre
            string nombre = orden.nombreArticulo;
            string lineaUno = nombre.Length > 31 ? nombre.Substring(0, 31) : nombre;
            string lineaDos = nombre.Length > 31 ? nombre.Substring(31) : string.Empty;

            // primera linea
            zpl += $"^FO{posX - 28},20";
            zpl += "^A0R,15,15";
            zpl += $"^FD{lineaUno}^FS";

            // segunda linea
            if (!string.IsNullOrEmpty(lineaDos))
            {
                zpl += $"^FO{posX - 48},20";
                zpl += "^A0R,15,15";
                zpl += $"^FD{lineaDos}^FS";
            }

            // Color
            zpl += $"^FO{posX - 90},20";
            zpl += "^A0R,25,25";
            zpl += $"^FDCOLOR:{orden.color}   QTY: {orden.cantidadTransferida}^FS";

            // Area, semana y año
            zpl += $"^FO{posX - 124},20";
            zpl += "^A0R,25,25";
            zpl += $"^FDT:{orden.area}   {orden.semana} - {orden.year}^FS";


            return zpl;
        }

    }
}
