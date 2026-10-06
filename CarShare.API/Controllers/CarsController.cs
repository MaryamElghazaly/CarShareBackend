using CarShare.BLL.DTOs.Car;
using CarShare.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : BaseController
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAvailable()
        {
            var result = await _carService.GetAllAvailableAsync();

            // HandleResult will return the list of cars including the owner info if present in DTO
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _carService.GetByIdAsync(id);
            return HandleResult(result);
        }

        [Authorize(Roles = "CarOwner")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CarCreateDTO carDTO)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == null)
                return Unauthorized("Invalid user identification in token.");

            var result = await _carService.CreateAsync(carDTO, ownerId.Value);
            return CreatedAtAction(nameof(GetById), new { id = result.CarId }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> ApproveCar(Guid id)
        {
            await _carService.ApproveCarAsync(id);
            return NoContent();
        }
    }
}
