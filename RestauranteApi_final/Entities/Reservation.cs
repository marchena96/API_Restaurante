namespace RestauranteApi.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public DateOnly Date { get; set; }

        public TimeSpan ReservationTime { get; set; }

        public int GuestCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ClientId { get; set; }

        public int TableId { get; set; }

        public int TurnId { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Active;

        public Client Client { get; set; }

        public Table Table { get; set; }

        public Turn Turn { get; set; }

        public ICollection<ReservationHistory> ReservationHistories { get; set; } = new List<ReservationHistory>();
    }
}
