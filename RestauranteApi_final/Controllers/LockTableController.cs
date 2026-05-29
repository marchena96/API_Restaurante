using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestauranteApi.DataBase;
using RestauranteApi.DTOs;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/lockstable")]
    public class LockTableController : ControllerBase
    {
        private readonly ILockTableService _lockTableService;
        private readonly RestauranteApiDbContext _RestauranteApiDbcontext;

        public LockTableController(ILockTableService lockTableService, RestauranteApiDbContext RestauranteApiDbContext)
        {
            _lockTableService = lockTableService;
            _RestauranteApiDbcontext = RestauranteApiDbContext;
        }

        private LockTableResponseDto MapToDto(Entities.LockTable l) => new LockTableResponseDto
        {
            Id = l.Id,
            Reason = l.Reason,
            From = l.From,
            To = l.To,
            IsActive = l.IsActive,
            TableId = l.TableId,
            TableNumber = l.Table?.Number
        };

        // GET /api/lockstable
        [HttpGet]
        public IActionResult GetAll()
        {
            var lockTables = _lockTableService.GetAllList()
                .Select(l =>
                {
                    l.Table = _RestauranteApiDbcontext.Tables.Find(l.TableId);
                    return MapToDto(l);
                }).ToList();
            return Ok(lockTables);
        }

        // GET /api/lockstable/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var lockTable = _lockTableService.GetById(id);
            if (lockTable == null) return NotFound($"LockTable with id {id} not found.");

            lockTable.Table = _RestauranteApiDbcontext.Tables.Find(lockTable.TableId);
            return Ok(MapToDto(lockTable));
        }

        // GET /api/lockstable/table/{tableId}
        [HttpGet("table/{tableId}")]
        public IActionResult GetByTable(int tableId)
        {
            var table = _RestauranteApiDbcontext.Tables.Find(tableId);
            if (table == null) return NotFound($"Table with id {tableId} not found.");

            var lockTables = _lockTableService.GetByTableId(tableId)
                .Select(l => { l.Table = table; return MapToDto(l); })
                .ToList();
            return Ok(lockTables);
        }

        // POST /api/lockstable
        [HttpPost]
        public IActionResult Create([FromBody] LockTableCreateDto dto)
        {
            try
            {
                var lockTable = _lockTableService.Create(dto.TableId, dto.Reason, dto.From, dto.To);
                lockTable.Table = _RestauranteApiDbcontext.Tables.Find(lockTable.TableId);
                return CreatedAtAction(nameof(GetById), new { id = lockTable.Id }, MapToDto(lockTable));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH /api/lockstable/{id}/cancel
        [HttpPatch("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                _lockTableService.Deactivate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE /api/lockstable/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _lockTableService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
