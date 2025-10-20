using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;

using Service.IServices;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalContactController : ControllerBase
    {
        private readonly IRentalContactService _service;

        public RentalContactController(IRentalContactService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        // 🔍 Lấy hợp đồng theo RentalOrderId
        [HttpGet("byRentalOrder/{rentalOrderId}")]
        public async Task<IActionResult> GetByRentalOrderId(int rentalOrderId)
        {
            var contact = await _service.GetByRentalOrderIdAsync(rentalOrderId);
            if (contact == null) return NotFound($"Không tìm thấy hợp đồng với RentalOrderId = {rentalOrderId}");
            return Ok(contact);
        }

        [HttpPost]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Create([FromBody] RentalContact contact)
        {
            await _service.AddAsync(contact);
            return Ok(contact);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] RentalContact contact)
        {
            if (id != contact.Id) return BadRequest();
            await _service.UpdateAsync(contact);
            return Ok(contact);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
