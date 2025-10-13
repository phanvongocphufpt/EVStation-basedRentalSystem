using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitizenIdController : ControllerBase
    {
        private readonly ICitizenIdService _citizenIdService;

        public CitizenIdController(ICitizenIdService citizenIdService)
        {
            _citizenIdService = citizenIdService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _citizenIdService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _citizenIdService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CitizenId citizenId)
        {
            var created = await _citizenIdService.CreateAsync(citizenId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CitizenId citizenId)
        {
            if (id != citizenId.Id) return BadRequest();
            var updated = await _citizenIdService.UpdateAsync(citizenId);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _citizenIdService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}


