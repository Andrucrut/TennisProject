using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Data.Entities;

namespace Models.Dtos
{
    public class GameOrderUserInfoDto
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public string? TelegramUsername { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public GameOrderStatus? GameOrderStatus { get; set; }
        public GameOrderUserStatus? UserStatus { get; set; }
        public int? CityId { get; set; }
        public double? TennisLevel { get; set; }
        public int Age { get; set; }
        [Column(TypeName = "jsonb")]
        public string? Photo { get; set; }

    }
}
