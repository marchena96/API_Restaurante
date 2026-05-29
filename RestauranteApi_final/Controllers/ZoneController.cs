using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/zones")]
    public class ZonesController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public ZonesController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        private ZoneResponseDto MapToDto(Zone z) => new ZoneResponseDto
        {
            Id = z.Id,
            Name = z.Name
        };

        // GET /api/zones
        [HttpGet]
        public IActionResult GetAll()
        {
            var zones = _zoneService.GetAll().Select(MapToDto).ToList();
            return Ok(zones);
        }

        // GET /api/zones/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var zone = _zoneService.GetById(id);
            if (zone == null) return NotFound($"Zone with id {id} not found.");
            return Ok(MapToDto(zone));
        }

        // POST /api/zones
        [HttpPost]
        public IActionResult Create([FromBody] ZoneCreateDto dto)
        {
            try
            {
                var zone = _zoneService.Create(dto.Name);
                return CreatedAtAction(nameof(GetById), new { id = zone.Id }, MapToDto(zone));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/zones/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ZoneCreateDto dto)
        {
            try
            {
                var zone = _zoneService.Update(id, dto.Name);
                return Ok(MapToDto(zone));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE /api/zones/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _zoneService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}