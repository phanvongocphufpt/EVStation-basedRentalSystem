using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverLicenseController : ControllerBase
    {
        private readonly IDriverLicenseService _driverLicenseService;

        public DriverLicenseController(IDriverLicenseService driverLicenseService)
        {
            _driverLicenseService = driverLicenseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _driverLicenseService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _driverLicenseService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DriverLicense driverLicense)
        {
            var created = await _driverLicenseService.CreateAsync(driverLicense);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DriverLicense driverLicense)
        {
            if (id != driverLicense.Id) return BadRequest();
            var updated = await _driverLicenseService.UpdateAsync(driverLicense);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _driverLicenseService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}


