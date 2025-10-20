using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;
using System.Linq;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập
    public class RentalContactController : ControllerBase
    {
        private readonly IRentalContactService _service;

        public RentalContactController(IRentalContactService service)
        {
            _service = service;
        }

        // 🟢 Lấy tất cả hợp đồng
        // Admin/Staff thấy tất cả, User chỉ thấy hợp đồng của chính mình
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userIdClaim = User.FindFirst("Id");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            int userId = int.Parse(userIdClaim.Value);
            string userRole = roleClaim?.Value ?? "User";

            var contacts = await _service.GetAllAsync();

            if (userRole == "User")
                contacts = contacts.Where(c => c.LesseeId == userId && !c.IsDeleted);

            return Ok(contacts);
        }

        // 🔍 Lấy hợp đồng theo RentalOrderId
        [HttpGet("byRentalOrder/{rentalOrderId}")]
        public async Task<IActionResult> GetByRentalOrderId(int rentalOrderId)
        {
            var userIdClaim = User.FindFirst("Id");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            int userId = int.Parse(userIdClaim.Value);
            string userRole = roleClaim?.Value ?? "User";

            var contact = await _service.GetByRentalOrderIdAsync(rentalOrderId);
            if (contact == null || contact.IsDeleted)
                return NotFound($"Không tìm thấy hợp đồng với RentalOrderId = {rentalOrderId}");

            // User chỉ được xem hợp đồng của mình
            if (userRole == "User" && contact.LesseeId != userId)
                return Forbid("Bạn không có quyền xem hợp đồng này.");

            return Ok(contact);
        }

        // Tạo hợp đồng (chỉ Staff, Admin)
        [HttpPost]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Create([FromBody] RentalContact contact)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            contact.IsDeleted = false;
            contact.RentalDate = contact.RentalDate == default ? DateTime.UtcNow : contact.RentalDate;

            await _service.AddAsync(contact);
            return CreatedAtAction(nameof(GetByRentalOrderId), new { rentalOrderId = contact.RentalOrderId }, contact);
        }

        //  Cập nhật hợp đồng (chỉ Staff, Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] RentalContact contact)
        {
            if (id != contact.Id)
                return BadRequest("ID không khớp.");

            var existing = await _service.GetByRentalOrderIdAsync(id);
            if (existing == null || existing.IsDeleted)
                return NotFound("Không tìm thấy hợp đồng thuê.");

            await _service.UpdateAsync(contact);
            return Ok(new { Message = "Cập nhật hợp đồng thành công.", contact });
        }

        // Xóa mềm (chỉ Staff, Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _service.GetByRentalOrderIdAsync(id);
            if (contact == null || contact.IsDeleted)
                return NotFound("Không tìm thấy hợp đồng thuê.");

            await _service.DeleteAsync(id);
            return Ok(new { Message = "Đã xóa mềm hợp đồng thành công." });
        }
    }
}
