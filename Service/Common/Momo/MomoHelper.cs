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
                        // ✅ DEBUG: Log raw response từ MoMo
                        System.Diagnostics.Debug.WriteLine("========== MoMo CreatePayment Response ==========");
                        System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                        System.Diagnostics.Debug.WriteLine($"Response Content: {responseContent}");
                        
                        // Parse response từ MoMo
                        var jsonDoc = JsonDocument.Parse(responseContent);
                        var root = jsonDoc.RootElement;
                        
                        // ✅ Parse tất cả các field theo tài liệu MoMo
                        var result = new MomoPaymentResponse
                        {
                            PartnerCode = root.TryGetProperty("partnerCode", out var partnerCodeEl) 
                                ? partnerCodeEl.GetString() ?? "" 
                                : "",
                            RequestId = root.TryGetProperty("requestId", out var requestIdEl) 
                                ? requestIdEl.GetString() ?? "" 
                                : "",
                            OrderId = root.TryGetProperty("orderId", out var orderIdEl) 
                                ? orderIdEl.GetString() ?? "" 
                                : "",
                            Amount = root.TryGetProperty("amount", out var amountEl) 
                                ? (amountEl.ValueKind == System.Text.Json.JsonValueKind.String 
                                    ? long.Parse(amountEl.GetString() ?? "0") 
                                    : amountEl.GetInt64())
                                : 0,
                            ResponseTime = root.TryGetProperty("responseTime", out var responseTimeEl) 
                                ? responseTimeEl.GetInt64() 
                                : 0,
                            Message = root.TryGetProperty("message", out var messageEl) 
                                ? messageEl.GetString() ?? "" 
                                : "",
                            ResultCode = root.TryGetProperty("resultCode", out var resultCodeEl) 
                                ? resultCodeEl.GetInt32() 
                                : -1,
                            PayUrl = root.TryGetProperty("payUrl", out var payUrlEl) 
                                ? payUrlEl.GetString() ?? "" 
                                : "",
                            Deeplink = root.TryGetProperty("deeplink", out var deeplinkEl) 
                                ? deeplinkEl.GetString() ?? "" 
                                : "",
                            QrCodeUrl = root.TryGetProperty("qrCodeUrl", out var qrCodeUrlEl) 
                                ? qrCodeUrlEl.GetString() ?? "" 
                                : "",
                            Signature = root.TryGetProperty("signature", out var signatureEl) 
                                ? signatureEl.GetString() ?? "" 
                                : ""
                        };
                        
                        // ✅ DEBUG: Log parsed response
                        System.Diagnostics.Debug.WriteLine("Parsed Response:");
                        System.Diagnostics.Debug.WriteLine($"  - PartnerCode: {result.PartnerCode}");
                        System.Diagnostics.Debug.WriteLine($"  - RequestId: {result.RequestId}");
                        System.Diagnostics.Debug.WriteLine($"  - OrderId: {result.OrderId}");
                        System.Diagnostics.Debug.WriteLine($"  - Amount: {result.Amount}");
                        System.Diagnostics.Debug.WriteLine($"  - ResponseTime: {result.ResponseTime}");
                        System.Diagnostics.Debug.WriteLine($"  - Message: {result.Message}");
                        System.Diagnostics.Debug.WriteLine($"  - ResultCode: {result.ResultCode}");
                        System.Diagnostics.Debug.WriteLine($"  - PayUrl: {result.PayUrl}");
                        System.Diagnostics.Debug.WriteLine($"  - Deeplink: {result.Deeplink}");
                        System.Diagnostics.Debug.WriteLine($"  - QrCodeUrl: {result.QrCodeUrl}");
                        System.Diagnostics.Debug.WriteLine($"  - Signature: {result.Signature}");
                        System.Diagnostics.Debug.WriteLine("=============================================");
                        
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
        /// Theo tài liệu MoMo: signature được tạo từ các field được sort theo thứ tự a-z:
        /// accessKey=$accessKey&amount=$amount&extraData=$extraData&message=$message&orderId=$orderId
        /// &orderInfo=$orderInfo&orderType=$orderType&partnerCode=$partnerCode&payType=$payType
        /// &requestId=$requestId&responseTime=$responseTime&resultCode=$resultCode&transId=$transId
        /// </summary>
        public bool VerifySignature(Dictionary<string, string> parameters, string signature)
        {
            try
            {
                // Tạo dictionary mới bao gồm accessKey
                var allParams = new Dictionary<string, string>(parameters)
                {
                    { "accessKey", _settings.AccessKey }
                };

                // Sắp xếp parameters theo key (alphabetical order) như tài liệu MoMo yêu cầu
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
                    var calculatedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    return calculatedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Response từ MoMo API khi tạo payment request
    /// Theo tài liệu MoMo: https://developers.momo.vn/v3/docs/payment/api/wallet-online/php-sdk
    /// </summary>
    public class MomoPaymentResponse
    {
        /// <summary>
        /// Thông tin tích hợp (partnerCode)
        /// </summary>
        public string PartnerCode { get; set; } = string.Empty;

        /// <summary>
        /// Giống với yêu cầu ban đầu (requestId)
        /// </summary>
        public string RequestId { get; set; } = string.Empty;

        /// <summary>
        /// Mã đơn hàng của đối tác (orderId)
        /// </summary>
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// Giống với số tiền yêu cầu ban đầu (amount)
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// Thời gian trả kết quả thanh toán về đối tác (responseTime) - Định dạng: timestamp
        /// </summary>
        public long ResponseTime { get; set; }

        /// <summary>
        /// Mô tả lỗi, ngôn ngữ dựa trên lang (message)
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Result Code (resultCode)
        /// 0 = Thành công
        /// Khác 0 = Thất bại
        /// </summary>
        public int ResultCode { get; set; }

        /// <summary>
        /// URL để chuyển từ trang mua hàng của đối tác sang trang thanh toán của MoMo (payUrl)
        /// </summary>
        public string PayUrl { get; set; } = string.Empty;

        /// <summary>
        /// Deep link để mở app MoMo (deeplink) - nếu có
        /// </summary>
        public string Deeplink { get; set; } = string.Empty;

        /// <summary>
        /// QR Code URL để quét thanh toán (qrCodeUrl) - nếu có
        /// </summary>
        public string QrCodeUrl { get; set; } = string.Empty;

        /// <summary>
        /// Signature từ MoMo (nếu có)
        /// </summary>
        public string Signature { get; set; } = string.Empty;
    }
}

