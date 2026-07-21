using Core.DTOs.ClaseRespuesta;
using Core.DTOs.DiseñoEtiqueta;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class DiseñoEtiquetaRepository : IDiseñoEtiquetaRepository
    {
        private readonly string _connectionString;
        public DiseñoEtiquetaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("IMFinanzas");
        }

        public async Task<Respuesta<bool>> ImprimirPrueba(SolicitudImpresionDto request)
        {
            // 1. Validaciones iniciales
            if (request == null)
            {
                return Respuesta<bool>.Error("La solicitud no puede ser nula.");
            }

            if (string.IsNullOrWhiteSpace(request.Impresora))
            {
                return Respuesta<bool>.Error("Debe especificar la dirección IP de la impresora.");
            }

            if (string.IsNullOrWhiteSpace(request.Zpl))
            {
                return Respuesta<bool>.Error("El contenido ZPL a imprimir está vacío.");
            }

            if (request.Cantidad < 1)
            {
                return Respuesta<bool>.Error("La cantidad de impresiones debe ser al menos 1.");
            }

            int puertoEstandarZebra = 9100;

            try
            {
                using (var tcpClient = new TcpClient())
                {
                    // Intento de conexión con timeout de 4 segundos
                    var conexionTask = tcpClient.ConnectAsync(request.Impresora, puertoEstandarZebra);
                    var timeoutTask = Task.Delay(4000);

                    var taskCompletada = await Task.WhenAny(conexionTask, timeoutTask);

                    if (taskCompletada == timeoutTask)
                    {
                        return Respuesta<bool>.Error($"Tiempo de espera agotado al conectar con la impresora ({request.Impresora}). Verifique que esté encendida y en la red.");
                    }

                    // Confirmar si hubo alguna excepción en la conexión
                    await conexionTask;

                    // Enviar los bytes del ZPL vía TCP Stream
                    using (NetworkStream stream = tcpClient.GetStream())
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(request.Zpl);

                        await stream.WriteAsync(buffer, 0, buffer.Length);
                        await stream.FlushAsync();
                    }
                }

                return Respuesta<bool>.Ok(true, $"Se enviaron {request.Cantidad} etiqueta(s) correctamente a la impresora {request.Impresora}.");
            }
            catch (SocketException ex)
            {
                return Respuesta<bool>.Error($"Error de red al conectar con la impresora ({request.Impresora}): {ex.Message}");
            }
            catch (Exception ex)
            {
                return Respuesta<bool>.Error($"Ocurrió un error inesperado al procesar la impresión: {ex.Message}");
            }
        }

        public async Task<Respuesta<bool>> GuardarDiseño(List<ElementoEtiquetaDto> elementos)
        {
            if (elementos == null || elementos.Count == 0)
            {
                return Respuesta<bool>.Error("El diseño no contiene elementos válidos.");
            }

            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

                var parametrosCabecera = new List<SqlParameter>
                {
                    new SqlParameter("@CodigoDiseño", "UBICACION_RACK"),
                    new SqlParameter("@NombreDiseño", "Etiqueta de Ubicación de Rack"),
                    new SqlParameter("@AnchoMM", 100),
                    new SqlParameter("@AltoMM", 50)
                };

                var resultadoCabecera = await executeProcedure.ExecuteStoredProcedure<RespuestaDiseñoIdDto>(
                    "[dbo].[IM_WMS_GuardarEtiquetaDiseñoCabecera]",
                    parametrosCabecera
                );

                if (resultadoCabecera == null || resultadoCabecera.DiseñoId <= 0)
                {
                    return Respuesta<bool>.Error("No se pudo obtener el ID del diseño guardado.");
                }

                int diseñoId = resultadoCabecera.DiseñoId;

                int orden = 1;
                foreach (var item in elementos)
                {
                    var parametrosDetalle = new List<SqlParameter>
                    {
                        new SqlParameter("@EtiquetaDiseñoId", diseñoId),
                        new SqlParameter("@ElementoId", item.Id ?? string.Empty),
                        new SqlParameter("@NombreElemento", item.Nombre ?? string.Empty),
                        new SqlParameter("@TipoElemento", item.Tipo ?? string.Empty),
                        new SqlParameter("@TextoPredeterminado", (object)item.Texto ?? DBNull.Value),
                        new SqlParameter("@PosicionX", item.X),
                        new SqlParameter("@PosicionY", item.Y),
                        new SqlParameter("@TamanioFuente", (object)item.FontSize ?? DBNull.Value),
                        new SqlParameter("@EsNegrita", item.Bold),
                        new SqlParameter("@Orden", orden++)
                    };

                    await executeProcedure.ExecuteStoredProcedure<RespuestaDiseñoIdDto>(
                        "[dbo].[IM_WMS_GuardarEtiquetaDiseñoDetalle]",
                        parametrosDetalle
                    );
                }

                return Respuesta<bool>.Ok(true, "Diseño guardado correctamente en base de datos.");
            }
            catch (Exception ex)
            {
                return Respuesta<bool>.Error($"Error al guardar el diseño de etiqueta: {ex.Message}");
            }
        }
        public async Task<Respuesta<List<ElementoEtiquetaDto>>> ObtenerDiseño(string codigoDiseño = "UBICACION_RACK")
        {
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

                var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@CodigoDiseño", codigoDiseño)
                };

                List<ElementoEtiquetaDto> elementos = await executeProcedure.ExecuteStoredProcedureList<ElementoEtiquetaDto>(
                    "[dbo].[IM_WMS_ObtenerDiseñoEtiqueta]",
                    parametros
                );

                if (elementos == null || elementos.Count == 0)
                {
                    return Respuesta<List<ElementoEtiquetaDto>>.Error("No se encontró ningún diseño previo guardado.");
                }

                return Respuesta<List<ElementoEtiquetaDto>>.Ok(elementos, "Diseño cargado correctamente.");
            }
            catch (Exception ex)
            {
                return Respuesta<List<ElementoEtiquetaDto>>.Error($"Error al obtener el diseño de etiqueta: {ex.Message}");
            }
        }
    }
}
