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
        private readonly IRentalContactService _rentalContactService;

        public RentalContactController(IRentalContactService rentalContactService)
        {
            _rentalContactService = rentalContactService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contacts = await _rentalContactService.GetAllAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _rentalContactService.GetByIdAsync(id);
            if (contact == null)
                return NotFound(new { message = "RentalContact not found!" });

            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RentalContact contact)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _rentalContactService.AddAsync(contact);
            return Ok(new { message = "RentalContact created successfully!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RentalContact contact)
        {
            if (id != contact.Id)
                return BadRequest("RentalContact ID mismatch");

            await _rentalContactService.UpdateAsync(contact);
            return Ok(new { message = "RentalContact updated successfully!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _rentalContactService.DeleteAsync(id);
            return Ok(new { message = "RentalContact deleted successfully!" });
        }
    }
}
