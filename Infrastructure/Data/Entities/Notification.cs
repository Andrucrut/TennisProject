
namespace Infrastructure.Data.Entities
{
    public class Notification
    {
        public long Id { get; set; }
        public long TelegramId { get; set; }
        public string Message { get; set; }
        public bool IsSent { get; set; }
        public NotificationAction Action { get; set; }
        public long? UserId { get; set; }
        public User? User { get; set; }
    }

    public enum NotificationAction : int
    {
        None = 0,

    }

}
