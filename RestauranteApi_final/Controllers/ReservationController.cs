using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestauranteApi.DataBase;
using RestauranteApi.Entities;
using RestauranteApi.DTOs;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly RestauranteApiDbContext _RestauranteDbContext;

        public ReservationController(IReservationService reservationService, RestauranteApiDbContext RestauranteApiDbContext)
        {
            _reservationService = reservationService;
            _RestauranteDbContext = RestauranteApiDbContext;
        }

        private ReservationResponseDto MapToDto(Entities.Reservation r) => new ReservationResponseDto
        {
            Id = r.Id,
            Date = r.Date,
            ReservationTime = r.ReservationTime,
            GuestCount = r.GuestCount,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            ClientId = r.ClientId,
            ClientName = r.Client != null ? $"{r.Client.Name} {r.Client.LastName}" : null,
            TableId = r.TableId,
            TableNumber = r.Table.Number,
            ZoneName = r.Table.Section.Zone.Name,
            TurnId = r.TurnId,
            TurnName = r.Turn.Name
        };

        // GET /api/reservations
        [HttpGet]
        public IActionResult GetAll()
        {
            var reservations = _reservationService.GetAll().Select(MapToDto).ToList();
            return Ok(reservations);
        }

        // GET /api/reservations/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var reservation = _reservationService.GetById(id);
            if (reservation == null) return NotFound($"Reservation with id {id} not found.");

            return Ok(MapToDto(reservation));
        }

        // GET /api/reservations/{id}/historical
        [HttpGet("{id}/historical")]
        public IActionResult GetHistory(int id)
        {
            var reservation = _reservationService.GetById(id);
            if (reservation == null) return NotFound($"Reservation with id {id} not found.");

            var historical = _reservationService.GetHistory(id)
                .Select(h => new ReservationHistoryResponseDto
                {
                    Id = h.Id,
                    PrevState = h.PrevState,
                    NewState = h.NewState,
                    ChangedAt = h.ChangedAt,
                    ReservationId = h.ReservationId
                }).ToList();

            return Ok(historical);
        }

        // POST /api/reservations
        [HttpPost]
        public IActionResult Create([FromBody] ReservationCreateDto dto)
        {
            try
            {
                var reservation = _reservationService.Create(
                    dto.ClientId, dto.TableId, dto.Date, dto.ReservationTime, dto.GuestCount);

                var ReservationComplete = _reservationService.GetById(reservation.Id)!;
                return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, MapToDto(ReservationComplete));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/reservations/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ReservationUpdateDto dto)
        {
            try
            {
                var reservation = _reservationService.Update(id, dto.Date, dto.ReservationTime, dto.GuestCount);
                var ReservationComplete = _reservationService.GetById(reservation.Id)!;
                return Ok(MapToDto(ReservationComplete));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH /api/reservations/{id}/cancel
        [HttpPatch("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                _reservationService.Cancel(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found") ? NotFound(ex.Message) : Conflict(ex.Message);
            }
        }

        // PATCH /api/reservations/{id}/attend
        [HttpPatch("{id}/attend")]
        public IActionResult MarkAsAttended(int id)
        {
            try
            {
                _reservationService.MarkAsAttended(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found") ? NotFound(ex.Message) : Conflict(ex.Message);
            }
        }
    }
}
