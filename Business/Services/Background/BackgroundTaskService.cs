using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Business.Helpers;
using Business.Mappings.Maps;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Interfaces.Interfaces;
using Interfaces.Interfaces.Scrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Models;
using Models.Models.Scrapper;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Business.Services.Background
{
    public class BackgroundTaskService : IBackgroundTaskService
    {
        private readonly TennisDbContext tennisDb;
        private readonly ILogService logService;
        private readonly IScrapperManagerService scrapperManager;
        private readonly INotificationService notificationService;
        private readonly IAcquiringService acquiringService;
        private readonly IBookingProcessingBotService bookingProcessingBotService;
        protected ILogger Logger { get; set; }

        public BackgroundTaskService(TennisDbContext tennisDb, ILogger<UserService> logger, INotificationService notificationService,
                                     ILogService logService, IScrapperManagerService scrapperManager, IAcquiringService acquiringService,
                                     IBookingProcessingBotService bookingProcessingBotService)
        {
            this.tennisDb = tennisDb;
            Logger = logger;
            this.notificationService = notificationService;
            this.logService = logService;
            this.scrapperManager = scrapperManager;
            this.acquiringService = acquiringService;
            this.bookingProcessingBotService = bookingProcessingBotService;
        }

        public async Task<ResponseBase> ConvertGamesToPlayed()
        {
          
            try
            {
                var moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
                var currentTimeMoscow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);
                var currentMoscowTimeOnly = TimeOnly.FromDateTime(currentTimeMoscow);

                var gameOrders = await tennisDb.GameOrders
                    .Include(_ => _.Booking)
                    .Include(_ => _.Game)
                    .Where(g => g.Booking.Date.ToUniversalTime().Date <= DateTime.UtcNow.Date &&
                                g.Booking.Status == BookingStatus.Booked && g.Game.Status == GameStatus.Ready &&
                                g.Status == GameOrderStatus.Confirmed &&
                                g.Booking.Schedules.Max(s => s.EndTime) < currentMoscowTimeOnly).ToListAsync();

                var games = gameOrders
                    .Select(go => go.Game)
                    .DistinctBy(g => g.Id)
                    .ToList();

                games.ForEach(_ => _.Status = GameStatus.Played);
                gameOrders.ForEach(_ => _.Status = GameOrderStatus.Played);



                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Response = JsonConvert.SerializeObject(games.Select(_ => _.Id).ToList()),
                    Controller = "BackgroundTaskController/ConvertGamesToPlayed",
                    Service = "ConvertGamesToPlayed",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                });

                await tennisDb.SaveChangesAsync();
                return new ResponseBase { Success = true };
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Message = ex.Message,
                    Controller = "BackgroundTaskController/ConvertGamesToPlayed",
                    Service = "ConvertGamesToPlayed",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                });
                return new ResponseBase { Success = false, ExceptionMess = ex.Message };
            }
        }

        public async Task<ResponseBase> UpdateOccupiedSchedules()
        {

            return new ResponseBase { Success = true };

            //try
            //{
            //    var go2SportClubIds = await tennisDb.CourtOrganizations
            //        .Where(_ => _.Go2SportId != null)
            //        .Select(_ => _.Go2SportId)
            //        .ToListAsync();

            //    DateTime currentDate = DateTime.Today;
            //    DateTime fourteenDaysLater = currentDate.AddDays(14);
            //    List<DateTime> dates = new List<DateTime>();

            //    while (currentDate <= fourteenDaysLater)
            //    {
            //        dates.Add(currentDate);
            //        currentDate = currentDate.AddDays(1);
            //    }

            //    await Parallel.ForEachAsync(dates, async (date, ct) =>
            //    {
            //        foreach (var clubId in go2SportClubIds)
            //        {
            //            await scrapperManager.UpdateSlots(new GetScrapperCourtsRequest
            //            {
            //                TargetDate = date,
            //                ClubId = (int)clubId
            //            });
            //        }
            //    });

            //    await logService.AddLog(new Log
            //    {
            //        Time = DateTime.UtcNow,
            //        Response = JsonConvert.SerializeObject(go2SportClubIds),
            //        Controller = "BackgroundTaskController/UpdateOccupiedSchedules",
            //        Service = "UpdateOccupiedSchedules",
            //        LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
            //    });

            //    await tennisDb.SaveChangesAsync();

            //    return new ResponseBase { Success = true };
            //}
            //catch (Exception ex)
            //{
            //    await logService.AddLog(new Log
            //    {
            //        Time = DateTime.UtcNow,
            //        Message = ex.Message,
            //        Controller = "BackgroundTaskController/UpdateOccupiedSchedules",
            //        Service = "UpdateOccupiedSchedules",
            //        LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
            //    });
            //    await tennisDb.SaveChangesAsync();

            //    return new ResponseBase { Success = false, ExceptionMess = ex.Message };
            //}
        }

        public async Task<ResponseBase> SendGameReminders()
        {
            try
            {
                var moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
                var currentTimeMoscow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);

                var games = await tennisDb.GameOrders
                    .Include(g => g.Booking)
                    .ThenInclude(b => b.Schedules)
                    .Where(g => g.Status == GameOrderStatus.Confirmed && g.Game.Status == GameStatus.Ready)
                    .ToListAsync();

                foreach (var game in games)
                {
                    foreach (var schedule in game.Booking.Schedules)
                    {
                        var startTime = new DateTime(
                            game.Booking.Date.Year,
                            game.Booking.Date.Month,
                            game.Booking.Date.Day,
                            schedule.StartTime.Hour,
                            schedule.StartTime.Minute,
                            schedule.StartTime.Second
                        );

                        var startTimeMoscow = TimeZoneInfo.ConvertTimeFromUtc(startTime, moscowTimeZone);
                        var timeDifference = startTimeMoscow - currentTimeMoscow;

                        if (timeDifference.TotalHours <= 24 && timeDifference.TotalHours > 23)
                        {
                            await notificationService.SendMessageUserId((long)game.UserId, "Reminder: Your game starts in 24 hours!");
                        }
                        else if (timeDifference.TotalHours <= 1 && timeDifference.TotalHours > 0)
                        {
                            await notificationService.SendMessageUserId((long)game.UserId, "Reminder: Your game starts in 1 hour!");
                        }
                    }
                }

                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Response = JsonConvert.SerializeObject(games.Select(_ => _.Id).ToList()),
                    Controller = "BackgroundTaskController/SendGameReminders",
                    Service = "SendGameReminders",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                });

                await tennisDb.SaveChangesAsync();

                return new ResponseBase { Success = true };
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Message = ex.Message,
                    Controller = "BackgroundTaskController/SendGameReminders",
                    Service = "SendGameReminders",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                });
                await tennisDb.SaveChangesAsync();
                return new ResponseBase { Success = false, ExceptionMess = ex.Message };
            }
        }

        public async Task<ResponseBase> CheckPaymentStatus()
        {
            try
            {
                await ConvertBookingsWithoutPayments();
                var bookings = await tennisDb.Bookings.Include(_ => _.Schedules).Where(_ => _.Status == BookingStatus.Pending && _.PaymentId != null).ToListAsync();
                var status = new BookingStatus();
                foreach (var booking in bookings)
                {
                  //  var response = await scrapperManager.GetPaymentStatus(new ScrapperPaymentRequest { OrderId = (long)booking.OrderId });

                    var response = await acquiringService.GetPaymentStatus(new Models.Models.Payment.GetPaymentStatusRequest { PaymentId = booking.PaymentId });
                    status = EnumConvertetr.AcquiringStatusToBookingStatus(response.Status);

                    //status = BookingStatus.Booked;

                    if (response.Success == true && status != booking.Status)
                    {
                        booking.Status = status;
                        if (status == BookingStatus.Booked)
                        {
                            var processingBotResponse = await bookingProcessingBotService.SendBookingRequestToBot(booking.Id);

                            //   await SendNotificationsToGuests(booking);

                            await logService.AddLog(new Log
                            {
                                Time = DateTime.UtcNow,
                                Response = JsonConvert.SerializeObject(booking.Id + processingBotResponse),
                                Controller = "BackgroundTaskController/CheckPaymentStatus",
                                Service = "CheckPaymentStatus/BookingBot",
                                LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                            });
                        }
                        if (status == BookingStatus.Cancelled)
                        {
                            await NotPayedCancelledAction(booking.Id);
                        }
                    }
                }

                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Response = JsonConvert.SerializeObject(bookings.Select(_ => _.Id).ToList()),
                    Controller = "BackgroundTaskController/CheckPaymentStatus",
                    Service = "CheckPaymentStatus",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                });

                await tennisDb.SaveChangesAsync();
                return new ResponseBase { Success = true };
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Message = ex.Message,
                    Controller = "BackgroundTaskController/CheckPaymentStatus",
                    Service = "CheckPaymentStatus",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                });
                await tennisDb.SaveChangesAsync();
                return new ResponseBase { Success = false, ExceptionMess = ex.Message };

            }
        }


        //Проверяется через скраппер. Закоментировано, т.к скраппер не работает. Актуальная версия через эквайринг
        //public async Task<ResponseBase> CheckPaymentStatus()
        //{
        //    try
        //    {
        //        await ConvertBookingsWithoutPayments();
        //        var bookings = await tennisDb.Bookings.Include(_ => _.Schedules).Where(_ => _.Status == BookingStatus.Pending && _.OrderId != null).ToListAsync();


        //        var status = new BookingStatus();
        //        foreach (var booking in bookings)
        //        {
        //            var response = await scrapperManager.GetPaymentStatus(new ScrapperPaymentRequest { OrderId = (long)booking.OrderId });

        //            status = EnumConvertetr.ScrapperPaymentToBookingStatus(response.PaymentStatus);

        //            //status = BookingStatus.Booked;

        //            if (status != booking.Status)
        //            {
        //                booking.Status = status;
        //                if (status == BookingStatus.Booked)
        //                {
        //                    await SendNotificationsToGuests(booking);
        //                }
        //                if (status == BookingStatus.NotPayedCancelled)
        //                {
        //                    await NotPayedCancelledAction(booking.Id);
        //                }
        //            }
        //        }

        //        await logService.AddLog(new Log
        //        {
        //            Time = DateTime.UtcNow,
        //            Response = JsonConvert.SerializeObject(bookings.Select(_ => _.Id).ToList()),
        //            Controller = "BackgroundTaskController/SendGameReminders",
        //            Service = "SendGameReminders",
        //            LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
        //        });

        //        await tennisDb.SaveChangesAsync();
        //        return new ResponseBase { Success = true };
        //    }
        //    catch (Exception ex)
        //    {
        //        await logService.AddLog(new Log
        //        {
        //            Time = DateTime.UtcNow,
        //            Message = ex.Message,
        //            Controller = "BackgroundTaskController/CheckPaymentStatus",
        //            Service = "CheckPaymentStatus",
        //            LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
        //        });
        //        await tennisDb.SaveChangesAsync();
        //        return new ResponseBase { Success = false, ExceptionMess = ex.Message };

        //    }
        //}

        public async Task<ResponseBase> CapturePayments()
        {
            try
            {
                var sixDaysAgo = DateTime.UtcNow.AddDays(-6);
                var bookings = await tennisDb.Bookings.Where(_ => _.Status == BookingStatus.Booked && _.BookingTime.Date <= sixDaysAgo.Date).ToListAsync();
                foreach (var booking in bookings)
                {
                    if (booking.PaymentId != null)
                        await acquiringService.CapturePayment(new Models.Models.Payment.GetPaymentStatusRequest { PaymentId = booking.PaymentId });
                }
                return new ResponseBase { Success = true };
            }
            catch(Exception ex)
            {
                return new ResponseBase { Success = false, ExceptionMess = ex.Message };
            }
            
        }

        private async Task<bool> ConvertBookingsWithoutPayments()
        {
            var bookings = await tennisDb.Bookings.Include(_ => _.Schedules).Where(_ => _.Status == BookingStatus.Pending && _.PaymentId == null).ToListAsync();
            foreach (var booking in bookings)
            {
                booking.Status = BookingStatus.Booked;
                await SendNotificationsToGuests(booking);
            }
            await tennisDb.SaveChangesAsync();
            return true;
        }

        private async Task<bool> SendNotificationsToGuests(Booking booking)
        {
            if (booking == null)
                return false;

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

        private async Task<bool> NotPayedCancelledAction(long bookingId)
        {
            var gameOrders = await tennisDb.GameOrders.Where(_ => _.BookingId == bookingId).ToListAsync();
            gameOrders.ForEach(_ => _.Status = GameOrderStatus.Denied);

            var game = await tennisDb.Games.FirstOrDefaultAsync(_ => _.Id == gameOrders.FirstOrDefault().GameId);
            game.Status = GameStatus.Canceled;

            await tennisDb.SaveChangesAsync();
            return true;
        }

    }

}
