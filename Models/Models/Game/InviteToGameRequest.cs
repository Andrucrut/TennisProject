namespace Models.Models.Game
{
    public class InviteToGameRequest
    {
        public List<long>? InviteIds { get; set; }
        public long GameId { get; set; }
    }
}
