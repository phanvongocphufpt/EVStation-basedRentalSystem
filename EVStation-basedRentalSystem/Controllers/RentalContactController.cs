using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập
    public class RentalContactController : ControllerBase
    {
        private readonly IRentalContactService _rentalContactService;

        public RentalContactController(IRentalContactService rentalContactService)
        {
            _rentalContactService = rentalContactService;
        }

        // 🟢 Lấy tất cả hợp đồng (chỉ Admin/Staff)
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rentalContactService.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        // 🔍 Lấy hợp đồng theo RentalOrderId
        [HttpGet("byRentalOrder/{rentalOrderId}")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByRentalOrderId(int rentalOrderId)
        {
            var result = await _rentalContactService.GetByRentalOrderIdAsync(rentalOrderId);

            if (!result.IsSuccess || result.Data == null)
                return NotFound(result.Message ?? $"Không tìm thấy hợp đồng với RentalOrderId = {rentalOrderId}");

            var userIdClaim = User.FindFirst("Id");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            int userId = int.Parse(userIdClaim.Value);
            string role = roleClaim?.Value ?? "Customer";

            // ✅ Khách chỉ xem được hợp đồng của chính họ
            if (role == "Customer" && result.Data.LesseeId != userId)
                return Forbid("Bạn không có quyền xem hợp đồng này.");

            return Ok(result.Data);
        }

        // 🟡 Tạo mới hợp đồng thuê
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] RentalContact contact)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            contact.IsDeleted = false;
            contact.RentalDate = contact.RentalDate == default
                ? DateTime.UtcNow
                : contact.RentalDate;

            var result = await _rentalContactService.AddAsync(contact);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return CreatedAtAction(nameof(GetByRentalOrderId),
                new { rentalOrderId = result.Data.RentalOrderId },
                result.Data);
        }

        // 🟠 Cập nhật hợp đồng
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(int id, [FromBody] RentalContact contact)
        {
            if (contact.RentalOrderId == null)
                return BadRequest("RentalOrderId không được để trống.");

            var existing = await _rentalContactService.GetByRentalOrderIdAsync(contact.RentalOrderId.Value);

            if (!existing.IsSuccess || existing.Data == null)
                return NotFound("Không tìm thấy hợp đồng thuê.");

            var result = await _rentalContactService.UpdateAsync(contact);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(new { Message = "Cập nhật hợp đồng thành công.", result.Data });
        }

        // 🔴 Xóa hợp đồng
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _rentalContactService.GetByRentalOrderIdAsync(id);
            if (!existing.IsSuccess || existing.Data == null)
                return NotFound("Không tìm thấy hợp đồng thuê.");

            var result = await _rentalContactService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(new { Message = "Đã xóa hợp đồng thành công." });
        }
    }
}
