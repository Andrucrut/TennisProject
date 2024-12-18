namespace Models.Models.Review
{
    public class ReviewUserRequest
    {
        public long? GameId { get; set; }
        public long? ReviewerId { get; set; }
        public long? ReviewedPlayerId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
