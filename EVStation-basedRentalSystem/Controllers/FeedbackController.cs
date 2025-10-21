using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu xác thực cho tất cả các hành động
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        // Không phân trang
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _feedbackService.GetAllAsync();
            return Ok(list);
        }
        // 🔹 GET: api/Feedback (phân trang + tìm kiếm)
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 5, [FromQuery] string? keyword = null)
        {
            var pagedFeedbacks = await _feedbackService.GetPagedAsync(pageIndex, pageSize, keyword);
            return Ok(pagedFeedbacks);
        }


        // 🔍 Tìm feedback theo tên xe
        [HttpGet("byCar/{carName}")]
        public async Task<IActionResult> GetByCarName(string carName)
        {
            var fb = await _feedbackService.GetByCarName(carName);
            if (fb == null)
                return NotFound($"Không tìm thấy feedback cho xe có tên chứa: {carName}");
            return Ok(fb);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Feedback fb)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 🔹 Lấy userId từ token JWT
            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");


            int userId = int.Parse(userIdClaim.Value);
            fb.UserId = userId; // 🔒 Gắn chủ sở hữu feedback

            // 🔹 Gắn ngày tạo
            fb.CreatedAt = DateTime.UtcNow;
            fb.IsDeleted = false;

            await _feedbackService.AddAsync(fb);

            return Ok(new
            {
                Message = "Thêm feedback thành công.",
                fb
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Feedback fb)
        {
            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            int userId = int.Parse(userIdClaim.Value);

            // 🔍 Lấy feedback cũ
            var existing = await _feedbackService.GetByCarName("carName");
            if (existing == null || existing.IsDeleted)
                return NotFound("Feedback không tồn tại.");

            // ❌ Không phải chủ sở hữu
            if (existing.UserId != userId)
                return Forbid("Bạn không có quyền chỉnh sửa feedback này.");

            // ✅ Cập nhật thông tin
            existing.Title = fb.Title;
            existing.Content = fb.Content;
            existing.UpdatedAt = DateTime.UtcNow;

            await _feedbackService.UpdateAsync(existing);

            return Ok(new { Message = "Cập nhật feedback thành công.", existing });
        }
        [HttpDelete("{id}")]
         public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            int userId = int.Parse(userIdClaim.Value);

            var feedback = await _feedbackService.GetByCarName("carName");
            if (feedback == null || feedback.IsDeleted)
                return NotFound("Feedback không tồn tại.");

            if (feedback.UserId != userId)
                return Forbid("Bạn không có quyền xóa feedback này.");

            await _feedbackService.DeleteAsync(id);

            return Ok(new { Message = "Đã xóa mềm feedback thành công." });
        }
    }
}
