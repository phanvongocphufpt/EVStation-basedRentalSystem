using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Service.IServices;
using Service.DTOs;

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
        /// Phân tích dữ liệu tổng thể: xe, phản hồi, đơn hàng
        /// </summary>
        [HttpGet("analyze")]
        public async Task<IActionResult> AnalyzeData()
        {
            try
            {
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

                var prompt = $@"
Bạn là chuyên gia phân tích dữ liệu thuê xe.
Dưới đây là thông tin các xe, đơn hàng và phản hồi khách hàng:

Xe: {System.Text.Json.JsonSerializer.Serialize(cars)}
Phản hồi khách hàng: {System.Text.Json.JsonSerializer.Serialize(feedbacks)}
Đơn hàng: {System.Text.Json.JsonSerializer.Serialize(rentalOrders)}

Hãy phân tích và đưa ra gợi ý nâng cấp dịch vụ hoặc cải thiện xe.
Trả lời ngắn, tối đa 1000 ký tự.
Dễ nhìn, mỗi ý 1 dòng, không dùng *, ** hay markdown.
";

                var aiResponse = await _aiService.GenerateResponseAsync(prompt, shortAnswer: true);

                return Ok(new { response = aiResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing data for AI");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Phân tích tỷ lệ sử dụng xe và giờ cao điểm
        /// </summary>
        [HttpGet("car-usage")]
        public async Task<IActionResult> AnalyzeCarUsage()
        {
            try
            {
                // Lấy danh sách xe và các đơn thuê liên quan
                var cars = await _dbContext.Cars
                    .Where(c => !c.IsDeleted && c.IsActive)
                    .Select(c => new { c.Id, c.Name })
                    .ToListAsync();

                var rentalOrders = await _dbContext.RentalOrders
                    .Where(r => r.Status == Repository.Entities.Enum.RentalOrderStatus.Completed
                             || r.Status == Repository.Entities.Enum.RentalOrderStatus.Renting)
                    .ToListAsync();

                if (!cars.Any() || !rentalOrders.Any())
                    return Ok(new { response = "Chưa có dữ liệu thuê xe để phân tích." });

                // Tính tỷ lệ sử dụng xe
                var usageData = cars.Select(c =>
                {
                    var count = rentalOrders.Count(r => r.CarId == c.Id);
                    var ratio = cars.Count > 0 ? (double)count / rentalOrders.Count : 0;
                    return new { c.Name, Rentals = count, UsageRatio = Math.Round(ratio * 100, 2) + "%" };
                }).OrderByDescending(x => x.Rentals).ToList();

                // Tính giờ cao điểm theo PickupTime
                var peakHours = rentalOrders
                    .GroupBy(r => r.PickupTime.Hour)
                    .Select(g => new { Hour = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(3)
                    .ToList();

                var prompt = $@"
Bạn là chuyên gia phân tích dữ liệu thuê xe.
Dữ liệu hiện tại:

Tỷ lệ sử dụng xe:
{System.Text.Json.JsonSerializer.Serialize(usageData)}

Giờ cao điểm thuê xe:
{System.Text.Json.JsonSerializer.Serialize(peakHours)}

Hãy viết phân tích rõ ràng, dễ đọc, mỗi ý 1 dòng, tối đa 1000 ký tự.
Không dùng *, ** hay markdown.
";

                var aiResponse = await _aiService.GenerateResponseAsync(prompt, shortAnswer: true);

                return Ok(new { response = aiResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing car usage");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Chat với AI
        /// </summary>
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] Service.DTOs.ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { error = "Message không được để trống" });

            try
            {
                var reply = await _aiService.GenerateResponseAsync(request.Message, shortAnswer: request.ShortAnswer);
                return Ok(new Service.DTOs.ChatResponse { Reply = reply });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in chat endpoint");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
