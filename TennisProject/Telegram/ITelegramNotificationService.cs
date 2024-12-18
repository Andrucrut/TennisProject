namespace TennisProject.Telegram
{
    public interface ITelegramNotificationService
    {
        public Task<bool> SendMessage(long telegramId, string message);
    }
}
