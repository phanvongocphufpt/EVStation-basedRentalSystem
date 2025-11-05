using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly EVSDbContext _dbContext;
        private readonly ILogger<AIController> _logger;

        public AIController(IAIService aiService, EVSDbContext dbContext, ILogger<AIController> logger)
        {
            _aiService = aiService;
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Phân tích dữ liệu từ database và trả về gợi ý nâng cấp
        /// </summary>
        [HttpGet("analyze")]
        public async Task<IActionResult> AnalyzeData()
        {
            try
            {
                // 1. Lấy dữ liệu từ database (giới hạn sample để prompt ngắn)
                var cars = await _dbContext.Cars
                    .Where(c => !c.IsDeleted && c.IsActive)
                    .Take(5)
                    .Select(c => new { c.Name, c.Model, c.Seats, c.BatteryDuration, c.RentPricePerDay })
                    .ToListAsync();

                var feedbacks = await _dbContext.Feedbacks
                    .Where(f => !f.IsDeleted)
                    .OrderByDescending(f => f.CreatedAt)
                    .Take(5)
                    .Select(f => new { f.Title, f.Content })
                    .ToListAsync();

                var rentalOrders = await _dbContext.RentalOrders
                    .OrderByDescending(r => r.OrderDate)
                    .Take(5)
                    .Select(r => new { r.Id, r.WithDriver, r.Status, r.Total })
                    .ToListAsync();

                // 2. Tạo prompt gửi AI
                var prompt = @$"
Bạn là chuyên gia phân tích dữ liệu thuê xe.
Dưới đây là thông tin các xe, đơn hàng và phản hồi khách hàng:

Xe: {System.Text.Json.JsonSerializer.Serialize(cars)}
Phản hồi khách hàng: {System.Text.Json.JsonSerializer.Serialize(feedbacks)}
Đơn hàng: {System.Text.Json.JsonSerializer.Serialize(rentalOrders)}

Hãy phân tích và đưa ra gợi ý nâng cấp dịch vụ hoặc cải thiện xe.
Trả lời ngắn gọn, tối đa 300 ký tự.
";

                // 3. Gửi prompt tới AI
                var aiResponse = await _aiService.GenerateResponseAsync(prompt);

                // 4. Trả về response
                return Ok(new { response = aiResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing data for AI");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
