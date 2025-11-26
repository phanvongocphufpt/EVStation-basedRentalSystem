using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalOrderController : ControllerBase
    {
        private readonly IRentalOrderService _rentalOrderService;
        public RentalOrderController(IRentalOrderService rentalOrderService)
        {
            _rentalOrderService = rentalOrderService;
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rentalOrderService.GetAllAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("GetByPhone")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByPhone(string phoneNumber)
        {
            var result = await _rentalOrderService.GetByPhoneNumber(phoneNumber);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("GetByLocation")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetByLocation(int locationId)
        {
            var result = await _rentalOrderService.GetOrderByLocationAsync(locationId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("GetById")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _rentalOrderService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("GetByOrderWithPayments")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByOrderWithPayments(int orderId)
        {
            var result = await _rentalOrderService.GetByIdWithDetailsAsync(orderId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("GetByUserId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _rentalOrderService.GetByUserIdAsync(userId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> Create([FromBody] CreateRentalOrderDTO createRentalOrderDTO)
        {
            var result = await _rentalOrderService.CreateAsync(createRentalOrderDTO, HttpContext);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("UpdateTotal")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateTotal([FromBody] UpdateRentalOrderTotalDTO updateRentalOrderTotalDTO)
        {
            var result = await _rentalOrderService.UpdateTotalAsync(updateRentalOrderTotalDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("ConfirmTotal")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ConfirmTotal(int orderId)
        {
            var result = await _rentalOrderService.ConfirmTotalAsync(orderId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("ConfirmOrderPayment")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ConfirmOrderPayment([FromBody] ConfirmOrderPaymentDTO dto)
        {
            var result = await _rentalOrderService.ConfirmOrderPaymentAsync(dto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("CancelOrder")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> CancelOrder([FromForm] int orderId)
        {
            var result = await _rentalOrderService.CancelOrderAsync(orderId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("CancelOrderForStaff")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CancelOrderForStaff([FromForm] int orderId)
        {
            var result = await _rentalOrderService.CancelOrderForStaffAsync(orderId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        //[HttpGet("Checkout/PaymentCallbackVnpay")]
        //[HttpPost("Checkout/PaymentCallbackVnpay")]
        //public async Task<IActionResult> PaymentCallbackVnpay()
        //{
        //    var result = await _rentalOrderService.ProcessVnpayCallbackAsync(Request.Query);

        //    if (result.IsSuccess)
        //        return Redirect($"http://localhost:7200/payment-success?orderId={result.OrderId}");

        //    return Redirect($"http://localhost:7200/payment-failed?code={result.VnPayResponseCode ?? "99"}");
        //}
        //[HttpPost("api/payment/vnpay-ipn")]
        //public async Task<IActionResult> VnpayIpn()
        //{
        //    var result = await _rentalOrderService.ProcessVnpayIpnAsync(Request.Query);

        //    return Ok(new
        //    {
        //        RspCode = result.IsSuccess ? "00" : "99",
        //        Message = result.Message
        //    });
        //}
        [HttpPost("api/payment/confirm-orderdeposit-manual")]
        public async Task<IActionResult> ConfirmPaymentManual([FromBody] ConfirmPaymentDto dto)
        {
            // FE sẽ gửi lên TxnRef và ResponseCode
            var result = await _rentalOrderService.ProcessVnpayCallbackManualAsync(dto.TxnRef, dto.ResponseCode);

            if (result.IsSuccess)
            {
                return Ok(new { success = true, message = "Thanh toán thành công!", orderId = result.OrderId });
            }

            return Ok(new { success = false, message = result.Message });
        }

        public class ConfirmPaymentDto
        {
            public string TxnRef { get; set; } = string.Empty;
            public string ResponseCode { get; set; } = string.Empty;
        }
        [HttpPost("AddContactToOrder")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> AddContactToOrder([FromBody] AddContactToOrderDTO dto)
        {
            var result = await _rentalOrderService.AddContactToOrderAsync(dto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("GetContactFromOrder")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetContactFromOrder(int orderId)
        {
            var result = await _rentalOrderService.GetContactFromOrderDTO(orderId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("UpdateContact")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> UpdateContact([FromBody] GetContactFromOrderDTO dto)
        {
            var result = await _rentalOrderService.UpdateContact(dto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}