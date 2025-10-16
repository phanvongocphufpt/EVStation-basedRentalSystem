using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var feedback = await _feedbackService.GetByIdAsync(id);
            if (feedback == null)
                return NotFound(new { message = "Feedback not found." });

            return Ok(feedback);
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
                return BadRequest();

            if (!await _feedbackService.ExistsAsync(id))
                return NotFound();

            await _feedbackService.UpdateAsync(feedback);
            return Ok(new { message = "Feedback updated successfully!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _feedbackService.ExistsAsync(id))
                return NotFound();

            await _feedbackService.DeleteAsync(id);
            return Ok(new { message = "Feedback deleted successfully!" });
        }
    }
}
