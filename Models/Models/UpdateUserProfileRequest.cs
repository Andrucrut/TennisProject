namespace Models.Models
{
    public class UpdateUserProfileRequest
    {
        public long UserId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? CityId { get; set; }
        public string? Birthday { get; set; }
        public int? Sex { get; set; }
        public string? PhoneNumber { get; set; }
        public int? CourtDictionaryId { get; set; }
        public double? TennisLevel { get; set; }
        public string? AboutMe { get; set; }
    }
}
