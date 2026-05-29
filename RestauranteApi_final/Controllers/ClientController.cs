using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IReservationService _reservationService;

        public ClientController(IClientService clientService, IReservationService reservationService)
        {
            _clientService = clientService;
            _reservationService = reservationService;
        }

        private ClientResponseDto MapToDto(Client c) => new ClientResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            LastName = c.LastName,
            Phone = c.Phone
        };

        // GET /api/clients
        [HttpGet]
        public IActionResult GetAll()
        {
            var clients = _clientService.GetAll().Select(MapToDto).ToList();
            return Ok(clients);
        }

        // GET /api/clients/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var client = _clientService.GetById(id);
            if (client == null) return NotFound($"Client with id {id} not found.");
            return Ok(MapToDto(client));
        }

        // GET /api/clients/{id}/reservations
        [HttpGet("{id}/reservations")]
        public IActionResult GetReservations(int id)
        {
            var client = _clientService.GetById(id);
            if (client == null) return NotFound($"Client with id {id} not found."); 
            var reservas = _reservationService.GetByClientId(id)
                .Select(r => new ReservationResponseDto
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
                }).ToList();

            return Ok(reservas);
        }

        // POST /api/clients
        [HttpPost]
        public IActionResult Create([FromBody] ClientCreateDto dto)
        {
            try
            {
                var cliente = _clientService.Create(dto.Name, dto.LastName, dto.Phone);
                return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, MapToDto(cliente));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/clients/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ClientCreateDto dto)
        {
            try
            {
                var client = _clientService.Update(id, dto.Name, dto.LastName, dto.Phone);
                return Ok(MapToDto(client));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE /api/clients/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _clientService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}