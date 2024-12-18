namespace Models.Models.Game
{
    public class RespondToGameRequest: GameBaseRequest
    {
        public ActionEnum Action { get; set; }
    }
    public enum ActionEnum : int
    {
        Deny = 0,
        Accept = 1
    }
}
