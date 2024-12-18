namespace Infrastructure.Data.Entities
{
    public class GameOrder
    {
        public int Id { get; set; }
        public long? GameId { get; set; }
        public Game? Game { get; set; }
        public long? UserId { get; set; }
        public User? User { get; set; }
        //public int? CourtId { get; set; } // нужно ли, если gameOrder 
        //public Court? Court { get; set; }
        public long? BookingId { get; set; }
        public Booking? Booking { get; set; }
        public GameOrderStatus? Status { get; set; }
        public GameOrderUserStatus? UserStatus { get; set; }
    }

    public enum GameOrderStatus : int
    {
        Opend = 0,
        Confirmed = 2,
        Played = 3,
        Denied = 4,
    }
    public enum GameOrderUserStatus : int
    {
        Creator = 0,
        Applicant = 1,
        Invited = 2,
    }

}
