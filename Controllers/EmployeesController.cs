using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using EmployeeApi.Models;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
        {
            var employees = new List<Employee>();
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                /*// ✅ Debug: Check connected DB
                using (SqlCommand checkCmd = new SqlCommand("SELECT DB_NAME()", conn))
                {
                    var dbName = (await checkCmd.ExecuteScalarAsync())?.ToString();
                    Console.WriteLine($"[DEBUG] Connected to DB: {dbName}");
                }*/  //WITHOUT THIS IS ALSO WORKING, JUST MAKE IT SIMPLE

                // ✅ Call stored procedure
                using (SqlCommand cmd = new SqlCommand("getEmp", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employees.Add(new Employee
                            {
                                Empid = reader.GetInt32(reader.GetOrdinal("Empid")),
                                Ename = reader.GetString(reader.GetOrdinal("Ename")),
                                Dept_ID = reader.GetInt32(reader.GetOrdinal("DeptID"))
                            });
                        }
                    }
                }
            }

            return Ok(employees);
        }


        // GET: api/Employees/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("getEmpById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmpID", id);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var employee = new Employee
                            {
                                Empid = reader.GetInt32(reader.GetOrdinal("Empid")),
                                Ename = reader.GetString(reader.GetOrdinal("Emp_name")),
                                Dept_ID = reader.GetInt32(reader.GetOrdinal("DeptID"))
                            };
                            return Ok(employee);
                        }
                        else
                        {
                            return NotFound(); // ID not found
                        }
                    }
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("insertEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // supply all three fields
                    cmd.Parameters.AddWithValue("@Empid", employee.Empid);
                    cmd.Parameters.AddWithValue("@Ename", employee.Ename);
                    cmd.Parameters.AddWithValue("@DeptID", employee.Dept_ID);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Empid }, employee);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Employee employee)
        {
            if (id != employee.Empid)
                return BadRequest("Empid mismatch.");

            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("updateEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Empid", employee.Empid);
                    cmd.Parameters.AddWithValue("@Ename", employee.Ename);
                    cmd.Parameters.AddWithValue("@DeptID", employee.Dept_ID);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return NoContent(); // 204 is standard for PUT
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("deleteEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Empid", id);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                        return NotFound(); // No employee with this id
                }
            }

            return NoContent(); // 204 is standard for DELETE
        }



    }
}
