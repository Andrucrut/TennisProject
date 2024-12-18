namespace Models.Models
{
    public class SendBookingRequest
    {
        public long BookingId { get; set; }
        public long TelegramId { get; set; }
        public string TelegramUsername { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CityName { get; set; }
        public string CourtOrganizationName { get; set; }
        public int CourtNumber { get; set; }
        public string PhoneNumber { get;set; }
        public string Go2SportLink { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}