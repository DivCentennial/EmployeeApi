using Microsoft.AspNetCore.Mvc;
using EmployeeApi.Data;
using EmployeeApi.Models;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagersController : ControllerBase
    {
        private readonly ManagerRepository _repository;

        public ManagersController(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            _repository = new ManagerRepository(connectionString);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Manager>>> GetAll()
        {
            var managers = await _repository.GetAllManagersAsync();
            return Ok(managers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Manager>> GetById(int id)
        {
            var manager = await _repository.GetManagerByIdAsync(id);
            if (manager == null) return NotFound();
            return Ok(manager);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Manager manager)
        {
            await _repository.CreateManagerAsync(manager);
            return CreatedAtAction(nameof(GetById), new { id = manager.ManagerID }, manager);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Manager manager)
        {
            if (id != manager.ManagerID)
                return BadRequest("Manager ID mismatch.");

            await _repository.UpdateManagerAsync(manager);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteManagerAsync(id);
            return NoContent();
        }
    }
}
