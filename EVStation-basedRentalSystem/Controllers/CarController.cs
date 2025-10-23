using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;

namespace EVStation_basedRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        // GET: api/Car

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cars = await _carService.GetAllAsync();
            return Ok(cars);
        }

        // GET: api/Car/byName/{name}

        [HttpGet("byName/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var car = await _carService.GetByNameAsync(name);
            if (car == null)
                return NotFound($"Không tìm thấy xe có tên: {name}");
            return Ok(car);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null)
        {
            var result = await _carService.GetPagedAsync(pageIndex, pageSize, keyword);
            return Ok(result);
        }
        // POST: api/Car
        [Authorize(Roles = "Staff,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Car car)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            car.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            car.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            await _carService.AddAsync(car);
            return CreatedAtAction(nameof(GetByName), new { name = car.Name }, car);
        }

        // PUT: api/Car/{id}
        [Authorize(Roles = "Staff,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Car car)
        {
            if (id != car.Id)

                return BadRequest("ID không khớp");
            car.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            await _carService.UpdateAsync(car);
            return NoContent();
        }

        // DELETE: api/Car/{id}
        [Authorize(Roles = "Staff,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _carService.DeleteAsync(id);
            return NoContent();
        }

        // ✅ GET: api/Car/TopRented
        [HttpGet("TopRented")]
     
        public async Task<IActionResult> GetTopRentedCars([FromQuery] int topCount = 3)
        {
            var cars = await _carService.GetTopRentedAsync(topCount);
            return Ok(cars);
        }



    }
}