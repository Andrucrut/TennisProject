namespace Models.Models
{
    public class AuthRegisterRequest
    {
        public long TelegramId {  get; set; }
        public string? TelegramUsername { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Sex { get; set; }
        public string? Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int? CityId { get; set; }
        public int? HomeCourtId { get; set; } // courtDictionaryId
        public string? HomeCourt { get; set; }
        // TODO 
        // ПОМЕНЯТЬ НА INT
        public double? TennisLevel { get; set; }
        public int? HowOftenPlay { get; set; }

        public string? Photo { get; set; }
        public List<int>? InterestIds { get; set; }
        public string? Occupation { get; set; }
      //  public int? AccountStatus { get; set; }




    }
}
