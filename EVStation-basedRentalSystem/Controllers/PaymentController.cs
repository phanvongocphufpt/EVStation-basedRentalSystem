using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Service.DTOs;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllPayment()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments);
        }

        [HttpGet("byRentalLocation")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetRevenueByLocation()
        {
            var result = await _paymentService.GetRevenueByLocationAsync();
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpGet("GetAllByUserId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetAllCustomerPayment(int userId)
        {
            var payments = await _paymentService.GetAllByUserIdAsync(userId);
            return Ok(payments);
        }

        [HttpGet("GetByPaymentId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (!payment.IsSuccess)
                return NotFound(payment.Message);

            return Ok(payment.Data);
        }

        [HttpPost("CreateFromOrder")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            var result = await _paymentService.AddAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpPut("UpdatePaymentStatus")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update([FromBody] UpdatePaymentStatusDTO updatePaymentStatusDTO)
        {
            if (updatePaymentStatusDTO == null)
                return BadRequest("Invalid data.");

            var result = await _paymentService.UpdatePaymentStatusAsync(updatePaymentStatusDTO);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpPut("ConfirmDepositPayment")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ConfirmDepositPayment(int orderId)
        {
            var result = await _paymentService.ConfirmDepositPaymentAsync(orderId);
            return Ok(result);
        }

        // ============================
        // MoMo endpoints
        // ============================

        // 1. Tạo MoMo payment → trả về payUrl để redirect frontend
        [HttpPost("CreateMomoPayment")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> CreateMomoPayment([FromQuery] int rentalOrderId, [FromQuery] int userId, [FromQuery] double amount)
        {
            var result = await _paymentService.CreateMomoPaymentAsync(rentalOrderId, userId, amount);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        // 2. Nhận IPN từ MoMo
        [HttpPost("MomoIPN")]
        [AllowAnonymous] // IPN gọi từ MoMo, không cần auth
        public async Task<IActionResult> MomoIpn()
        {
            try
            {
                // ✅ DEBUG: Log request info
                System.Diagnostics.Debug.WriteLine("========== MoMo IPN Controller ==========");
                System.Diagnostics.Debug.WriteLine($"Request Method: {Request.Method}");
                System.Diagnostics.Debug.WriteLine($"Request Path: {Request.Path}");
                System.Diagnostics.Debug.WriteLine($"Content-Type: {Request.ContentType}");
                
                // ✅ Đọc raw body và parse thủ công để tránh lỗi với System.Text.Json
                string rawBody;
                using (var reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8, leaveOpen: true))
                {
                    rawBody = await reader.ReadToEndAsync();
                    Request.Body.Position = 0; // Reset stream position
                }
                
                System.Diagnostics.Debug.WriteLine($"Raw Body: {rawBody}");
                
                if (string.IsNullOrWhiteSpace(rawBody))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Payload is null or empty!");
                    // Log và trả về 200 để MoMo không retry
                    return Ok(new { success = false, message = "Payload null or empty" });
                }

                // Parse JSON string thành JObject
                JObject payload;
                try
                {
                    payload = JObject.Parse(rawBody);
                }
                catch (Exception parseEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Failed to parse JSON: {parseEx.Message}");
                    return Ok(new { success = false, message = $"Invalid JSON format: {parseEx.Message}" });
                }

                System.Diagnostics.Debug.WriteLine($"Parsed Payload: {payload?.ToString() ?? "null"}");

                if (payload == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Parsed payload is null!");
                    return Ok(new { success = false, message = "Failed to parse payload" });
                }

                var result = await _paymentService.ProcessMomoIpnAsync(payload);
                
                // ✅ QUAN TRỌNG: Luôn trả về 200 OK để MoMo không retry
                // Log lỗi nhưng vẫn trả về 200 để tránh MoMo gửi lại callback
                if (!result.IsSuccess)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ ProcessMomoIpnAsync failed: {result.Message}");
                    // Có thể log vào database hoặc file log ở đây
                    return Ok(new { success = false, message = result.Message });
                }

                System.Diagnostics.Debug.WriteLine("✅ MoMo IPN processed successfully!");
                System.Diagnostics.Debug.WriteLine("========================================");
                return Ok(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                // ✅ DEBUG: Log exception chi tiết
                System.Diagnostics.Debug.WriteLine($"❌ Exception in MomoIpn: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                // Log exception và trả về 200 để MoMo không retry
                // _logger.LogError(ex, "Lỗi không mong đợi khi xử lý MoMo IPN");
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // 3. Lấy payment theo MomoOrderId
        [HttpGet("GetByMomoOrderId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByMomoOrderId([FromQuery] string momoOrderId)
        {
            var result = await _paymentService.GetPaymentByMomoOrderIdAsync(momoOrderId);
            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }
        // ========================
        // PayOS Endpoints
        // ========================

        // 1. Tạo PayOS payment
        [HttpPost("CreatePayOSPayment")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> CreatePayOSPayment([FromQuery] int rentalOrderId, [FromQuery] int userId, [FromQuery] double amount)
        {
            var result = await _paymentService.CreatePayOSPaymentAsync(rentalOrderId, userId, amount);

            if (!result.IsSuccess)
            {
                // Optional: log thất bại nếu cần
                // _logger.LogWarning("Tạo PayOS payment thất bại: {Message}", result.Message);
                return BadRequest(new { success = false, message = result.Message });
            }

            // Trả về object chuẩn
            return Ok(new { success = true, data = result.Data });
        }

        // 2. Nhận IPN từ PayOS
        [HttpPost("PayOSIPN")]
        [AllowAnonymous] // IPN gọi từ PayOS, không cần auth
        public async Task<IActionResult> PayOSIpn()
        {
            try
            {
                // ✅ DEBUG: Log request info
                System.Diagnostics.Debug.WriteLine("========== PayOS IPN Controller ==========");
                System.Diagnostics.Debug.WriteLine($"Request Method: {Request.Method}");
                System.Diagnostics.Debug.WriteLine($"Request Path: {Request.Path}");
                System.Diagnostics.Debug.WriteLine($"Content-Type: {Request.ContentType}");
                
                // ✅ Đọc raw body và parse thủ công để tránh lỗi với System.Text.Json
                string rawBody;
                using (var reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8, leaveOpen: true))
                {
                    rawBody = await reader.ReadToEndAsync();
                    Request.Body.Position = 0; // Reset stream position
                }
                
                System.Diagnostics.Debug.WriteLine($"Raw Body: {rawBody}");
                
                if (string.IsNullOrWhiteSpace(rawBody))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Payload is null or empty!");
                    // Log và trả về 200 để PayOS không retry
                    return Ok(new { success = false, message = "Payload null or empty" });
                }

                // Parse JSON string thành JObject
                JObject payload;
                try
                {
                    payload = JObject.Parse(rawBody);
                }
                catch (Exception parseEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Failed to parse JSON: {parseEx.Message}");
                    return Ok(new { success = false, message = $"Invalid JSON format: {parseEx.Message}" });
                }

                System.Diagnostics.Debug.WriteLine($"Parsed Payload: {payload?.ToString() ?? "null"}");

                if (payload == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Parsed payload is null!");
                    return Ok(new { success = false, message = "Failed to parse payload" });
                }

                var result = await _paymentService.ProcessPayOSIpnAsync(payload);

                // ✅ QUAN TRỌNG: PayOS yêu cầu trả về 200 OK để xác nhận nhận IPN
                // Log lỗi nhưng vẫn trả về 200 để tránh PayOS gửi lại callback
                if (!result.IsSuccess)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ ProcessPayOSIpnAsync failed: {result.Message}");
                    // Có thể log vào database hoặc file log ở đây
                    return Ok(new { success = false, message = result.Message });
                }

                System.Diagnostics.Debug.WriteLine("✅ PayOS IPN processed successfully!");
                System.Diagnostics.Debug.WriteLine("========================================");
                return Ok(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                // ✅ DEBUG: Log exception chi tiết
                System.Diagnostics.Debug.WriteLine($"❌ Exception in PayOSIpn: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                // Log exception và trả về 200 để PayOS không retry
                // _logger.LogError(ex, "Lỗi không mong đợi khi xử lý PayOS IPN");
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // 3. Lấy payment theo PayOSOrderCode
        [HttpGet("GetByPayOSOrderCode")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByPayOSOrderCode([FromQuery] int orderCode)
        {
            var result = await _paymentService.GetPaymentByPayOSOrderCodeAsync(orderCode);

            if (!result.IsSuccess)
            {
                return NotFound(new { success = false, message = result.Message });
            }

            return Ok(new { success = true, data = result.Data });
        }

        // ========================
        // Unified Payment Gateway
        // ========================

        /// <summary>
        /// Tạo payment với gateway được chọn (MoMo, PayOS, Cash, BankTransfer)
        /// </summary>
        [HttpPost("CreatePaymentWithGateway")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> CreatePaymentWithGateway([FromBody] CreatePaymentRequestDTO request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Request không hợp lệ." });

            var result = await _paymentService.CreatePaymentAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { success = false, message = result.Message });
            }

            return Ok(new { success = true, data = result.Data });
        }

        /// <summary>
        /// Đổi phương thức thanh toán cho payment đã tạo
        /// </summary>
        [HttpPut("ChangePaymentGateway")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> ChangePaymentGateway([FromBody] ChangePaymentGatewayRequestDTO request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Request không hợp lệ." });

            var result = await _paymentService.ChangePaymentGatewayAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { success = false, message = result.Message });
            }

            return Ok(new { success = true, data = result.Data });
        }

        /// <summary>
        /// Kiểm tra và cập nhật payment status (dùng khi callback chưa được gọi)
        /// </summary>
        [HttpPost("CheckPaymentStatus/{paymentId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CheckPaymentStatus(int paymentId)
        {
            var result = await _paymentService.CheckAndUpdatePaymentStatusAsync(paymentId);

            if (!result.IsSuccess)
            {
                return BadRequest(new { success = false, message = result.Message });
            }

            return Ok(new { success = true, message = "Payment status đã được kiểm tra." });
        }
    }
}
