using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Service.IServices;
using System.Threading.Tasks;

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

        // GET: api/Car/id
        [HttpGet("{Name}")]
        public async Task<IActionResult> SearchByNameAsync(string Name)
        {
            var car = await _carService.SearchByNameAsync(Name);
            if (car == null)

                return NotFound(new { message = "Car Maybe Deleted or Not Existed !!! " }); ;

            return Ok(car);
        }

        // POST: api/Car
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Car car)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _carService.AddAsync(car);
            return Ok(new { message = "Car created successfully !!! " });
        }

        // PUT: api/Car/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Car car)
        {
            if (id != car.Id)
                return BadRequest("Not Found This ID");

            await _carService.UpdateAsync(car);
            return Ok(new { message = "Car updated successfully !!!" });
        }

        // DELETE: api/Car/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _carService.DeleteAsync(id);
            return Ok(new { message = "Car deleted successfully !!!" });
        }
    }
}
