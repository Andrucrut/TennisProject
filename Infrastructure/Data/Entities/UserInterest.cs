namespace Infrastructure.Data.Entities
{
    public class UserInterest
    {
        public long Id { get; set; }
        public int? InterestId { get; set; }
        public Interest? Interest { get; set; }
        public long? UserId { get; set; }
        public User? User { get; set; }
    }
}
