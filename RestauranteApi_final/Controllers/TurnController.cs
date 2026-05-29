using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/turns")]
    public class TurnController : ControllerBase
    {
        private readonly ITurnService _turnService;

        public TurnController(ITurnService turnService)
        {
            _turnService = turnService;
        }

        private static TurnResponseDto MapToDto(Entities.Turn t) => new TurnResponseDto
        {
            Id = t.Id, Name = t.Name, StartTime = t.StartTime, EndTime = t.EndTime, IsActive = t.IsActive
        };

        // GET /api/turns
        [HttpGet]
        public IActionResult GetAll()
        {
            var turns = _turnService.GetAll().Select(MapToDto).ToList();
            return Ok(turns);
        }

        // GET /api/turns/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var turn = _turnService.GetById(id);
            if (turn == null) return NotFound($"Turn with id {id} not found.");
            return Ok(MapToDto(turn));
        }

        // POST /api/turns
        [HttpPost]
        public IActionResult Create([FromBody] TurnCreateDto dto)
        {
            var turn = _turnService.Create(dto.Name, dto.StartTime, dto.EndTime);
            return CreatedAtAction(nameof(GetById), new { id = turn.Id }, MapToDto(turn));
        }

        // PUT /api/turns/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TurnCreateDto dto)
        {
            try
            {
                var turn = _turnService.Update(id, dto.Name, dto.StartTime, dto.EndTime);
                return Ok(MapToDto(turn));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PATCH /api/turns/{id}/activate
        [HttpPatch("{id}/activate")]
        public IActionResult Activate(int id)
        {
            try
            {
                _turnService.Activate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PATCH /api/turns/{id}/deactivate
        [HttpPatch("{id}/deactivate")]
        public IActionResult Deactivate(int id)
        {
            try
            {
                _turnService.Deactivate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE /api/turns/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _turnService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
