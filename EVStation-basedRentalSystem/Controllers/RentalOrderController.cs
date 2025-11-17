using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;
using System.Threading.Tasks;

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
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rentalOrderService.GetAllAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetById")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _rentalOrderService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetByUserId")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _rentalOrderService.GetByUserIdAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> Create([FromBody] CreateRentalOrderDTO dto)
        {
            if (dto == null) return BadRequest("Invalid data.");

            var result = await _rentalOrderService.CreateAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("UpdateTotal")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateTotal([FromBody] UpdateRentalOrderTotalDTO dto)
        {
            if (dto == null) return BadRequest("Invalid data.");

            var result = await _rentalOrderService.UpdateTotalAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("ConfirmTotal")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ConfirmTotal(int orderId)
        {
            var result = await _rentalOrderService.ConfirmTotalAsync(orderId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("ConfirmPayment")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var result = await _rentalOrderService.ConfirmPaymentAsync(orderId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("UpdateStatus")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateRentalOrderStatusDTO dto)
        {
            if (dto == null) return BadRequest("Invalid data.");

            var result = await _rentalOrderService.UpdateStatusAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
