using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Interfaces.Interfaces;
using Infrastructure.Data.Entities;
using Models;
using Models.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Requests.Abstractions;
using Business.Services.Notification;

namespace Business.Services
{
    public class BookingProcessingBotService : IBookingProcessingBotService
    {
        private readonly TennisDbContext tennisDb;
        private readonly INotificationService notificationService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogService logService;
        private readonly string _bookingProcessingBaseUrl;

        public BookingProcessingBotService(TennisDbContext tennisDb,
            INotificationService notificationService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogService logService)
        {
            this.tennisDb = tennisDb;
            this.notificationService = notificationService;
            this.httpClientFactory = httpClientFactory;
            _bookingProcessingBaseUrl = configuration["ConnectionStrings:BookingProcessing"];
            this.logService = logService;
        }

        /// <summary>
        /// отправляем booking-process-bot запрос на бронирование
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        public async Task<string> SendBookingRequestToBot(long bookingId/*Booking booking*/) // спросить у антона что лучше передавать bookingId или booking
        {

            var booking = await tennisDb.Bookings.Include(_ => _.Schedules).FirstOrDefaultAsync(_ => _.Id == bookingId);
            var user = await tennisDb.UserBookings.Include(_ => _.User).Where(ub => ub.BookingId == booking.Id).Select(_ => _.User).FirstOrDefaultAsync();

            var statTime = booking.Schedules.Min(_ => _.StartTime);
            var endTime = booking.Schedules.Max(_ => _.EndTime);

            var schedule = booking.Schedules.FirstOrDefault();

            var court = await tennisDb.Courts.Include(_ => _.CourtOrganization).ThenInclude(_ => _.City).FirstOrDefaultAsync(_ => _.Id == schedule.CourtId);



            var bookingRequest = new SendBookingRequest
            {
                BookingId = booking.Id,
                TelegramId = user.TelegramId,
                TelegramUsername = user.TelegramUsername,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CityName = court?.CourtOrganization?.City?.Name,
                CourtOrganizationName = court.CourtOrganization.Name,
                CourtNumber = court.Number,
                Go2SportLink = court.CourtOrganization?.Go2SportLink,
                Date = booking.Date,
                PhoneNumber = user.PhoneNumber,
                StartTime = booking.Date.Add(statTime.ToTimeSpan()).ToString("yyyy-MM-ddTHH:mm:ss.fff"),
                EndTime = booking.Date.Add(endTime.ToTimeSpan()).ToString("yyyy-MM-ddTHH:mm:ss.fff")
            };
         //   return JsonConvert.SerializeObject(bookingRequest);
            try
            {
                await SendBookingRequestAsync(bookingRequest);
                //to do: исправить ошибку при добавлении лога

                return JsonConvert.SerializeObject(bookingRequest);
                //await logService.AddLog(new Log
                //{
                //    Time = DateTime.UtcNow,
                //    Request = JsonConvert.SerializeObject(booking),
                //    Controller = "SendBookingRequestToBot",
                //    Service = "SendBookingRequestToBot",
                //    LogLevel = LogLevel.Info,
                //    UserId = user.Id,
                //    BookingId = booking.Id,
                //});
            }
            
            catch(Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
                //await logService.AddLog(new Log
                //{
                //    Time = DateTime.UtcNow,
                //    Request = JsonConvert.SerializeObject(booking),
                //    Controller = "SendBookingRequestToBot",
                //    Service = "SendBookingRequestToBot",
                //    LogLevel = LogLevel.Error,
                //    UserId = user.Id,
                //    BookingId = booking.Id,
                //});
            }

                                

            return "Booking confirmed.";

        }

        /// <summary>
        /// обрабатываем результат бронирования от booking-process-bot (модератор забронировал или нет корт)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> ProcessBookingResultAsync(ProcessBookingResultRequest request)
        {
            var booking = await tennisDb.Bookings.Include(_ => _.Schedules).FirstOrDefaultAsync(b => b.Id == request.BookingId);
            if (booking == null)
                return "Booking not found.";
            var userId = await tennisDb.UserBookings.Where(_ => _.BookingId == request.BookingId).Select(_ => _.UserId).FirstOrDefaultAsync();
            if (request.Action == BookingAction.Reserved)
            {
                var gameOrder = await tennisDb.GameOrders.Include(_ => _.Game).FirstOrDefaultAsync(_ => _.BookingId == booking.Id && _.UserId == userId);
                gameOrder.Status = GameOrderStatus.Confirmed;
                gameOrder.Game.Status = GameStatus.Ready;
                await tennisDb.SaveChangesAsync();
                
                if (userId != null)
                {

                    await notificationService.SendMessageUserId((long)userId, "Корт успешно забронирован!");
                    await SendNotificationsToGuests(booking);
                }
                return "Booking confirmed.";
            }
            else if (request.Action == BookingAction.NotReserved)
            {
              
                if (userId != null)
                {
                    await notificationService.SendMessageUserId((long)userId, "Произошла ошибка в бронировании корта, корт не был забронирован, попробуйте еще раз.");
                }

                booking.Status = BookingStatus.Cancelled;

                var gameOrders = await tennisDb.GameOrders.Where(go => go.BookingId == request.BookingId).ToListAsync();

                if (gameOrders.Any())
                {
                    var game = await tennisDb.Games.FirstOrDefaultAsync(g => g.Id == gameOrders[0].GameId);
                    if (game != null)
                    {
                        game.Status = GameStatus.Canceled;
                    }
                }

                foreach (var order in gameOrders)
                {
                    order.Status = GameOrderStatus.Denied;
                }

                await tennisDb.SaveChangesAsync();

                return "Booking canceled and user notified.";
            }

            return "Invalid action.";
        }

        private async Task SendBookingRequestAsync(SendBookingRequest request)
        {
            try
            {
                using var client = httpClientFactory.CreateClient();
                string bookingProcessingUrl = _bookingProcessingBaseUrl + "booking";

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(bookingProcessingUrl, content);
                
                var result = await response.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }


        // этот метод скопирован с BackgroundTaskService. возможно стоит его переместить в NotificationService. to do: подумать над переиспользованием
        private async Task<bool> SendNotificationsToGuests(Booking booking)
        {
           
            var creatorInfo = await tennisDb.GameOrders.Include(_ => _.User).Where(_ => _.UserStatus == GameOrderUserStatus.Creator && _.BookingId == booking.Id).Select(_ => new
            {
                FirstName = _.User.FirstName,
                LastName = _.User.LastName,
                TelegramUsername = _.User.TelegramUsername
            }).FirstOrDefaultAsync();

            var invitedUserIds = await tennisDb.GameOrders.Where(_ => _.UserStatus == GameOrderUserStatus.Invited && _.BookingId == booking.Id).Select(_ => _.UserId).ToListAsync();

            //   var test = booking.Schedules.FirstOrDefault().Court.CourtOrganization.Name;

            var courId = booking.Schedules.FirstOrDefault().CourtId;
            var courtName = await tennisDb.Courts.Include(c => c.CourtOrganization).Where(c => c.Id == courId).Select(co => co.CourtOrganization.Name).FirstOrDefaultAsync();

            foreach (long userId in invitedUserIds)
            {
                await notificationService.SendLinkedMessageUserId(userId,
                   $"{creatorInfo.FirstName} {creatorInfo.LastName} @{creatorInfo.TelegramUsername} пригласил вас на игру\n" +
                   $"Корт: {courtName}", "Подробнее", "https://app.unicort.ru/games");
            }
            return true;
        }

    }
}
