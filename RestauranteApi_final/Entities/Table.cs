namespace RestauranteApi.Entities
{
    public class Table
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public int  Capacity { get; set; }

        public int SectionId { get; set; }

        public bool IsActive { get; set; } = true;

        public Section Section { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public ICollection<LockTable> LockTables { get; set; } = new List<LockTable>();
    }
}
