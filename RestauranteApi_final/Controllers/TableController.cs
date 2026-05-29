using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/tables")]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;
        private readonly IAvailabilityService _availabilityService;

        public TableController(ITableService tableService, IAvailabilityService availabilityService)
        {
            _tableService = tableService;
            _availabilityService = availabilityService;
        }

        private TableResponseDto MapToDto(Table t) => new TableResponseDto
        {
            Id = t.Id,
            Number = t.Number,
            Capacity = t.Capacity,
            IsActive = t.IsActive,
            SectionId = t.SectionId,
            SectionName = t.Section.Name,
            ZoneName = t.Section.Zone.Name
        };

        // GET /api/tables
        [HttpGet]
        public IActionResult GetAll()
        {
            var tables = _tableService.GetAll().Select(MapToDto).ToList();
            return Ok(tables);
        }

        // GET /api/tables/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var table = _tableService.GetById(id);
            if (table == null) return NotFound($"Table with id {id} not found.");
            return Ok(MapToDto(table));
        }

        // GET /api/tables/section/{sectionId}
        [HttpGet("section/{sectionId}")]
        public IActionResult GetBySection(int sectionId)
        {
            var tables = _tableService.GetBySectionId(sectionId).Select(MapToDto).ToList();
            return Ok(tables);
        }

        // GET /api/tables/zone/{zoneId}
        [HttpGet("zone/{zoneId}")]
        public IActionResult GetByZone(int zoneId)
        {
            var tables = _tableService.GetByZoneId(zoneId).Select(MapToDto).ToList();
            return Ok(tables);
        }

        // GET /api/tables/available?date=&time=&capacity=&zoneId=
        [HttpGet("available")]
        public IActionResult GetAvailable([FromQuery] DateOnly date, [FromQuery] TimeSpan time,
            [FromQuery] int capacity, [FromQuery] int? zoneId = null)
        {
            try
            {
                var tables = _availabilityService.GetAvailableTables(date, time, capacity, zoneId);
                var result = tables.Select(MapToDto).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/tables/availability/turn/{turnId}?date=&zoneId=
        [HttpGet("availability/turn/{turnId}")]
        public IActionResult GetAvailabilityByTurn(int turnId, [FromQuery] DateOnly date, [FromQuery] int? zoneId = null)
        {
            try
            {
                var result = _availabilityService.GetAvailabilityByTurn(date, turnId, zoneId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/tables/{id}/availability?date=&time=
        [HttpGet("{id}/availability")]
        public IActionResult GetTableAvailability(int id, [FromQuery] DateOnly date, [FromQuery] TimeSpan time)
        {
            try
            {
                var result = _availabilityService.GetTableAvailabilityStatus(id, date, time);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST /api/tables
        [HttpPost]
        public IActionResult Create([FromBody] TableCreateDto dto)
        {
            try
            {
                var table = _tableService.Create(dto.Number, dto.Capacity, dto.SectionId);
                return CreatedAtAction(nameof(GetById), new { id = table.Id }, MapToDto(table));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/tables/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TableCreateDto dto)
        {
            try
            {
                var table = _tableService.Update(id, dto.Number, dto.Capacity, dto.SectionId);
                return Ok(MapToDto(table));
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found") ? NotFound(ex.Message) : BadRequest(ex.Message);
            }
        }

        // PATCH /api/tables/{id}/deactivate
        [HttpPatch("{id}/deactivate")]
        public IActionResult Deactivate(int id)
        {
            try
            {
                _tableService.Deactivate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE /api/tables/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _tableService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}