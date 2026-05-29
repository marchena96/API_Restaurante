namespace RestauranteApi.Entities
{
    public class Zone
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
