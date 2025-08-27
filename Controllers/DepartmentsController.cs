using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using EmployeeApi.Models;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("insertDepartment", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Department_ID", department.Department_ID);
                    cmd.Parameters.AddWithValue("@Department_Name", department.Department_Name);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return CreatedAtAction(nameof(GetById), new { id = department.Department_ID }, department);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Department department)
        {
            if (id != department.Department_ID)
                return BadRequest("Department_ID mismatch.");

            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("updateDepartment", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Department_ID", department.Department_ID);
                    cmd.Parameters.AddWithValue("@Department_Name", department.Department_Name);

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

                using (SqlCommand cmd = new SqlCommand("deleteDepartment", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Department_ID", id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return NoContent();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            Department department = null;
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SELECT Department_ID, Department_Name FROM Department_Details WHERE Department_ID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            department = new Department
                            {
                                Department_ID = reader.GetInt32(0),
                                Department_Name = reader.GetString(1)
                            };
                        }
                    }
                }
            }

            if (department == null) return NotFound();

            return Ok(department);
        }
    }
}
