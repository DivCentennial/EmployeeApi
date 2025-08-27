using Microsoft.AspNetCore.Mvc;
using EmployeeApi.Models;
using EmployeeApi.Data;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentRepository _repository;

        public DepartmentsController(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            _repository = new DepartmentRepository(connectionString);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {
            await _repository.CreateAsync(department);
            return CreatedAtAction(nameof(GetById), new { id = department.Department_ID }, department);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Department department)
        {
            if (id != department.Department_ID)
                return BadRequest("Department_ID mismatch.");

            await _repository.UpdateAsync(department);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _repository.GetByIdAsync(id);
            if (department == null) return NotFound();
            return Ok(department);
        }
    }
}
