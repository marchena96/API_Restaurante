namespace RestauranteApi.DTOs
{
    public class ClientCreateDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }

    public class ClientResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
}
