using Microsoft.Extensions.Configuration;
using Service.Common.Momo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Common.Momo.MomoServices
{
    public class MoMoService : IMoMoService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public MoMoService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<(string PaymentUrl, string RequestId)> CreatePaymentUrlAsync(
            string orderId,
            string orderInfo,
            long amount,
            string extraData = "")
        {
            var partnerCode = _configuration["MomoSettings:PartnerCode"];
            var accessKey = _configuration["MomoSettings:AccessKey"];
            var secretKey = _configuration["MomoSettings:SecretKey"];
            var endpoint = _configuration["MomoSettings:Endpoint"];
            var returnUrl = _configuration["MomoSettings:ReturnUrl"];
            var ipnUrl = _configuration["MomoSettings:IpnUrl"];

            var requestId = Guid.NewGuid().ToString();
            var requestType = "captureWallet";

            var rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType={requestType}";

            var signature = ComputeHmacSha256(rawHash, secretKey);

            var request = new MomoPaymentRequest
            {
                PartnerCode = partnerCode,
                PartnerName = "EV Station Rental",
                StoreId = partnerCode,
                RequestId = requestId,
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo,
                RedirectUrl = returnUrl,
                IpnUrl = ipnUrl,
                RequestType = requestType,
                ExtraData = extraData,
                Signature = signature,
                Lang = "vi"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            var json = JsonSerializer.Serialize(request, jsonOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var momoResponse = JsonSerializer.Deserialize<MomoPaymentResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (momoResponse != null && momoResponse.ResultCode == 0)
            {
                return (momoResponse.PayUrl ?? momoResponse.DeepLink ?? momoResponse.QrCodeUrl ?? "", requestId);
            }

            throw new Exception($"MoMo payment creation failed: {momoResponse?.Message ?? "Unknown error"}");
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}

