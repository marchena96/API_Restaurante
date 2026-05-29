namespace RestauranteApi.Entities
{
    public class Client
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public ICollection<WaitingList> WaitingLists { get; set; } = new List<WaitingList>();
    }
}
