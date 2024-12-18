namespace Models.Models
{
    public class RespondToFriendRequest
    {
        public long ResponderId { get; set; }
        public long InitiatorId { get; set; }
        public FriendRespondAction Action { get; set; }
    }
    public enum FriendRespondAction: int
    {
        Deny = 0,
        Accept= 1,
    }
}
