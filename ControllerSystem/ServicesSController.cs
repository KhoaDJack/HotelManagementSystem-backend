using HotelDBFinal.DomainSystem;
using HotelDBFinal.InterfaceAndServiceSystem;
using Microsoft.AspNetCore.Mvc;

namespace HotelDBFinal.ControllerSystem
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesSController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServicesSController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _serviceService.GetAllAsync();
            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null) return NotFound();
            return Ok(service);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Service service)
        {
            var id = await _serviceService.CreateAsync(service);
            return CreatedAtAction(nameof(GetById), new { id }, service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Service service)
        {
            if (id != service.ServiceId) return BadRequest();
            var result = await _serviceService.UpdateAsync(service);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _serviceService.DeleteAsync(id);
            return result ? Ok() : NotFound();
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10)
        {
            var (items, totalCount) = await _serviceService.GetPagedServicesAsync(page, pageSize);

            return Ok(new
            {
                page,
                pageSize,
                totalCount,
                items
            });
        }
    }
}