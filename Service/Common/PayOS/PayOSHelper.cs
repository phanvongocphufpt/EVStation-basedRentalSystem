using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Service.Configurations;

namespace Service.Common.PayOS
{
    public class PayOSHelper
    {
        private readonly PayOSSettings _settings;
        private readonly HttpClient _httpClient;

        public PayOSHelper(PayOSSettings settings)
        {
            _settings = settings;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            // PayOS authentication - custom headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _settings.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _settings.ClientId);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            
            System.Diagnostics.Debug.WriteLine("=== PayOS Client Initialized ===");
            System.Diagnostics.Debug.WriteLine($"ClientId: {_settings.ClientId}");
            System.Diagnostics.Debug.WriteLine($"ApiKey: {_settings.ApiKey}");
            System.Diagnostics.Debug.WriteLine($"Endpoint: {_settings.Endpoint}");
            System.Diagnostics.Debug.WriteLine("================================");
        }

        /// <summary>
        /// Tạo signature/checksum cho PayOS theo đúng tài liệu chính thức
        /// Format: amount=$amount&cancelUrl=$cancelUrl&description=$description&orderCode=$orderCode&returnUrl=$returnUrl
        /// KHÔNG có items và KHÔNG có key trong raw string
        /// Sau đó dùng HMAC_SHA256 với checksumKey để hash
        /// </summary>
        private string CreateSignature(int orderCode, long amount, string description, string returnUrl, string cancelUrl)
        {
            // Theo tài liệu PayOS: signature chỉ gồm 5 field theo thứ tự alphabetical
            // amount, cancelUrl, description, orderCode, returnUrl
            // KHÔNG có items và KHÔNG có key trong raw string
            var rawString = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";

            System.Diagnostics.Debug.WriteLine("=== PayOS Signature Calculation (Official Format) ===");
            System.Diagnostics.Debug.WriteLine($"Raw String (NO items, NO key): {rawString}");
            System.Diagnostics.Debug.WriteLine($"ChecksumKey: {_settings.ChecksumKey}");

            // Tạo HMAC SHA256 với checksumKey
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.ChecksumKey));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawString));
            string signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            System.Diagnostics.Debug.WriteLine($"Generated Signature: {signature}");
            System.Diagnostics.Debug.WriteLine("=====================================================");

            return signature;
        }

        /// <summary>
        /// Tạo payment link từ PayOS - Cách tiếp cận mới hoàn toàn
        /// </summary>
        public async Task<PayOSPaymentResponse> CreatePaymentLinkAsync(
            int orderCode,
            long amount,
            string description,
            string returnUrl,
            string cancelUrl)
        {
            try
            {
                // Tạo items array đơn giản
                var items = new[]
                {
                    new { name = description, quantity = 1, price = amount }
                };

                // Tạo signature theo đúng tài liệu PayOS (KHÔNG có items trong signature)
                string signature = CreateSignature(orderCode, amount, description, returnUrl, cancelUrl);

                // Tạo request object
                var requestObj = new
                {
                    orderCode,
                    amount,
                    description,
                    returnUrl,
                    cancelUrl,
                    items,
                    signature
                };

                // Serialize to JSON
                string jsonBody = JsonSerializer.Serialize(requestObj, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });

                System.Diagnostics.Debug.WriteLine("=== PayOS Request (New Method) ===");
                System.Diagnostics.Debug.WriteLine($"URL: {_settings.Endpoint}");
                System.Diagnostics.Debug.WriteLine($"Body: {jsonBody}");
                System.Diagnostics.Debug.WriteLine("==================================");

                // Gửi request
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_settings.Endpoint, content);
                string responseJson = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine("=== PayOS Response ===");
                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Body: {responseJson}");
                System.Diagnostics.Debug.WriteLine("======================");

                // Parse response
                var doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;

                int code = -1;
                if (root.TryGetProperty("code", out var codeEl))
                {
                    if (codeEl.ValueKind == JsonValueKind.Number)
                        code = codeEl.GetInt32();
                    else if (codeEl.ValueKind == JsonValueKind.String)
                        int.TryParse(codeEl.GetString(), out code);
                }

                string desc = root.TryGetProperty("desc", out var descEl) ? descEl.GetString() ?? "" : "";

                string checkoutUrl = "";
                string qrCode = "";
                JsonElement? dataEl = null;

                if (root.TryGetProperty("data", out var data) && data.ValueKind != JsonValueKind.Null)
                {
                    dataEl = data;
                    if (data.TryGetProperty("checkoutUrl", out var urlEl) && urlEl.ValueKind == JsonValueKind.String)
                        checkoutUrl = urlEl.GetString() ?? "";
                    if (data.TryGetProperty("qrCode", out var qrEl) && qrEl.ValueKind == JsonValueKind.String)
                        qrCode = qrEl.GetString() ?? "";
                }

                return new PayOSPaymentResponse
                {
                    Code = code,
                    Desc = desc,
                    CheckoutUrl = checkoutUrl,
                    QrCode = qrCode,
                    Data = dataEl ?? default
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PayOS Exception: {ex.GetType().Name} - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");

                return new PayOSPaymentResponse
                {
                    Code = -1,
                    Desc = $"Exception: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Verify signature từ PayOS callback
        /// </summary>
        public bool VerifySignature(Dictionary<string, object> data, string signature)
        {
            try
            {
                // Extract values từ dictionary (chỉ các field cần thiết cho signature)
                int orderCode = data.ContainsKey("orderCode") ? Convert.ToInt32(data["orderCode"]) : 0;
                long amount = data.ContainsKey("amount") ? Convert.ToInt64(data["amount"]) : 0;
                string description = data.ContainsKey("description") ? data["description"].ToString() ?? "" : "";
                string returnUrl = data.ContainsKey("returnUrl") ? data["returnUrl"].ToString() ?? "" : "";
                string cancelUrl = data.ContainsKey("cancelUrl") ? data["cancelUrl"].ToString() ?? "" : "";

                // Verify signature (KHÔNG có items)
                string calculated = CreateSignature(orderCode, amount, description, returnUrl, cancelUrl);
                return calculated.Equals(signature, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }

    public class PayOSPaymentResponse
    {
        public int Code { get; set; }
        public string Desc { get; set; } = "";
        public JsonElement? Data { get; set; }
        public string CheckoutUrl { get; set; } = "";
        public string QrCode { get; set; } = "";
    }
}
