namespace Models.Dtos
{
    public class FriendshipRequestDto
    {
        public long? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TelegramUsername { get; set; }
        public double? TennisLevel { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public DateTime? LastLogInDateTime { get; set; }
        public string? HomeCourtName { get; set; }
        public string? HomeCourtAddress { get; set; }
        public string? AboutMe { get; set; }
        public int Age { get; set; }

        public string? Photo { get; set; }
    }
}
