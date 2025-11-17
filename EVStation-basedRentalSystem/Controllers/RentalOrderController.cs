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

        // GET: api/RentalOrder
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rentalOrderService.GetAllAsync();
            return MakeResultActionResult(result);
        }

        // GET: api/RentalOrder/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _rentalOrderService.GetByIdAsync(id);
            return MakeResultActionResult(result);
        }

        // GET: api/RentalOrder/User/5
        [HttpGet("User/{userId}")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _rentalOrderService.GetByUserIdAsync(userId);
            return MakeResultActionResult(result);
        }

        // POST: api/RentalOrder
        [HttpPost]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> Create([FromBody] CreateRentalOrderDTO createRentalOrderDTO)
        {
            var result = await _rentalOrderService.CreateAsync(createRentalOrderDTO);
            return MakeResultActionResult(result);
        }

        // PUT: api/RentalOrder/UpdateTotal
        [HttpPut("UpdateTotal")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateTotal([FromBody] UpdateRentalOrderTotalDTO updateRentalOrderTotalDTO)
        {
            var result = await _rentalOrderService.UpdateTotalAsync(updateRentalOrderTotalDTO);
            return MakeResultActionResult(result);
        }

        // PUT: api/RentalOrder/{orderId}/ConfirmTotal
        [HttpPut("{orderId}/ConfirmTotal")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ConfirmTotal(int orderId)
        {
            var result = await _rentalOrderService.ConfirmTotalAsync(orderId);
            return MakeResultActionResult(result);
        }

        // PUT: api/RentalOrder/{orderId}/ConfirmPayment
        [HttpPut("{orderId}/ConfirmPayment")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var result = await _rentalOrderService.ConfirmPaymentAsync(orderId);
            return MakeResultActionResult(result);
        }

        // PUT: api/RentalOrder/UpdateStatus
        // Cập nhật trạng thái order — body là UpdateRentalOrderStatusDTO
        [HttpPut("UpdateStatus")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateRentalOrderStatusDTO updateRentalOrderStatusDTO)
        {
            var result = await _rentalOrderService.UpdateStatusAsync(updateRentalOrderStatusDTO);
            return MakeResultActionResult(result);
        }

        // Helper: chuẩn hóa việc chuyển Result<T> thành IActionResult
        private IActionResult MakeResultActionResult<T>(Service.Common.Result<T> result)
        {
            if (result == null)
                return StatusCode(500, "Internal error: null result from service.");

            if (result.IsSuccess)
                return Ok(result);

            // Nếu message chứa từ khóa "không tồn tại" -> NotFound
            var msg = result.Message?.ToLower() ?? string.Empty;
            if (msg.Contains("không tồn tại") || msg.Contains("không tìm thấy") || msg.Contains("not found"))
                return NotFound(result);

            // Mặc định trả BadRequest
            return BadRequest(result);
        }
    }
}
