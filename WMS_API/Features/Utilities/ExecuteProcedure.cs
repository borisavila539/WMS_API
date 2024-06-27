using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WMS_API.Features.Utilities
{
    public class ExecuteProcedure
    {
        private readonly string _connectionString;

        public ExecuteProcedure(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<T>> ExecuteStoredProcedureList<T>(string storedProcedureName, List<SqlParameter> parameters) where T : new()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters.ToArray());
                        }

                        List<T> response = new List<T>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                T obj = GetMappedResult<T>(reader);
                                response.Add(obj);
                            }
                        }
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                // Maneja la excepción (por ejemplo, loguea el error o relanza la excepción)
                throw new Exception("Error al ejecutar el procedimiento almacenado", ex);
            }
        }

        public async Task<T> ExecuteStoredProcedure<T>(string storedProcedureName, List<SqlParameter> parameters) where T : new()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters.ToArray());
                        }

                        T response = new T();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                response = GetMappedResult<T>(reader);
                            }
                        }
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                // Maneja la excepción (por ejemplo, loguea el error o relanza la excepción)
                throw new Exception("Error al ejecutar el procedimiento almacenado", ex);
            }
        }

        private T GetMappedResult<T>(SqlDataReader reader) where T : new()
        {
            T obj = new T();

            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (!reader.HasColumn(prop.Name) || reader[prop.Name] == DBNull.Value)
                    continue;

                if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(obj, Convert.ToInt32(reader[prop.Name]));
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(obj, reader[prop.Name].ToString() != "0");
                }
                else if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(obj, reader[prop.Name].ToString());
                }
                else if(prop.PropertyType == typeof(Decimal))
                {
                    prop.SetValue(obj, Convert.ToDecimal(reader[prop.Name]));
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    prop.SetValue(obj, Convert.ToDateTime(reader[prop.Name]));
                }
                // Añadir más tipos según sea necesario
            }

            return obj;
        }
    }
}

public static class SqlDataReaderExtensions
{
    public static bool HasColumn(this SqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
