using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;
using Newtonsoft.Json.Linq;

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
    }
}
