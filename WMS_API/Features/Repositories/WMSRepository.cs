using Core.DTOs;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace WMS_API.Features.Repositories
{
    public class WMSRepository: IWMSRepository
    {
        private readonly string _connectionStringAX;

        public WMSRepository(IConfiguration configuracion)
        {
            _connectionStringAX = configuracion.GetConnectionString("MicrosoftDynamicsAX_PRO");
        }

        public async Task<LoginDTO> PostLogin(LoginDTO datos)
        {
            using (SqlConnection sql = new SqlConnection(_connectionStringAX))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_Login_WMS]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@user",datos.user));
                    cmd.Parameters.Add(new SqlParameter("@pass", datos.pass));

                    var response = new LoginDTO();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            response = getLogin(reader);
                        }
                    }
                    return response;
                }
            }
        }
        public LoginDTO getLogin(SqlDataReader reader)
        {
            return new LoginDTO()
            {
                user = reader["user"].ToString(),
                pass = reader["pass"].ToString(),
                logeado = reader["logeado"].ToString() != "0" ? true:false

            };
        }

        public async Task<List<DiariosAbiertosDTO>> GetDiariosAbiertos(string user, string filtro)
        {
            using (SqlConnection sql = new SqlConnection(_connectionStringAX))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_ObtenerDiariosMovimientoAbiertos]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@user", user));
                    cmd.Parameters.Add(new SqlParameter("@filtro", filtro));

                    var response = new List<DiariosAbiertosDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetDiariosAbiertos(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public DiariosAbiertosDTO GetDiariosAbiertos(SqlDataReader reader)
        {
            return new DiariosAbiertosDTO()
            {
                JOURNALID = reader["JOURNALID"].ToString(),
                DESCRIPTION = reader["DESCRIPTION"].ToString(),
                NUMOFLINES = Convert.ToInt32(reader["NUMOFLINES"].ToString()),
                JOURNALNAMEID = reader["JOURNALNAMEID"].ToString(),
            };
        }

        public async Task<List<LineasDTO>> GetLineasDiario(string diario)
        {
            using (SqlConnection sql = new SqlConnection(_connectionStringAX))
            {
                using (SqlCommand cmd = new SqlCommand("[IM_ObtenerLineasDiario]", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@JOURNALID", diario));

                    var response = new List<LineasDTO>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(GetLineasDiario(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public LineasDTO GetLineasDiario(SqlDataReader reader)
        {
            return new LineasDTO()
            {
                ITEMBARCODE = reader["ITEMBARCODE"].ToString(),
                ITEMID = reader["ITEMID"].ToString(),
                INVENTCOLORID = reader["INVENTCOLORID"].ToString(),
                INVENTSIZEID = reader["INVENTSIZEID"].ToString(),
                QTY = Convert.ToInt32(Convert.ToDecimal(reader["QTY"].ToString())),
               
            };
        }
    } 
}
