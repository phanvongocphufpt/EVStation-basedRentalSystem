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
                var result = await _aiService.AnalyzeCarUsageWithSuggestionsAsync(shortAnswer: true);
                return Ok(new { analysis = result.Analysis, suggestions = result.Suggestions });
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
                // Lấy thông tin về dịch vụ để làm context
                var cars = await _dbContext.Cars
                    .Where(c => !c.IsDeleted && c.IsActive)
                    .Take(5)
                    .Select(c => new { c.Name, c.Model, c.Seats, c.BatteryDuration, c.RentPricePerDay })
                    .ToListAsync();

                var rentalLocations = await _dbContext.RentalLocations
                    .Where(rl => rl.IsActive && !rl.IsDeleted)
                    .Take(3)
                    .Select(rl => new { rl.Name, rl.Address })
                    .ToListAsync();

                // Tạo system prompt với context về EV Rental
                var systemPrompt = $@"Bạn là ChatBot chuyên tư vấn cho dịch vụ EV Rental - dịch vụ thuê xe điện.

THÔNG TIN VỀ DỊCH VỤ EV RENTAL:
- Dịch vụ cho thuê xe điện (Electric Vehicle Rental)
- Cung cấp nhiều loại xe điện với đầy đủ tiện nghi
- Có nhiều địa điểm cho thuê tại các khu vực khác nhau
- Hỗ trợ thuê xe có tài xế hoặc tự lái
- Giá cả hợp lý, dịch vụ chuyên nghiệp

DANH SÁCH XE ĐIỆN:
{System.Text.Json.JsonSerializer.Serialize(cars)}

ĐỊA ĐIỂM CHO THUÊ:
{System.Text.Json.JsonSerializer.Serialize(rentalLocations)}

QUY TẮC TRẢ LỜI:
1. BẮT BUỘC: Mọi câu hỏi của khách hàng, dù về chủ đề gì, bạn PHẢI liên kết và trả lời trong ngữ cảnh của dịch vụ EV Rental
2. Nếu khách hỏi về chủ đề khác (thời tiết, tin tức, v.v.), hãy chuyển hướng sang giới thiệu dịch vụ EV Rental một cách tự nhiên
3. Luôn nhiệt tình, thân thiện và chuyên nghiệp
4. Trả lời ngắn gọn, dễ hiểu, khoảng 100-150 từ
5. Luôn kết thúc bằng cách mời khách hàng sử dụng dịch vụ hoặc đặt câu hỏi thêm về EV Rental

CÂU HỎI CỦA KHÁCH HÀNG: {request.Message}

Hãy trả lời câu hỏi trên theo quy tắc đã nêu.";

                var reply = await _aiService.GenerateResponseAsync(systemPrompt, shortAnswer: request.ShortAnswer);
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
