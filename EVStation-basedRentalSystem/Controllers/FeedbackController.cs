using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.Interfaces;
using Service.IServices;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var feedbacks = await _feedbackService.GetAllAsync();
            return Ok(feedbacks);
        }

        // 🔍 Tìm feedback theo RentalOrderId (gắn với 1 chiếc xe cụ thể)
        [HttpGet("byCar/{carName}")]
        public async Task<IActionResult> GetByCarName(string carName)
        {
            var fb = await _feedbackService.GetByCarName(carName);
            if (fb == null)
                return NotFound($"Không tìm thấy feedback cho xe có tên chứa: {carName}");
            return Ok(fb);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Feedback feedback)
        {
            await _feedbackService.AddAsync(feedback);
            return Ok(new { message = "Feedback created successfully!" });
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Feedback feedback)
        {
            if (id != feedback.Id)
                return BadRequest("ID không khớp");
            await _feedbackService.UpdateAsync(feedback);
            return NoContent();
        }

        // 🔥 XÓA MỀM THEO ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _feedbackService.DeleteAsync(id);
            return NoContent();
        }
    }
}
