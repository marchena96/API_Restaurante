namespace RestauranteApi.DTOs
{
    public class SectionCreateDto
    {
        public string Name { get; set; }
        public int ZoneId { get; set; }
    }

    public class SectionResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
    }
}
