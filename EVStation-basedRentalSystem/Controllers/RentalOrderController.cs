using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalOrderController : ControllerBase
    {
        private readonly IRentalOrderService _rentalOrderService;

        public RentalOrderController(IRentalOrderService rentalOrderService)
        {
            _rentalOrderService = rentalOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rentalOrders = await _rentalOrderService.GetAllAsync();
            return Ok(rentalOrders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rentalOrder = await _rentalOrderService.GetByIdAsync(id);
            if (rentalOrder == null)
                return NotFound();
            return Ok(rentalOrder);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RentalOrder rentalOrder)
        {
            var createdRentalOrder = await _rentalOrderService.CreateAsync(rentalOrder);
            return CreatedAtAction(nameof(GetById), new { id = createdRentalOrder.Id }, createdRentalOrder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RentalOrder rentalOrder)
        {
            if (id != rentalOrder.Id)
                return BadRequest();
            
            var updatedRentalOrder = await _rentalOrderService.UpdateAsync(rentalOrder);
            return Ok(updatedRentalOrder);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rentalOrderService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.Common;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu authentication cho tất cả endpoints
    public class RentalOrderController : ControllerBase
    {
        private readonly IRentalOrderService _rentalOrderService;

        public RentalOrderController(IRentalOrderService rentalOrderService)
        {
            _rentalOrderService = rentalOrderService;
        }

        /// <summary>
        /// Lấy tất cả đơn hàng thuê xe
        /// </summary>
        /// <returns>Danh sách tất cả đơn hàng thuê xe</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rentalOrderService.GetAllAsync();
            
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Lấy đơn hàng thuê xe theo ID
        /// </summary>
        /// <param name="id">ID của đơn hàng thuê xe</param>
        /// <returns>Thông tin đơn hàng thuê xe</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _rentalOrderService.GetByIdAsync(id);
            
            if (!result.IsSuccess)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(result.Message);
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Tạo đơn hàng thuê xe mới
        /// </summary>
        /// <param name="rentalOrder">Thông tin đơn hàng thuê xe</param>
        /// <returns>Đơn hàng thuê xe đã được tạo</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RentalOrder rentalOrder)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _rentalOrderService.CreateAsync(rentalOrder);
            
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        /// <summary>
        /// Cập nhật đơn hàng thuê xe
        /// </summary>
        /// <param name="id">ID của đơn hàng thuê xe</param>
        /// <param name="rentalOrder">Thông tin đơn hàng thuê xe cần cập nhật</param>
        /// <returns>Đơn hàng thuê xe đã được cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RentalOrder rentalOrder)
        {
            if (id != rentalOrder.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _rentalOrderService.UpdateAsync(rentalOrder);
            
            if (!result.IsSuccess)
            {
                if (result.Message.Contains("not found"))
                
                    return NotFound(result.Message);
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Xóa đơn hàng thuê xe
        /// </summary>
        /// <param name="id">ID của đơn hàng thuê xe</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rentalOrderService.DeleteAsync(id);
            
            if (!result.IsSuccess)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(result.Message);
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message });
        }

        /// <summary>
        /// Lấy đơn hàng thuê xe theo trang (pagination)
        /// </summary>
        /// <param name="pageIndex">Số trang (bắt đầu từ 0)</param>
        /// <param name="pageSize">Số lượng item mỗi trang</param>
        /// <returns>Danh sách đơn hàng thuê xe theo trang</returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var result = await _rentalOrderService.GetPagedAsync(pageIndex, pageSize);
            
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Lấy đơn hàng thuê xe theo User ID
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <returns>Danh sách đơn hàng thuê xe của user</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _rentalOrderService.GetByUserIdAsync(userId);
            
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Lấy đơn hàng thuê xe theo Car ID
        /// </summary>
        /// <param name="carId">ID của xe</param>
        /// <returns>Danh sách đơn hàng thuê xe của xe</returns>
        [HttpGet("car/{carId}")]
        public async Task<IActionResult> GetByCarId(int carId)
        {
            var result = await _rentalOrderService.GetByCarIdAsync(carId);
            
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }
    }
}
