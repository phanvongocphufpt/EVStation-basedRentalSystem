using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarReturnHistoryController : ControllerBase
    {
        private readonly ICarReturnHistoryService _service;

        public CarReturnHistoryController(ICarReturnHistoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound("Không tìm thấy lịch sử trả xe.");
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CarReturnHistoryCreateDTO dto)
        {
            await _service.AddAsync(dto);
            return Ok("Thêm lịch sử trả xe thành công.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CarReturnHistoryCreateDTO dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Cập nhật lịch sử trả xe thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok("Xóa lịch sử trả xe thành công.");
        }
    }
}

