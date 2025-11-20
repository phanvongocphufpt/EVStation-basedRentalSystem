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
            var result = await _rentalOrderService.CreateAsync(createRentalOrderDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("ConfirmDocuments")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ConfirmDocuments(int orderId)
        {
            var result = await _rentalOrderService.ConfirmDocumentAsync(orderId);
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
        [HttpPut("ConfirmPayment")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var result = await _rentalOrderService.ConfirmPaymentAsync(orderId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("CancelOrder")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> CancelOrder ([FromForm] int orderId)
        {
            var result = await _rentalOrderService.CancelOrderAsync(orderId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
