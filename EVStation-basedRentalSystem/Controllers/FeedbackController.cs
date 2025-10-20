using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpGet]
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

            await _feedbackService.AddAsync(fb);
            return Ok(fb);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Feedback fb)
        {
            if (id != fb.Id)
                return BadRequest("ID không khớp");

            await _feedbackService.UpdateAsync(fb);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _feedbackService.DeleteAsync(id);
            return NoContent();
        }
    }
}
