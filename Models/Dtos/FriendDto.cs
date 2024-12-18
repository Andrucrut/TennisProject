namespace Models.Dtos
{
    public class FriendDto
    {
        public long TelegramId { get; set; }
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TelegramUsername { get; set; }
        public double? TennisLevel { get; set; }
        public int Age { get; set; }
        public string? HomeCourtName { get; set; }
        public string? HomeCourtAddress { get; set; }
        public DateTime? LastLogInDateTime { get; set; }

        //для проверки на фронте, эту штуку поменять потом
        public bool IsFriend { get; set; }

        // должен быть city name!!! но это на фронте поменять
        public int? CityId { get; set; }

        public string? CityName { get; set; }
        public string? AboutMe { get; set; }
        public string? Photo { get; set; }


    }
}
