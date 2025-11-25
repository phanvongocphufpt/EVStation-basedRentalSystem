using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Common.VNPay.Model;
using Service.Common.VNPay.VnPayServices;
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
        private readonly IVnPayService _vnPayService;
        public PaymentController(IPaymentService paymentService, IVnPayService vnPayService)
        {
            _paymentService = paymentService;
            _vnPayService = vnPayService;
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
        public async Task<IActionResult> ConfirmDepositPayment([FromBody] ConfirmDepositPaymentDTO dto)
        {
            var result = await _paymentService.ConfirmDepositPaymentAsync(dto);
            return Ok(result);
        }
        [HttpPut("ConfirmRefundDepositCarPayment")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ConfirmRefundDepositCarPayment([FromBody] ConfirmRefundDepositCarPaymentDTO dto)
        {
            var result = await _paymentService.ConfirmRefundDepositCarAsync(dto);
            return Ok(result);
        }
        [HttpPut("ConfirmRefundOrderDepositPayment")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ConfirmRefundOrderDepositPayment([FromBody] ConfirmRefundDepositOrderPaymentDTO dto)
        {
            var result = await _paymentService.ConfirmRefundDepositOrderAsync(dto);
            return Ok(result);
        }

        [HttpPost("CreateMomoPayment")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> CreateMomoPayment([FromBody] CreateMomoPaymentDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            if (dto.OrderId <= 0)
                return BadRequest("OrderId không hợp lệ.");

            if (dto.UserId <= 0)
                return BadRequest("UserId không hợp lệ.");

            if (dto.Amount <= 0)
                return BadRequest("Amount phải lớn hơn 0.");

            if (string.IsNullOrWhiteSpace(dto.OrderInfo))
                return BadRequest("OrderInfo không được để trống.");

            var result = await _paymentService.CreateMomoPaymentAsync(dto.OrderId, dto.UserId, dto.Amount, dto.OrderInfo);
            
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(new { PaymentUrl = result.Data, Message = result.Message });
        }
    }
}
