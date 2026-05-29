namespace RestauranteApi.DTOs
{
    public class LockTableCreateDto
    {
        public int TableId { get; set; }
        public string Reason { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class LockTableResponseDto
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool IsActive { get; set; }
        public int TableId { get; set; }
        public string TableNumber { get; set; }
    }
}
