namespace Interfaces.Interfaces
{
    public interface INotificationService
    {
        public Task SendMessageTelegramId(long telegramId, string message);
        public Task SendMessageUserId(long userId, string message);
        public Task SendLinkedMessageUserId(long userId, string message, string buttonText, string link);
    }
}
