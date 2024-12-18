using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities
{
    public class Game
    {
        public long Id { get; set; }

        // for pre mvp we dont need this shit. but later gotta think about selecting city and district for game
        //public int? CityId { get; set; }
        //public City? City { get; set; }
        //public int? DistrictId { get; set; } // or list of ids?
        //public District? District { get; set; }

        //[Column(TypeName = "jsonb")]
        //public string? DateWithoutTime { get; set; }

        /// <summary>
        /// Json like, List of TimeOnly
        /// for ex:
        /// {
        ///     [10:00, 11:00, 20:00]
        /// }
        /// </summary>

        //[Column(TypeName = "jsonb")]
        //public string? TimeSlots { get; set; }
        public DateTime? Date { get; set; }
        //public int? WhoPays { get; set; }
        //public int? Expectations { get; set; }
        //public double? GameLevelMin { get; set; }
        //public double? GameLevelMax { get; set; }
        //public string? Comment { get; set; }
        public long? UpdateUserId { get; set; }
        public User? UpdateUser { get; set; }
        public GameStatus? Status { get; set; }
        public int? Type { set; get; } // opend, not opend    0 -   closed, 1 - opend   
        public int? CourtOrganizationId { get; set; }
        public CourtOrganization? CourtOrganization { get; set; }
        public int PlayersAmount { get; set; }
        //public List<int>? CourtIds { get; set; }
        //public List<Court>? Courts { get; set; }
    }

    public enum GameStatus : int
    {
        Opened = 0,
        UserFound = 1,
        UserAccepted = 2,
        Ready = 3,
        Played = 4,
        Canceled = 5,
        Expired = 6,
    }
}
