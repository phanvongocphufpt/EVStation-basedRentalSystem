using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Entities.Enum;
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
        /// Phân tích dữ liệu tổng thể: xe, phản hồi, đơn hàng, thanh toán, địa điểm
        /// </summary>
        [HttpGet("analyze")]
        public async Task<IActionResult> AnalyzeData()
        {
            try
            {
                // Thống kê tổng quan
                var totalCars = await _dbContext.Cars.CountAsync(c => !c.IsDeleted && c.IsActive);
                var totalOrders = await _dbContext.RentalOrders.CountAsync();
                var totalUsers = await _dbContext.Users.CountAsync(u => u.Role == "Customer" && u.IsActive);
                var totalFeedbacks = await _dbContext.Feedbacks.CountAsync(f => !f.IsDeleted);
                
                // Thống kê xe
                var carStats = await _dbContext.Cars
                    .Where(c => !c.IsDeleted && c.IsActive)
                    .GroupBy(c => c.SizeType)
                    .Select(g => new { 
                        SizeType = g.Key, 
                        Count = g.Count(),
                        AvgPrice = g.Average(c => c.RentPricePerDay),
                        AvgBattery = g.Average(c => c.BatteryDuration)
                    })
                    .ToListAsync();

                var topCars = await _dbContext.Cars
                    .Where(c => !c.IsDeleted && c.IsActive)
                    .OrderByDescending(c => c.RentPricePerDay)
                    .Take(5)
                    .Select(c => new { 
                        c.Name, 
                        c.Model, 
                        c.Seats, 
                        c.BatteryDuration, 
                        c.RentPricePerDay,
                        c.SizeType,
                        c.BatteryType
                    })
                    .ToListAsync();

                // Thống kê đơn hàng
                var orderStats = await _dbContext.RentalOrders
                    .GroupBy(o => o.Status)
                    .Select(g => new { 
                        Status = g.Key.ToString(), 
                        Count = g.Count(),
                        TotalRevenue = g.Sum(o => o.Total ?? 0)
                    })
                    .ToListAsync();

                var orderWithDriverStats = await _dbContext.RentalOrders
                    .GroupBy(o => o.WithDriver)
                    .Select(g => new { 
                        WithDriver = g.Key, 
                        Count = g.Count(),
                        Percentage = (double)g.Count() / totalOrders * 100
                    })
                    .ToListAsync();

                var recentOrders = await _dbContext.RentalOrders
                    .Include(o => o.Car)
                    .Include(o => o.RentalLocation)
                    .OrderByDescending(r => r.OrderDate)
                    .Take(10)
                    .Select(r => new { 
                        r.Id, 
                        r.OrderDate,
                        r.PickupTime,
                        r.ExpectedReturnTime,
                        r.ActualReturnTime,
                        r.WithDriver, 
                        r.Status,
                        r.Total,
                        r.SubTotal,
                        CarName = r.Car.Name,
                        LocationName = r.RentalLocation.Name
                    })
                    .ToListAsync();

                // Thống kê phản hồi
                var feedbackStats = await _dbContext.Feedbacks
                    .Where(f => !f.IsDeleted)
                    .GroupBy(f => f.Rating)
                    .Select(g => new { 
                        Rating = g.Key, 
                        Count = g.Count()
                    })
                    .OrderBy(f => f.Rating)
                    .ToListAsync();

                var avgRating = await _dbContext.Feedbacks
                    .Where(f => !f.IsDeleted)
                    .AverageAsync(f => (double?)f.Rating) ?? 0;

                var recentFeedbacks = await _dbContext.Feedbacks
                    .Where(f => !f.IsDeleted)
                    .Include(f => f.User)
                    .OrderByDescending(f => f.CreatedAt)
                    .Take(10)
                    .Select(f => new { 
                        f.Title, 
                        f.Content,
                        f.Rating,
                        f.CreatedAt,
                        UserName = f.User.FullName
                    })
                    .ToListAsync();

                // Thống kê thanh toán
                var paymentStats = await _dbContext.Payments
                    .Where(p => p.Status == PaymentStatus.Completed)
                    .GroupBy(p => p.PaymentMethod)
                    .Select(g => new { 
                        PaymentMethod = g.Key ?? "Unknown", 
                        Count = g.Count(),
                        TotalAmount = g.Sum(p => p.Amount)
                    })
                    .ToListAsync();

                var totalRevenue = await _dbContext.Payments
                    .Where(p => p.Status == PaymentStatus.Completed)
                    .SumAsync(p => (double?)p.Amount) ?? 0;

                // Thống kê địa điểm
                var locationStats = await _dbContext.RentalLocations
                    .Where(rl => rl.IsActive && !rl.IsDeleted)
                    .Select(rl => new {
                        rl.Name,
                        rl.Address,
                        CarCount = rl.Cars.Count(c => !c.IsDeleted && c.IsActive),
                        OrderCount = rl.RentalOrders.Count
                    })
                    .ToListAsync();

                // Tạo prompt phân tích
                var prompt = $@"
Bạn là chuyên gia phân tích dữ liệu và tư vấn cho dịch vụ thuê xe điện (EV Rental).

THỐNG KÊ TỔNG QUAN:
- Tổng số xe: {totalCars}
- Tổng số đơn hàng: {totalOrders}
- Tổng số khách hàng: {totalUsers}
- Tổng số phản hồi: {totalFeedbacks}
- Tổng doanh thu: {totalRevenue:N0} VNĐ
- Đánh giá trung bình: {avgRating:F1}/5

THỐNG KÊ XE:
Phân loại theo kích thước: {System.Text.Json.JsonSerializer.Serialize(carStats)}
Top 5 xe giá cao nhất: {System.Text.Json.JsonSerializer.Serialize(topCars)}

THỐNG KÊ ĐƠN HÀNG:
Theo trạng thái: {System.Text.Json.JsonSerializer.Serialize(orderStats)}
Thuê có/không tài xế: {System.Text.Json.JsonSerializer.Serialize(orderWithDriverStats)}
10 đơn hàng gần nhất: {System.Text.Json.JsonSerializer.Serialize(recentOrders)}

THỐNG KÊ PHẢN HỒI:
Theo điểm đánh giá: {System.Text.Json.JsonSerializer.Serialize(feedbackStats)}
10 phản hồi gần nhất: {System.Text.Json.JsonSerializer.Serialize(recentFeedbacks)}

THỐNG KÊ THANH TOÁN:
Theo phương thức: {System.Text.Json.JsonSerializer.Serialize(paymentStats)}

THỐNG KÊ ĐỊA ĐIỂM:
{System.Text.Json.JsonSerializer.Serialize(locationStats)}

NHIỆM VỤ:
Dựa trên dữ liệu trên, hãy phân tích và đưa ra:
1. Điểm mạnh của dịch vụ hiện tại
2. Điểm yếu cần cải thiện
3. Gợi ý nâng cấp cụ thể cho từng khía cạnh (xe, dịch vụ, địa điểm, thanh toán)
4. Dự đoán xu hướng và cơ hội phát triển

YÊU CẦU:
- Trả lời ngắn gọn, tối đa 1500 ký tự
- Mỗi ý một dòng, không dùng markdown (*, **, #)
- Tập trung vào các gợi ý thực tế và khả thi
- Ưu tiên các cải thiện có tác động cao
";

                var aiResponse = await _aiService.GenerateResponseAsync(prompt, shortAnswer: true);

                // Build structured response
                var response = new AnalysisResponseDTO
                {
                    AiAnalysis = aiResponse,
                    Summary = new SummaryDTO
                    {
                        TotalCars = totalCars,
                        TotalOrders = totalOrders,
                        TotalUsers = totalUsers,
                        TotalFeedbacks = totalFeedbacks,
                        TotalRevenue = totalRevenue,
                        AvgRating = avgRating
                    },
                    CarStatistics = new CarStatisticsDTO
                    {
                        BySizeType = carStats.Select(c => new CarSizeTypeStatDTO
                        {
                            SizeType = c.SizeType ?? "Unknown",
                            Count = c.Count,
                            AvgPrice = c.AvgPrice,
                            AvgBattery = c.AvgBattery
                        }).ToList(),
                        TopCars = topCars.Select(c => new TopCarDTO
                        {
                            Name = c.Name ?? "Unknown",
                            Model = c.Model ?? "Unknown",
                            Seats = c.Seats,
                            BatteryDuration = c.BatteryDuration,
                            RentPricePerDay = c.RentPricePerDay,
                            SizeType = c.SizeType ?? "Unknown",
                            BatteryType = c.BatteryType ?? "Unknown"
                        }).ToList()
                    },
                    OrderStatistics = new OrderStatisticsDTO
                    {
                        ByStatus = orderStats.Select(o => new OrderStatusStatDTO
                        {
                            Status = o.Status,
                            Count = o.Count,
                            TotalRevenue = o.TotalRevenue
                        }).ToList(),
                        DriverOption = new DriverOptionStatDTO
                        {
                            WithDriverCount = orderWithDriverStats.FirstOrDefault(o => o.WithDriver)?.Count ?? 0,
                            WithoutDriverCount = orderWithDriverStats.FirstOrDefault(o => !o.WithDriver)?.Count ?? 0,
                            WithDriverPercentage = orderWithDriverStats.FirstOrDefault(o => o.WithDriver)?.Percentage ?? 0,
                            WithoutDriverPercentage = orderWithDriverStats.FirstOrDefault(o => !o.WithDriver)?.Percentage ?? 0
                        },
                        RecentOrders = recentOrders.Select(o => new RecentOrderDTO
                        {
                            Id = o.Id,
                            OrderDate = o.OrderDate,
                            PickupTime = o.PickupTime,
                            ExpectedReturnTime = o.ExpectedReturnTime,
                            ActualReturnTime = o.ActualReturnTime,
                            WithDriver = o.WithDriver,
                            Status = o.Status.ToString(),
                            Total = o.Total,
                            SubTotal = o.SubTotal,
                            CarName = o.CarName ?? "Unknown",
                            LocationName = o.LocationName ?? "Unknown"
                        }).ToList()
                    },
                    FeedbackStatistics = new FeedbackStatisticsDTO
                    {
                        ByRating = feedbackStats.Select(f => new RatingStatDTO
                        {
                            Rating = f.Rating,
                            Count = f.Count
                        }).ToList(),
                        RecentFeedbacks = recentFeedbacks.Select(f => new RecentFeedbackDTO
                        {
                            Title = f.Title ?? "",
                            Content = f.Content ?? "",
                            Rating = f.Rating,
                            CreatedAt = f.CreatedAt,
                            UserName = f.UserName ?? "Unknown"
                        }).ToList()
                    },
                    PaymentStatistics = new PaymentStatisticsDTO
                    {
                        ByMethod = paymentStats.Select(p => new PaymentMethodStatDTO
                        {
                            PaymentMethod = p.PaymentMethod ?? "Unknown",
                            Count = p.Count,
                            TotalAmount = p.TotalAmount
                        }).ToList()
                    },
                    LocationStatistics = new LocationStatisticsDTO
                    {
                        Locations = locationStats.Select(l => new LocationStatDTO
                        {
                            Name = l.Name ?? "Unknown",
                            Address = l.Address ?? "Unknown",
                            CarCount = l.CarCount,
                            OrderCount = l.OrderCount
                        }).ToList()
                    }
                };

                return Ok(response);
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
