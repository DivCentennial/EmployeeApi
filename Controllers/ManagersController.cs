using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using EmployeeApi.Models;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ManagersController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Manager manager)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("insertManager", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ManagerID", manager.ManagerID);
                    cmd.Parameters.AddWithValue("@ManagerName", manager.ManagerName);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return CreatedAtAction(nameof(GetById), new { id = manager.ManagerID }, manager);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Manager manager)
        {
            if (id != manager.ManagerID)
                return BadRequest("ManagerID mismatch.");

            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
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

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("deleteManager", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ManagerID", id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return NoContent();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            Manager manager = null;
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SELECT ManagerID, ManagerName FROM Manager_Details WHERE ManagerID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            manager = new Manager
                            {
                                ManagerID = reader.GetInt32(0),
                                ManagerName = reader.GetString(1)
                            };
                        }
                    }
                }
            }

            if (manager == null) return NotFound();

            return Ok(manager);
        }
    }
}
