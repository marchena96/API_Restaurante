namespace RestauranteApi.Entities
{
    public class Section
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ZoneId { get; set; }

        public Zone Zone { get; set; }

        public ICollection<Table> Tables { get; set; } = new List<Table>();
    }
}
