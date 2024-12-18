namespace Models.Dtos
{
    public class UserDto
    {
        public long Id { get; set; }
        public string? TelegramUsername { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public int Age { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Sex { get; set; }
        public string? PhoneNumber { get; set; }
        public int? CourtDictionaryId { get; set; }
        public string? HomeCourtName { get; set; }
        public string? HomeCourtAddress { get; set; }
        public string? AboutMe { get; set; }

        public double? TennisLevel { get; set; }
        public DateTime? LastLogInDateTime { get; set; }
        public string? Photo { get; set; }
    }
}
