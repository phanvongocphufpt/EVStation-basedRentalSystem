using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateFromOrder([FromBody] CreatePaymentWithOrderDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            var result = await _paymentService.CreatePaymentFromOrderAsync(dto);
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
    }
}
