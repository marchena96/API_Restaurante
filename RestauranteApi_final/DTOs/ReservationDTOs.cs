using RestauranteApi.Entities;

namespace RestauranteApi.DTOs
{
    public class ReservationCreateDto
    {
        public int ClientId { get; set; }
        public int TableId { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int GuestCount { get; set; }
    }

    public class ReservationUpdateDto
    {
        public DateOnly Date { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int GuestCount { get; set; }
    }

    public class ReservationResponseDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int GuestCount { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int TableId { get; set; }
        public string TableNumber { get; set; }
        public string ZoneName { get; set; }
        public int TurnId { get; set; }
        public string TurnName { get; set; }
    }

    public class ReservationHistoryResponseDto
    {
        public int Id { get; set; }
        public ReservationStatus PrevState { get; set; }
        public ReservationStatus NewState { get; set; }
        public DateTime ChangedAt { get; set; }
        public int ReservationId { get; set; }
    }
}
