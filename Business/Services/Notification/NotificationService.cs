using System.Text;
using Infrastructure.Data;
using Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Business.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly TennisDbContext tennisDb;
        private readonly IConfiguration configuration;

        private string NotificationBaseUrl;
        public NotificationService(TennisDbContext tennisDb, IConfiguration configuration)
        {
            this.tennisDb = tennisDb;
            this.configuration = configuration;
            NotificationBaseUrl = configuration["ConnectionStrings:Notification"];
        }
        public async Task SendMessageTelegramId(long telegramId, string message)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { user_id = telegramId, message = message }), Encoding.UTF8, "application/json");
            var url = NotificationBaseUrl += "sendmessage";
            await httpClient.PostAsync(url, content);
        }

        public async Task SendMessageUserId(long userId, string message)
        {
            var telegramId = await tennisDb.Users
                .Where(_ => _.Id == userId)
                .Select(_ => (long?)_.TelegramId)
                .FirstOrDefaultAsync();

            if (telegramId.HasValue)
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(new { user_id = telegramId.Value, message = message }), 
                    Encoding.UTF8, 
                    "application/json"
                );
                var url = NotificationBaseUrl += "sendmessage";
                await httpClient.PostAsync(url, content);
            }
        }

        public async Task SendLinkedMessageUserId(long userId, string message, string buttonText, string link)
        {
            var telegramId = await tennisDb.Users
                .Where(_ => _.Id == userId)
                .Select(_ => (long?)_.TelegramId)
                .FirstOrDefaultAsync();

            if (telegramId.HasValue)
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(new { user_id = telegramId.Value, message = message, button_text = buttonText, link = link }), 
                    Encoding.UTF8, 
                    "application/json"
                );
                var url = NotificationBaseUrl += "msg_and_link";
                await httpClient.PostAsync("https://bot.unicort.ru/api/msg_and_link", content);
            }
        }
    }
}
