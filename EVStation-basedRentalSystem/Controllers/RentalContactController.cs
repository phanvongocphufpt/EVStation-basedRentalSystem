using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;
using Service.Services;
using System.Linq;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập
    public class RentalContactController : ControllerBase
    {
        private readonly IRentalContactService _rentalContactService;

        public RentalContactController(IRentalContactService service)
        {
            _rentalContactService = service;
        }

        // 🟢 Lấy tất cả hợp đồng
        // Admin/Staff thấy tất cả, User chỉ thấy hợp đồng của chính mình
        [HttpGet]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _rentalContactService.GetAllAsync();
            return Ok(list);
        }

        // 🔍 Lấy hợp đồng theo RentalOrderId
        [HttpGet("byRentalOrder/{rentalOrderId}")]
        public async Task<IActionResult> GetByRentalOrderId(int rentalOrderId)
        {
            var userIdClaim = User.FindFirst("id");
            var roleClaim = User.FindFirst("role");

            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            int userId = int.Parse(userIdClaim.Value);
            string userRole = roleClaim?.Value ?? "User";

            var contact = await _rentalContactService.GetByRentalOrderIdAsync(rentalOrderId);
            if (contact == null || contact.IsDeleted)
                return NotFound($"Không tìm thấy hợp đồng với RentalOrderId = {rentalOrderId}");

            // User chỉ được xem hợp đồng của mình
            if (userRole == "User" && contact.LesseeId != userId)
                return Forbid("Bạn không có quyền xem hợp đồng này.");

            return Ok(contact);
        }

        // 🟡 Tạo hợp đồng (chỉ Staff, Admin)
        [HttpPost]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Create([FromBody] RentalContact contact)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            contact.IsDeleted = false;
            contact.RentalDate = contact.RentalDate == default ? DateTime.UtcNow : contact.RentalDate;

            await _rentalContactService.AddAsync(contact);
            return CreatedAtAction(nameof(GetByRentalOrderId), new { rentalOrderId = contact.RentalOrderId }, contact);
        }

        //  Cập nhật hợp đồng (chỉ Staff, Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] RentalContact contact)
        {
            if (id != contact.Id)
                return BadRequest("ID không khớp.");

            var existing = await _rentalContactService.GetByRentalOrderIdAsync(id);
            if (existing == null || existing.IsDeleted)
                return NotFound("Không tìm thấy hợp đồng thuê.");

            await _rentalContactService.UpdateAsync(contact);
            return Ok(new { Message = "Cập nhật hợp đồng thành công.", contact });
        }

        // 🔴 Xóa mềm (chỉ Staff, Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _rentalContactService.GetByRentalOrderIdAsync(id);
            if (contact == null || contact.IsDeleted)
                return NotFound("Không tìm thấy hợp đồng thuê.");

            await _rentalContactService.DeleteAsync(id);
            return Ok(new { Message = "Đã xóa mềm hợp đồng thành công." });
        }
    }
}
