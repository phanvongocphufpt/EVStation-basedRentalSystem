using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Service.IServices;

namespace Service.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIService> _logger;
        private readonly string _apiKey;
        private readonly string _model;

        public AIService(HttpClient httpClient, ILogger<AIService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // 🔑 Nhập API key của bạn tại đây
            _apiKey = "AIzaSyBls6rTvX65uYqBwMq41S8AfSdTr8d07pk";

            // ⚙️ Dùng model tương thích bản free
            _model = "models/gemini-2.5-flash";

            _httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1/");
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt +"Trả lời ngắn trong 3 câu"}
                            }
                        }
                    }
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{_model}:generateContent?key={_apiKey}",
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
    }
}
