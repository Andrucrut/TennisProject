namespace Infrastructure.Data.Entities
{
    public class UserReview
    {
        public long Id { get; set; }
        public long? GameId { get; set; }
        public Game? Game { get; set; }
        public long? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public long? ReviewedPlayerId { get; set; }
        public User? ReviewedPlayer { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
