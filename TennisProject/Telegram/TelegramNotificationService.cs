using Interfaces.Interfaces;
using Telegram.Bot;

namespace TennisProject.Telegram
{
    public class TelegramNotificationService : ITelegramNotificationService
    {
        private readonly ITelegramBotClient botClient;
        private readonly ILogger<UpdateHandlers> logger;
        private static IAuthService AuthService { get; set; }

        public TelegramNotificationService(ITelegramBotClient botClient, ILogger<UpdateHandlers> logger)
        {
            this.botClient = botClient;
            this.logger = logger;
        }

        public async Task<bool> SendMessage(long telegramId ,string message)
        {
            await botClient.SendTextMessageAsync(telegramId, message);
            return true;
        }
    }
}
