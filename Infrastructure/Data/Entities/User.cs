using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities
{
    public class User
    {
        public long Id { get; set; }
        public long TelegramId { get; set; }
        public string? TelegramUsername { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? CityId { get; set; }
        public City? City { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Sex { get; set; }


        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Occupation { get; set; }
        public int? AccountStatus { get; set; }
        public double? TennisLevel { get; set; }
        public DateTime? RegistrationDate { get; set; }
        /// <summary>
        /// когда пользователь написал впервые боту /start
        /// </summary>
        public DateTime? FirstStartDate { get; set; }
        public int? CourtDictionaryId { get; set; }
        public CourtDictionary? CourtDictionary { get; set; }
        public string? HomeCourt { get; set; }
        public int? HowOftenPlay { get; set; }
        public bool? IsRegistered { get; set; }
        public string? AboutMe { get; set; }
        public DateTime? LastLogInDateTime { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Photo { get; set; }

    }
}
