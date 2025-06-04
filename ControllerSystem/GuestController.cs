using HotelDBFinal.DomainSystem;
using HotelDBFinal.InterfaceAndServiceSystem;
using Microsoft.AspNetCore.Mvc;

namespace HotelDBFinal.ControllerSystem
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : ControllerBase
    {
        private readonly IGuestService _guestService;

        public GuestController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var guests = await _guestService.GetAllAsync();
            return Ok(guests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var guest = await _guestService.GetByIdAsync(id);
            if (guest == null) return NotFound();
            return Ok(guest);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Guest guest)
        {
            var id = await _guestService.CreateAsync(guest);
            return CreatedAtAction(nameof(GetById), new { id }, guest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Guest guest)
        {
            if (id != guest.GuestId) return BadRequest();
            var result = await _guestService.UpdateAsync(guest);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _guestService.DeleteAsync(id);
            return result ? Ok() : NotFound();
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (items, totalCount) = await _guestService.GetPagedAsync(page, pageSize);

            return Ok(new
            {
                totalCount,
                items
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
                return BadRequest("LastName is required.");

            var guests = await _guestService.SearchByLastNameAsync(lastName);
            return Ok(guests);
        }

    }
}