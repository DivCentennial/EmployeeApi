using System.Data;
using System.Data.SqlClient;
using EmployeeApi.Models;

namespace EmployeeApi.Data
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            var employees = new List<Employee>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("getEmp", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
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

            return employees;
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            Employee? employee = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("getEmpById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmpID", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            employee = new Employee
                            {
                                Empid = reader.GetInt32(reader.GetOrdinal("Empid")),
                                Ename = reader.GetString(reader.GetOrdinal("Emp_name")),
                                Dept_ID = reader.GetInt32(reader.GetOrdinal("DeptID"))
                            };
                        }
                    }
                }
            }

            return employee;
        }

        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("insertEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // If Empid is identity, remove this parameter
                    cmd.Parameters.AddWithValue("@Ename", employee.Ename);
                    cmd.Parameters.AddWithValue("@DeptID", employee.Dept_ID);

                    // If SP returns new ID:
                    var newId = (await cmd.ExecuteScalarAsync());
                    return Convert.ToInt32(newId);
                }
            }
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("updateEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Empid", employee.Empid);
                    cmd.Parameters.AddWithValue("@Ename", employee.Ename);
                    cmd.Parameters.AddWithValue("@DeptID", employee.Dept_ID);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("deleteEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Empid", id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
