using Microsoft.EntityFrameworkCore;
using RestauranteApi.DataBase;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Service.Implementations
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly RestauranteApiDbContext _db;
        private readonly ITurnService _turnService;

        public AvailabilityService(RestauranteApiDbContext db, ITurnService turnService)
        {
            _db = db;
            _turnService = turnService;
        }

        public List<Table> GetAvailableTables(DateOnly date, TimeSpan reservationTime, int guestCount, int? zoneId = null)
        {
            // 1. Validar que hay un turno activo para esa hora
            var turn = _turnService.FindActiveTurnForTime(reservationTime);
            if (turn == null) throw new Exception("No hay ningún turno activo que cubra ese horario.");

            // 2. Obtener mesas activas con navegación a sección/zona
            var query = _db.Tables
                .Include(t => t.Section).ThenInclude(s => s.Zone)
                .Include(t => t.Reservations)
                .Include(t => t.LockTables)
                .Where(t => t.IsActive);

            // 3. Filtrar por zona si se especificó
            if (zoneId.HasValue)
                query = query.Where(t => t.Section.ZoneId == zoneId.Value);

            var tables = query.ToList();

            var reservationDateTime = date.ToDateTime(TimeOnly.FromTimeSpan(reservationTime));

            // 4-6. Capacidad suficiente, sin reserva activa en ese turno/fecha, sin bloqueo
            return tables
                .Where(t => t.Capacity >= guestCount)
                .Where(t => !t.Reservations.Any(r =>
                    r.Date == date &&
                    r.TurnId == turn.Id &&
                    r.Status == ReservationStatus.Active))
                .Where(t => !t.LockTables.Any(l =>
                    l.IsActive &&
                    l.From <= reservationDateTime &&
                    l.To >= reservationDateTime))
                .ToList();
        }

        public TurnAvailabilityResponseDto GetAvailabilityByTurn(DateOnly date, int turnId, int? zoneId = null)
        {
            var turn = _turnService.GetById(turnId);
            if (turn == null) throw new Exception($"Turn with id {turnId} not found.");
            if (!turn.IsActive) throw new Exception($"El turno '{turn.Name}' no está activo.");

            // Hora representativa del turno para evaluar bloqueos (usamos StartTime)
            var turnDateTime = date.ToDateTime(TimeOnly.FromTimeSpan(turn.StartTime));

            var query = _db.Tables
                .Include(t => t.Section).ThenInclude(s => s.Zone)
                .Include(t => t.Reservations).ThenInclude(r => r.Client)
                .Include(t => t.LockTables)
                .Where(t => t.IsActive);

            if (zoneId.HasValue)
                query = query.Where(t => t.Section.ZoneId == zoneId.Value);

            var tables = query.ToList();

            var tableStatuses = tables.Select(t => BuildTableAvailabilityDto(t, date, turn, turnDateTime)).ToList();

            return new TurnAvailabilityResponseDto
            {
                Date = date,
                TurnId = turn.Id,
                TurnName = turn.Name,
                TurnStart = turn.StartTime,
                TurnEnd = turn.EndTime,
                TotalTables = tableStatuses.Count,
                AvailableCount = tableStatuses.Count(x => x.IsAvailable),
                OccupiedCount = tableStatuses.Count(x => !x.IsAvailable && x.ActiveReservation != null),
                LockedCount = tableStatuses.Count(x => !x.IsAvailable && x.ActiveLock != null),
                Tables = tableStatuses
            };
        }

        public TableAvailabilityDto GetTableAvailabilityStatus(int tableId, DateOnly date, TimeSpan reservationTime)
        {
            var table = _db.Tables
                .Include(t => t.Section).ThenInclude(s => s.Zone)
                .Include(t => t.Reservations).ThenInclude(r => r.Client)
                .Include(t => t.LockTables)
                .FirstOrDefault(t => t.Id == tableId);

            if (table == null) throw new Exception($"Table with id {tableId} not found.");
            if (!table.IsActive) throw new Exception($"La mesa {table.Number} no está activa.");

            var turn = _turnService.FindActiveTurnForTime(reservationTime);
            if (turn == null) throw new Exception("No hay ningún turno activo que cubra ese horario.");

            var reservationDateTime = date.ToDateTime(TimeOnly.FromTimeSpan(reservationTime));

            return BuildTableAvailabilityDto(table, date, turn, reservationDateTime);
        }

        private static TableAvailabilityDto BuildTableAvailabilityDto(
            Table table, DateOnly date, Turn turn, DateTime referenceDateTime)
        {
            // Verificar reserva activa en ese turno/fecha
            var activeReservation = table.Reservations.FirstOrDefault(r =>
                r.Date == date &&
                r.TurnId == turn.Id &&
                r.Status == ReservationStatus.Active);

            // Verificar bloqueo activo que cubra el momento de referencia
            var activeLock = table.LockTables.FirstOrDefault(l =>
                l.IsActive &&
                l.From <= referenceDateTime &&
                l.To >= referenceDateTime);

            bool isAvailable = activeReservation == null && activeLock == null;

            string? reason = null;
            if (activeReservation != null)
                reason = $"Reservada para {activeReservation.GuestCount} persona(s) — turno {turn.Name}";
            else if (activeLock != null)
                reason = $"Bloqueada: {activeLock.Reason}";

            return new TableAvailabilityDto
            {
                TableId = table.Id,
                TableNumber = table.Number,
                Capacity = table.Capacity,
                SectionName = table.Section.Name,
                ZoneName = table.Section.Zone.Name,
                IsAvailable = isAvailable,
                UnavailabilityReason = reason,
                ActiveReservation = activeReservation == null ? null : new ActiveReservationSummaryDto
                {
                    ReservationId = activeReservation.Id,
                    ClientName = activeReservation.Client?.Name ?? "—",
                    GuestCount = activeReservation.GuestCount,
                    ReservationTime = activeReservation.ReservationTime
                },
                ActiveLock = activeLock == null ? null : new ActiveLockSummaryDto
                {
                    LockId = activeLock.Id,
                    Reason = activeLock.Reason,
                    From = activeLock.From,
                    To = activeLock.To
                }
            };
        }
    }
}
