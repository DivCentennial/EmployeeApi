using System.Data;
using System.Data.SqlClient;
using EmployeeApi.Models;

namespace EmployeeApi.Data
{
    public class ManagerRepository
    {
        private readonly string _connectionString;

        public ManagerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Manager>> GetAllManagersAsync()
        {
            var managers = new List<Manager>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("getManagers", conn)) // Use your stored proc name
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            managers.Add(new Manager
                            {
                                ManagerID = reader.GetInt32(reader.GetOrdinal("ManagerID")),
                                ManagerName = reader.GetString(reader.GetOrdinal("ManagerName"))
                            });
                        }
                    }
                }
            }

            return managers;
        }

        public async Task<Manager> GetManagerByIdAsync(int id)
        {
            Manager manager = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("getManagerById", conn)) // Stored proc name
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ManagerID", id);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            manager = new Manager
                            {
                                ManagerID = reader.GetInt32(reader.GetOrdinal("ManagerID")),
                                ManagerName = reader.GetString(reader.GetOrdinal("ManagerName"))
                            };
                        }
                    }
                }
            }

            return manager;
        }

        public async Task CreateManagerAsync(Manager manager)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("insertManager", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                   // cmd.Parameters.AddWithValue("@ManagerID", manager.ManagerID);
                    cmd.Parameters.AddWithValue("@ManagerName", manager.ManagerName);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateManagerAsync(Manager manager)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("updateManager", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ManagerID", manager.ManagerID);
                    cmd.Parameters.AddWithValue("@ManagerName", manager.ManagerName);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteManagerAsync(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("deleteManager", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ManagerID", id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
