using HotelDBFinal.DomainSystem;
using HotelDBFinal.InterfaceAndServiceSystem;
using Microsoft.AspNetCore.Mvc;

namespace HotelDBFinal.ControllerSystem
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var staffList = await _staffService.GetAllAsync();
            return Ok(staffList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var staff = await _staffService.GetByIdAsync(id);
            if (staff == null)
                return NotFound();
            return Ok(staff);
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Staff staff)
        {
            var id = await _staffService.CreateAsync(staff);
            return CreatedAtAction(nameof(GetById), new { id }, staff);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] Staff staff)
        {
            if (id != staff.StaffId) return BadRequest();
            var result = await _staffService.UpdateAsync(staff);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _staffService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10)
        {
            var result = await _staffService.GetPagedAsync(page, pageSize);
            return Ok(new
            {
                page,
                pageSize,
                totalCount = result.TotalCount,
                items = result.Items
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return BadRequest("Role query parameter is required.");

            var result = await _staffService.SearchByRoleAsync(role);
            return Ok(result);
        }

    }
}