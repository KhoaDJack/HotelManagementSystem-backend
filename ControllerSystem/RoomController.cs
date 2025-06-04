using HotelDBFinal.DomainSystem;
using HotelDBFinal.InterfaceAndServiceSystem;
using Microsoft.AspNetCore.Mvc;

namespace HotelDBFinal.ControllerSystem
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null)
                return NotFound();
            return Ok(room);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Room room)
        {
            var newId = await _roomService.CreateAsync(room);
            return CreatedAtAction(nameof(GetById), new { id = newId }, room);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] Room room)
        {
            if (id != room.RoomId)
                return BadRequest();

            var result = await _roomService.UpdateAsync(room);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roomService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? query = null)
        {
            var (items, totalCount) = await _roomService.GetPagedAndSearchedAsync(page, pageSize, query);

            return Ok(new
            {
                items,
                totalCount
            });
        }

    }
}