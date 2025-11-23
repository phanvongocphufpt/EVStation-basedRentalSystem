using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Service.Configurations;

namespace Service.Common.Momo
{
    public class MomoHelper
    {
        private readonly MomoSettings _settings;

        public MomoHelper(MomoSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Tạo signature cho MoMo payment request
        /// Theo format: accessKey=xxx&amount=xxx&extraData=xxx&ipnUrl=xxx&orderId=xxx&orderInfo=xxx&partnerCode=xxx&redirectUrl=xxx&requestId=xxx&requestType=xxx
        /// </summary>
        public string CreateSignature(Dictionary<string, string> parameters)
        {
            // Tạo dictionary mới bao gồm accessKey
            var allParams = new Dictionary<string, string>(parameters)
            {
                { "accessKey", _settings.AccessKey }
            };
            
            // Sắp xếp parameters theo key (alphabetical order)
            var sortedParams = new SortedDictionary<string, string>(allParams);
            var queryString = new StringBuilder();
            
            bool isFirst = true;
            foreach (var param in sortedParams)
            {
                if (!string.IsNullOrEmpty(param.Value))
                {
                    if (!isFirst)
                    {
                        queryString.Append("&");
                    }
                    queryString.Append($"{param.Key}={param.Value}");
                    isFirst = false;
                }
            }
            
            // Tạo HMAC SHA256 với SecretKey
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.SecretKey)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString.ToString()));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Tạo payment request và gửi đến MoMo để lấy payUrl
        /// </summary>
        public async Task<MomoPaymentResponse> CreatePaymentRequestAsync(
            string orderId,
            string requestId,
            long amount,
            string orderInfo,
            string extraData = "")
        {
            // Tạo rawData cho signature (chỉ các field cần thiết cho signature)
            var signatureData = new Dictionary<string, string>
            {
                { "partnerCode", _settings.PartnerCode },
                { "requestId", requestId },
                { "amount", amount.ToString() },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", _settings.RedirectUrl },
                { "ipnUrl", _settings.IpnUrl },
                { "extraData", extraData },
                { "requestType", "captureWallet" }
            };

            // Tạo signature
            var signature = CreateSignature(signatureData);
            
            // Tạo request body (bao gồm các field bổ sung)
            var rawData = new Dictionary<string, string>(signatureData)
            {
                { "partnerName", "EV Station Rental" },
                { "storeId", "EVStation" },
                { "lang", "vi" },
                { "autoCapture", "true" },
                { "signature", signature }
            };

            // Gửi request đến MoMo
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                
                // MoMo API yêu cầu format JSON với property names như trong rawData
                var jsonContent = JsonSerializer.Serialize(rawData);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                try
                {
                    var response = await httpClient.PostAsync(_settings.Endpoint, content);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse response từ MoMo (có thể có format khác)
                        var jsonDoc = JsonDocument.Parse(responseContent);
                        var root = jsonDoc.RootElement;
                        
                        var result = new MomoPaymentResponse
                        {
                            ResultCode = root.TryGetProperty("resultCode", out var resultCodeEl) ? resultCodeEl.GetInt32() : -1,
                            Message = root.TryGetProperty("message", out var messageEl) ? messageEl.GetString() ?? "" : "",
                            PayUrl = root.TryGetProperty("payUrl", out var payUrlEl) ? payUrlEl.GetString() ?? "" : "",
                            OrderId = root.TryGetProperty("orderId", out var orderIdEl) ? orderIdEl.GetString() ?? "" : "",
                            RequestId = root.TryGetProperty("requestId", out var requestIdEl) ? requestIdEl.GetString() ?? "" : "",
                            Amount = root.TryGetProperty("amount", out var amountEl) ? amountEl.GetInt64() : 0,
                            Signature = root.TryGetProperty("signature", out var signatureEl) ? signatureEl.GetString() ?? "" : ""
                        };
                        
                        return result;
                    }
                    else
                    {
                        return new MomoPaymentResponse
                        {
                            ResultCode = (int)response.StatusCode,
                            Message = $"HTTP Error: {response.StatusCode} - {responseContent}"
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new MomoPaymentResponse
                    {
                        ResultCode = -1,
                        Message = $"Exception: {ex.Message}"
                    };
                }
            }
        }

        /// <summary>
        /// Verify signature từ MoMo IPN callback
        /// </summary>
        public bool VerifySignature(Dictionary<string, string> parameters, string signature)
        {
            var calculatedSignature = CreateSignature(parameters);
            return calculatedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class MomoPaymentResponse
    {
        public int ResultCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string PayUrl { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string Signature { get; set; } = string.Empty;
    }
}

