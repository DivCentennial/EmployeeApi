using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using EmployeeApi.Models;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")] // Swagger shows JSON
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
        {
            var employees = new List<Employee>();

            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            using (SqlCommand cmd = new SqlCommand("getEmp", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new Employee
                        {
                            EmpNo = reader.GetInt32(reader.GetOrdinal("EmpNo")),
                            Employee_Name = reader.GetString(reader.GetOrdinal("Employee_Name")),
                            //Salary = reader.GetInt32(reader.GetOrdinal("Salary")),
                            Department_ID = reader.GetInt32(reader.GetOrdinal("Department_ID"))
                        });
                    }
                }
            }

            return Ok(employees);
        }
    }

}
