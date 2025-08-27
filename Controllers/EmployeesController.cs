using Microsoft.AspNetCore.Mvc;
using EmployeeApi.Data;
using EmployeeApi.Models;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeRepository _repository;

        public EmployeesController(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            _repository = new EmployeeRepository(connectionString);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
        {
            var employees = await _repository.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _repository.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            // If Empid is identity, we don’t pass it
            var newId = await _repository.CreateEmployeeAsync(employee);
            employee.Empid = newId;

            return CreatedAtAction(nameof(GetById), new { id = employee.Empid }, employee);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Employee employee)
        {
            if (id != employee.Empid)
                return BadRequest("Employee ID mismatch.");

            await _repository.UpdateEmployeeAsync(employee);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteEmployeeAsync(id);
            return NoContent();
        }
    }
}
