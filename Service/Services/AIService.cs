using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repository.Context;
using Service.IServices;
using Service.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Service.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIService> _logger;
        private readonly string _apiKey;
        private readonly EVSDbContext _dbContext;

        public AIService(HttpClient httpClient, ILogger<AIService> logger, IConfiguration configuration, EVSDbContext dbContext)
        {
            _httpClient = httpClient;
            _logger = logger;
            _dbContext = dbContext;

            _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey missing");
            _httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1/");
        }

        public async Task<string> GenerateResponseAsync(string prompt, string model = "flash", bool shortAnswer = false)
        {
            try
            {
                string modelName = model.ToLower() switch
                {
                    "pro" => "models/gemini-2.5-pro",
                    _ => "models/gemini-2.5-flash"
                };

                if (shortAnswer)
                {
                    prompt = "Trả lời ngắn gọn:khoảng 100 từ " + prompt;
                }

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{modelName}:generateContent?key={_apiKey}",
                    requestBody);

                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("[AI Error] Gemini API Error: {StatusCode} - {Body}", response.StatusCode, responseBody);
                    return $"[AI Error] Gemini API Error: {response.StatusCode} - {responseBody}";
                }

                using var doc = JsonDocument.Parse(responseBody);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? "[AI Error] Empty response from Gemini.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AI Error] Exception during Gemini call");
                return $"[AI Error] {ex.Message}";
            }
        }


        public async Task<string> AnalyzeCarUsageAsync(string model = "flash", bool shortAnswer = false)
        {
            try
            {
                // Lấy dữ liệu RentalOrder
                var orders = await _dbContext.RentalOrders
                    .Include(ro => ro.Car)
                    .ToListAsync();

                if (!orders.Any())
                    return "Chưa có dữ liệu thuê xe để phân tích.";

                // Tính tỷ lệ sử dụng theo xe
                var carUsage = orders
                    .GroupBy(ro => ro.Car.Name)
                    .Select(g => new
                    {
                        CarName = g.Key,
                        UsageCount = g.Count()
                    })
                    .OrderByDescending(x => x.UsageCount)
                    .ToList();

                // Tính giờ cao điểm dựa trên PickupTime
                var peakHours = orders
                    .GroupBy(ro => ro.PickupTime.Hour)
                    .Select(g => new { Hour = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                // Tạo prompt để AI phân tích
                var prompt = "Phân tích tỷ lệ sử dụng xe và giờ cao điểm dựa trên dữ liệu sau:\n\n";
                prompt += "Tỷ lệ sử dụng xe:\n";
                foreach (var c in carUsage)
                {
                    prompt += $"- {c.CarName}: {c.UsageCount} lần\n";
                }
                prompt += "\nGiờ cao điểm:\n";
                foreach (var h in peakHours)
                {
                    prompt += $"- {h.Hour}:00 - {h.Count} đơn\n";
                }

                // Gọi AI để viết phân tích đẹp
                var analysis = await GenerateResponseAsync(prompt, model, shortAnswer);
                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AI Error] Exception during car usage analysis");
                return $"[AI Error] {ex.Message}";
            }
        }

        public async Task<CarUsageAnalysisDTO> AnalyzeCarUsageWithSuggestionsAsync(string model = "flash", bool shortAnswer = false)
        {
            try
            {
                // Lấy dữ liệu RentalOrder
                var orders = await _dbContext.RentalOrders
                    .Include(ro => ro.Car)
                    .ToListAsync();

                if (!orders.Any())
                {
                    return new CarUsageAnalysisDTO
                    {
                        Analysis = "Chưa có dữ liệu thuê xe để phân tích.",
                        Suggestions = "Chưa có dữ liệu để đưa ra gợi ý."
                    };
                }

                // Tính tỷ lệ sử dụng theo xe
                var carUsage = orders
                    .GroupBy(ro => ro.Car.Name)
                    .Select(g => new
                    {
                        CarName = g.Key,
                        UsageCount = g.Count()
                    })
                    .OrderByDescending(x => x.UsageCount)
                    .ToList();

                // Tính giờ cao điểm dựa trên PickupTime
                var peakHours = orders
                    .GroupBy(ro => ro.PickupTime.Hour)
                    .Select(g => new { Hour = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                // Tạo dữ liệu JSON để gửi cho AI
                var usageDataJson = System.Text.Json.JsonSerializer.Serialize(carUsage);
                var peakHoursJson = System.Text.Json.JsonSerializer.Serialize(peakHours);

                // Prompt cho phân tích
                var analysisPrompt = $@"Bạn là chuyên gia phân tích dữ liệu thuê xe.
Dữ liệu hiện tại:

Tỷ lệ sử dụng xe:
{usageDataJson}

Giờ cao điểm thuê xe:
{peakHoursJson}

Hãy viết phân tích rõ ràng, dễ đọc về tỷ lệ sử dụng xe và giờ cao điểm.
Mỗi ý 1 dòng, tối đa 500 ký tự.
Không dùng *, ** hay markdown.
Chỉ phân tích dữ liệu, không đưa ra gợi ý.";

                // Prompt cho gợi ý
                var suggestionPrompt = $@"Bạn là chuyên gia tư vấn kinh doanh cho dịch vụ thuê xe điện.
Dựa trên dữ liệu sau:

Tỷ lệ sử dụng xe:
{usageDataJson}

Giờ cao điểm thuê xe:
{peakHoursJson}

Hãy đưa ra các gợi ý cụ thể để cải thiện hiệu quả kinh doanh, tối ưu hóa việc sử dụng xe, và tăng doanh thu.
Mỗi ý 1 dòng, tối đa 500 ký tự.
Không dùng *, ** hay markdown.
Tập trung vào gợi ý hành động cụ thể.";

                // Gọi AI để tạo phân tích và gợi ý song song
                var analysisTask = GenerateResponseAsync(analysisPrompt, model, shortAnswer);
                var suggestionTask = GenerateResponseAsync(suggestionPrompt, model, shortAnswer);

                await Task.WhenAll(analysisTask, suggestionTask);

                return new CarUsageAnalysisDTO
                {
                    Analysis = await analysisTask,
                    Suggestions = await suggestionTask
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AI Error] Exception during car usage analysis with suggestions");
                return new CarUsageAnalysisDTO
                {
                    Analysis = $"[AI Error] {ex.Message}",
                    Suggestions = $"[AI Error] {ex.Message}"
                };
            }
        }
    }
}
