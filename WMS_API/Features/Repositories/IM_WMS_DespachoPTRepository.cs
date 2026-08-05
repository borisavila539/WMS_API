using Core.DTOs.Despacho_PT;
using Core.DTOs.Despacho_PT.GenerarDespachoPorImportacionExcel;
using Core.Interfaces;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WMS_API.Features.Utilities;

namespace WMS_API.Features.Repositories
{
    public class IM_WMS_DespachoPTRepository: IIM_WMS_DespachoPTRepository
    {
        private readonly string _connectionString;
        public IM_WMS_DespachoPTRepository(IConfiguration  configuration) 
        {
            _connectionString = configuration.GetConnectionString("IMFinanzas");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        private async Task<bool> ValidarLineaDuplicada(string almacen, string prodID, string itemID, string size, int qty)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Almacen", almacen ?? ""),
                new SqlParameter("@ProdID", prodID ?? ""),
                new SqlParameter("@ItemID", itemID ?? ""),
                new SqlParameter("@Size", size ?? ""),
                new SqlParameter("@Qty", qty)
            };

            var result = await executeProcedure.ExecuteStoredProcedure<RespuestaValidacionDespacho>("[IM_WMS_Validar_Despacho_Duplicado]", parametros);

            return result.EsDuplicado == 1;
        }

        public async Task<DespachoImportResultDto> ProcesarYGuardarDespachoExcel(IFormFile file, int userCreated)
        {
            try
            {

                // 1. Parsear Excel
                DespachoImportResultDto parsedData = ParsearExcel(file);
                foreach (var detalle in parsedData.Detalle)
                {
                    bool esDuplicado = await ValidarLineaDuplicada(
                        parsedData.Cabecera.Almacen,
                        detalle.ProdID,
                        detalle.ItemID,
                        detalle.Size,
                        detalle.Qty
                    );

                    if (esDuplicado)
                    {
                        return new DespachoImportResultDto
                        {
                            Success = false,
                            Message = $"El despacho ya existe o fue procesado previamente. Se detectó duplicado en la OP '{detalle.ProdID}', Artículo '{detalle.ItemID}' y Talla '{detalle.Size}' para el Almacén '{parsedData.Cabecera.Almacen}'."
                        };
                    }
                }

                if (parsedData == null || parsedData.Cabecera == null)
                    throw new Exception("No se pudo extraer la información del archivo Excel.");

                // 2. Insert Cabecera y obtener el ID generado
                int despachoID = await InsertDespachoCabeceraImportExcel(parsedData.Cabecera, userCreated);

                if (despachoID <= 0)
                    throw new Exception("Error al generar el encabezado del despacho en la base de datos.");

                // 3. Inser Detalle en bucle
                foreach (var detalle in parsedData.Detalle)
                {
                    await InsertDespachoDetalleImportExcel(despachoID, detalle, userCreated.ToString());
                }

                // 4. Insertar Estatus de Unidades en bucle
                foreach (var estatus in parsedData.EstatusOP)
                {
                    await InsertEstatusUnidadesOPImportExcel(despachoID, estatus,"Automatico/ImportacionExcel");
                }

                // 5. Agregar a Secuencia para liquidación
                await GetSecuencia_PL_PTImportExcel(despachoID, int.Parse(parsedData.Cabecera.Almacen), 1);

                parsedData.DespachoID = despachoID;
                parsedData.Success = true;
                parsedData.Message = $"Despacho #{despachoID} importado y guardado con éxito.";
                return parsedData;
            }
            catch (Exception ex)
            {
                return new DespachoImportResultDto
                {
                    Success = false,
                    Message = $"Error al procesar el despacho: {ex.Message}"
                };
            }
        }
        public DespachoImportResultDto ParsearExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("El archivo Excel está vacío o no es válido.");

            var resultDto = new DespachoImportResultDto
            {
                Success = true,
                Message = "Archivo parseado exitosamente."
            };

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

                    // 1. CABECERA
                    string almacen = table.Rows[6][2]?.ToString()?.Trim() ?? "";

                    int.TryParse(table.Rows[8][6]?.ToString(), out int cajaSegundas);
                    int.TryParse(table.Rows[8][7]?.ToString(), out int cajasTerceras);

                    resultDto.Cabecera = new DespachoCabeceraDto
                    {
                        Driver = "Despacho por plantilla/manual",
                        Truck = "",
                        EstadoID = 3,
                        Almacen = almacen,
                        CreatedDateTime = DateTime.Now.ToString(),
                        CajaSegundas = cajaSegundas,
                        CajasTerceras = cajasTerceras
                    };

                    // 2. DETALLE Y ESTATUS 
                    for (int row = 12; row < table.Rows.Count; row++)
                    {
                        DataRow dr = table.Rows[row];
                        string prodID = dr[8]?.ToString()?.Trim();
                        string itemID = dr[2]?.ToString()?.Trim();

                        // Omitir filas vacías
                        if (string.IsNullOrEmpty(prodID) && string.IsNullOrEmpty(itemID))
                            continue;

                        string prodCutSheetID = !string.IsNullOrEmpty(prodID) && prodID.Length > 1
                            ? prodID.Substring(0, prodID.Length - 1)
                            : prodID ?? "";

                        int box = 1;      
                        string size = dr[7]?.ToString()?.Trim() ?? "";   
                        string color = dr[6]?.ToString()?.Trim() ?? ""; 
                        int.TryParse(dr[12]?.ToString(), out int qty); 

                        // Agregar registro al Detalle
                        resultDto.Detalle.Add(new DespachoDetalleDto
                        {
                            ProdCutSheetID = prodCutSheetID,
                            Box = box,
                            Size = size,
                            Color = color,
                            ItemID = itemID,
                            ProdID = prodID,
                            Qty = qty
                        });

                        // Estado de Unidades 
                        int.TryParse(dr[13]?.ToString(), out int costura1);
                        int.TryParse(dr[14]?.ToString(), out int textil1);
                        int.TryParse(dr[15]?.ToString(), out int costura2);
                        int.TryParse(dr[16]?.ToString(), out int textil2);

                        resultDto.EstatusOP.Add(new EstatusUnidadesDto
                        {
                            ProdID = prodID,
                            Size = size,
                            Costura1 = costura1,
                            Textil1 = textil1,
                            Costura2 = costura2,
                            Textil2 = textil2
                        });
                    }

                    return resultDto;
                }
            }
        }
        public async Task<int> InsertDespachoCabeceraImportExcel(DespachoCabeceraDto cabecera, int userCreated)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Driver", string.IsNullOrEmpty(cabecera.Driver) ? "Despacho por plantilla/manual" : cabecera.Driver),
                new SqlParameter("@Truck", cabecera.Truck ?? ""),
                new SqlParameter("@EstadoID", cabecera.EstadoID),
                new SqlParameter("@UserCreated", userCreated),
                new SqlParameter("@Almacen", cabecera.Almacen ?? ""),
                new SqlParameter("@CajaSegundas", cabecera.CajaSegundas),
                new SqlParameter("@CajasTerceras", cabecera.CajasTerceras)
            };
            try
            {

                SpResponseDTO result = await executeProcedure.ExecuteStoredProcedure<SpResponseDTO>("[IM_WMS_Insert_Despacho_PTImportExcel]", parametros);

                return result.ID;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<bool> InsertDespachoDetalleImportExcel(int despachoID, DespachoDetalleDto detalle, string userCreated)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", despachoID),
                new SqlParameter("@ProdID", detalle.ProdID ?? ""),
                new SqlParameter("@ProdCutSheetID", detalle.ProdCutSheetID ?? ""),
                new SqlParameter("@ItemID", detalle.ItemID ?? ""),
                new SqlParameter("@Size", detalle.Size ?? ""),
                new SqlParameter("@Color", detalle.Color ?? ""),
                new SqlParameter("@Box", detalle.Box),
                new SqlParameter("@Qty", detalle.Qty),
                new SqlParameter("@UserPicking", userCreated)
            };
            try
            {
                SpResponseDTO result = await executeProcedure.ExecuteStoredProcedure<SpResponseDTO>("[IM_WMS_Insert_Despacho_PT_DetalleImportExcel]", parametros);

                return result.Success;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> InsertEstatusUnidadesOPImportExcel(int despachoID, EstatusUnidadesDto estatus, string userCreate)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@ProdID", estatus.ProdID ?? ""),
                new SqlParameter("@Size", estatus.Size ?? ""),
                new SqlParameter("@Costura1", estatus.Costura1),
                new SqlParameter("@Textil1", estatus.Textil1),
                new SqlParameter("@Costura2", estatus.Costura2),
                new SqlParameter("@Textil2", estatus.Textil2),
                new SqlParameter("@CreatedDate", DateTime.Now),
                new SqlParameter("@UserCreated", userCreate),
            };
            try
            {
                SpResponseDTO result = await executeProcedure.ExecuteStoredProcedure<SpResponseDTO>("[IM_WMS_Insert_Estatus_Unidades_OPImportExcel]", parametros);

                return result.Success;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IM_WMS_ObtenerSecuencia_PL_PT_DTO> GetSecuencia_PL_PTImportExcel(int despachoID, int almacenFrom, int almacenTo)
        {
            ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@DespachoID", despachoID),
                new SqlParameter("@AlmacenFrom", almacenFrom),
                new SqlParameter("@AlmacenTO", almacenTo)
            };

            try
            {
                IM_WMS_ObtenerSecuencia_PL_PT_DTO result = await executeProcedure.ExecuteStoredProcedure<IM_WMS_ObtenerSecuencia_PL_PT_DTO>("[IM_WMS_ObtenerSecuencia_PL_PT_ImportExcel]", parametros);

                return result;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DespachoImportResultDto> ObtenerDespachoPorId(int despachoID)
        {
            try
            {
                ExecuteProcedure executeProcedure = new ExecuteProcedure(_connectionString);
                var parametrosCabecera = new List<SqlParameter>
                {
                    new SqlParameter("@DespachoID", despachoID)
                };
                var parametrosDetalle = new List<SqlParameter>
                {
                    new SqlParameter("@DespachoID", despachoID)
                };
                var parametrosUnidades = new List<SqlParameter>
                {
                    new SqlParameter("@DespachoID", despachoID)
                };

                // 1.Cabecera
                DespachoCabeceraDto cabecera = await executeProcedure.ExecuteStoredProcedure<DespachoCabeceraDto>(
                    "[IM_WMS_Get_Despacho_PT_Cabecera]",
                    parametrosCabecera
                );

                if (cabecera == null || string.IsNullOrEmpty(cabecera.Almacen))
                {
                    return new DespachoImportResultDto
                    {
                        Success = false,
                        Message = $"No se encontró ningún despacho con el ID #{despachoID}."
                    };
                }

                // 2. Detalles
                List<DespachoDetalleDto> detalles;
                try
                {

                 detalles = await executeProcedure.ExecuteStoredProcedureList<DespachoDetalleDto>(
                    "[IM_WMS_Get_Despacho_PT_Detalle]",
                    parametrosDetalle
                );
                }
                catch (Exception)
                {

                    throw;
                }

                // 3.Estatus de Unidades
                List<EstatusUnidadesDto> estatus = await executeProcedure.ExecuteStoredProcedureList<EstatusUnidadesDto>(
                    "[IM_WMS_Get_Estatus_Unidades_OP]",
                    parametrosUnidades
                );

                return new DespachoImportResultDto
                {
                    Success = true,
                    Message = "Despacho consultado exitosamente.",
                    Cabecera = cabecera,
                    Detalle = detalles ?? new List<DespachoDetalleDto>(),
                    EstatusOP = estatus ?? new List<EstatusUnidadesDto>()
                };
            }
            catch (Exception ex)
            {
                return new DespachoImportResultDto
                {
                    Success = false,
                    Message = $"Error al obtener el despacho: {ex.Message}"
                };
            }
        }

    }
}
