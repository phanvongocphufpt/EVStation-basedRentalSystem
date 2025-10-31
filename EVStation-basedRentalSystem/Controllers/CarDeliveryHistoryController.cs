using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CarDeliveryHistoryController : ControllerBase
    {
        private readonly ICarDeliveryHistoryService _service;

        public CarDeliveryHistoryController(ICarDeliveryHistoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(pageIndex, pageSize);
            return Ok(new { total = result.total, data = result.data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CarDeliveryHistoryCreateDTO dto)
        {
            await _service.AddAsync(dto);
            return Ok("Giao xe thành công, đã trừ số lượng tại chi nhánh.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CarDeliveryHistoryCreateDTO dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Cập nhật thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok("Xóa thành công.");
        }

    }
}
