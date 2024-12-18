using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Dtos
{
    public class FindUserDto
    {
        public long Id { get; set; }
        public string? TelegramUsername { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public int Age { get; set; }
        public int? Sex { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public double? TennisLevel { get; set; }
        public string? HomeCourtName { get; set; }
        public string? HomeCourtAddress { get; set; }
        public string? AboutMe { get; set; }
        public DateTime? LastLogInDateTime { get; set; }
        public bool? IsFriend { get; set; }
        public bool? FriendshipRequestSent { get; set; }
        public bool? ReceivedFriendshipRequest { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Photo { get; set; }


    }
}
