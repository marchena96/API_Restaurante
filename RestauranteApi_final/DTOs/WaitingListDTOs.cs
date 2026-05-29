namespace RestauranteApi.DTOs
{
    public class WaitingListCreateDto
    {
        public int ClientId { get; set; }
        public DateOnly DesiredDay { get; set; }
        public TimeSpan DesiredTime { get; set; }
        public int GuestCount { get; set; }
        public string? PreferZone { get; set; }
    }

    public class WaitingListAssignDto
    {
        public int TableId { get; set; }
    }

    public class WaitingListResponseDto
    {
        public int Id { get; set; }
        public DateTime ReqDate { get; set; }
        public DateOnly DesiredDay { get; set; }
        public TimeSpan DesiredTime { get; set; }
        public int GuestCount { get; set; }
        public string PreferZone { get; set; }
        public string Status { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
