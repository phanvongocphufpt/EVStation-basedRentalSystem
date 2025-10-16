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

        // POST: api/Car
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Car car)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _carService.AddAsync(car);
            return CreatedAtAction(nameof(GetByName), new { name = car.Name }, car);
        }

        // PUT: api/Car/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Car car)
        {
            if (id != car.Id)
                return BadRequest("ID không khớp");

            await _carService.UpdateAsync(car);
            return NoContent();
        }

        // DELETE: api/Car/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _carService.DeleteAsync(id);
            return NoContent();
        }
    }
}
