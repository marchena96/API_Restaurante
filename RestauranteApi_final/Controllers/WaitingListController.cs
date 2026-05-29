using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DataBase;
using RestauranteApi.DTOs;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/waitinglist")]
    public class WaitingListController : ControllerBase
    {
        private readonly IWaitingListService _waitingListService;
        private readonly RestauranteApiDbContext _RestauranteApiDbContext;

        public WaitingListController(IWaitingListService waitingListService, RestauranteApiDbContext RestauranteApiDbContext)
        {
            _waitingListService = waitingListService;
            _RestauranteApiDbContext = RestauranteApiDbContext;
        }

        private WaitingListResponseDto MapToDto(Entities.WaitingList w) => new WaitingListResponseDto
        {
            Id = w.Id,
            ReqDate = w.ReqDate,
            DesiredDay = w.DesiredDay,
            DesiredTime = w.DesiredTime,
            GuestCount = w.GuestCount,
            PreferZone = w.PreferZone,
            Status = w.Status,
            ClientId = w.ClientId,
            ClientName = w.Client != null ? $"{w.Client.Name} {w.Client.LastName}" : null
        };

        // GET /api/waitinglist
        [HttpGet]
        public IActionResult GetAll()
        {
            var lista = _waitingListService.GetAll()
                .Select(w => { w.Client = _RestauranteApiDbContext.Clients.Find(w.ClientId); return MapToDto(w); })
                .ToList();
            return Ok(lista);
        }

        // GET /api/waitinglist/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var entrance = _waitingListService.GetById(id);
            if (entrance == null) return NotFound($"Entrada con id {id} no encontrada.");

            entrance.Client = _RestauranteApiDbContext.Clients.Find(entrance.ClientId);
            return Ok(MapToDto(entrance));
        }

        // GET /api/waitinglist/pending
        [HttpGet("pending")]
        public IActionResult GetPending()
        {
            var list = _waitingListService.GetPending()
                .Select(w => { w.Client = _RestauranteApiDbContext.Clients.Find(w.ClientId); return MapToDto(w); })
                .ToList();
            return Ok(list);
        }

        // POST /api/waitinglist
        [HttpPost]
        public IActionResult Create([FromBody] WaitingListCreateDto dto)
        {
            try
            {
                var entrance = _waitingListService.Create(
                    dto.ClientId, dto.DesiredDay, dto.DesiredTime, dto.GuestCount, dto.PreferZone);

                entrance.Client = _RestauranteApiDbContext.Clients.Find(entrance.ClientId)!;
                return CreatedAtAction(nameof(GetById), new { id = entrance.Id }, MapToDto(entrance));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH /api/waitinglist/{id}/assign
        [HttpPatch("{id}/assign")]
        public IActionResult Assign(int id, [FromBody] WaitingListAssignDto dto)
        {
            try
            {
                var reservation = _waitingListService.AssignTable(id, dto.TableId);
                return Ok(new ReservationResponseDto
                {
                    Id = reservation.Id,
                    Date = reservation.Date,
                    ReservationTime = reservation.ReservationTime,
                    GuestCount = reservation.GuestCount,
                    Status = reservation.Status,
                    CreatedAt = reservation.CreatedAt,
                    ClientId = reservation.ClientId,
                    ClientName = reservation.Client != null ? $"{reservation.Client.Name} {reservation.Client.LastName}" : null,
                    TableId = reservation.TableId,
                    TableNumber = reservation.Table?.Number,
                    ZoneName = reservation.Table?.Section?.Zone?.Name,
                    TurnId = reservation.TurnId,
                    TurnName = reservation.Turn?.Name
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH /api/waitinglist/{id}/cancel
        [HttpPatch("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            { 
                _waitingListService.Cancel(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found") ? NotFound(ex.Message) : BadRequest(ex.Message);
            }
        }
    }
}
