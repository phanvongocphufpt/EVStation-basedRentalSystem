using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _feedbackService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _feedbackService.GetByIdAsync(id);
            if (!result.IsSuccess) return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Staff,Admin,Customer")]
        public async Task<IActionResult> Create([FromBody] CreateFeedbackDTO dto)
        {
            var result = await _feedbackService.CreateAsync(dto);
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result.Data);
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Staff,Admin,Customer")]
        public async Task<IActionResult> Update([FromBody] UpdateFeedbackDTO dto)
        {
            var result = await _feedbackService.UpdateAsync(dto);
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result.Data);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Staff,Admin,Customer")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _feedbackService.DeleteAsync(id);
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result.Data);
        }
    }
}
