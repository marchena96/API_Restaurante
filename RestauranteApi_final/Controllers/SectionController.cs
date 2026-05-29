using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/sections")]
    public class SectionController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        private SectionResponseDto MapToDto(Section s) => new SectionResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            ZoneId = s.ZoneId,
            ZoneName = s.Zone.Name
        };

        // GET /api/sections
        [HttpGet]
        public IActionResult GetAll()
        {
            var sections = _sectionService.GetAll().Select(MapToDto).ToList();
            return Ok(sections);
        }

        // GET /api/sections/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var section = _sectionService.GetById(id);
            if (section == null) return NotFound($"Section with id {id} not found.");
            return Ok(MapToDto(section));
        }

        // GET /api/sections/zone/{zoneId}
        [HttpGet("zone/{zoneId}")]
        public IActionResult GetByZone(int zoneId)
        {
            var sections = _sectionService.GetByZoneId(zoneId).Select(MapToDto).ToList();
            return Ok(sections);
        }

        // POST /api/sections
        [HttpPost]
        public IActionResult Create([FromBody] SectionCreateDto dto)
        {
            try
            {
                var section = _sectionService.Create(dto.Name, dto.ZoneId);
                return CreatedAtAction(nameof(GetById), new { id = section.Id }, MapToDto(section));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/sections/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] SectionCreateDto dto)
        {
            try
            {
                var section = _sectionService.Update(id, dto.Name, dto.ZoneId);
                return Ok(MapToDto(section));
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found") ? NotFound(ex.Message) : BadRequest(ex.Message);
            }
        }

        // DELETE /api/sections/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _sectionService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}