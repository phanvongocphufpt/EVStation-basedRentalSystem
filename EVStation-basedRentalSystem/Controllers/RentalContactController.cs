using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalContactController : ControllerBase
    {
        private readonly IRentalContactService _service;

        public RentalContactController(IRentalContactService service)
        {
            _service = service;
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (!result.IsSuccess)
                return StatusCode(500, result.Message); // lỗi server hoặc mapping

            return Ok(result.Data);
        }

        [HttpGet("byRentalOrder/{rentalOrderId}")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByRentalOrderId(int rentalOrderId)
        {
            var result = await _service.GetByRentalOrderIdAsync(rentalOrderId);
            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> Create([FromBody] RentalContactCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AddAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

     
        [HttpPut]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update([FromBody] RentalContactUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(dto);
            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

   
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}
