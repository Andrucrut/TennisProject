namespace Infrastructure.Data.Entities
{
    public class GameHistory
    {
        public long Id { get; set; }
        public long? GameId { get; set; }
        public Game? Game { get; set; }
        public long? UserId { get; set; }
        public GameOrderStatus? Action { get; set; }
        public GameOrderUserStatus? UserStatus { get; set; }
        public DateTime DateTime { get; set; }

    }
}
