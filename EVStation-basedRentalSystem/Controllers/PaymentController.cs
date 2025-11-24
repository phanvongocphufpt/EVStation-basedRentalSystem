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
        public async Task<IActionResult> MomoIpn([FromBody] JObject payload)
        {
            var result = await _paymentService.ProcessMomoIpnAsync(payload);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
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
        public async Task<IActionResult> PayOSIpn([FromBody] JObject payload)
        {
            var result = await _paymentService.ProcessPayOSIpnAsync(payload);

            if (!result.IsSuccess)
            {
                // Optional: log thất bại
                // _logger.LogWarning("IPN PayOS thất bại: {Message}", result.Message);
                return BadRequest(new { success = false, message = result.Message });
            }

            // PayOS thường yêu cầu trả về 200 để xác nhận nhận IPN
            return Ok(new { success = true, data = result.Data });
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
        [HttpPost("vCreatePaymentWithGateway")]
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
    }
}
