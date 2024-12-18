using System.Runtime.InteropServices.ComTypes;
using Business.Helpers;
using Business.Services.Notification;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Interfaces.Interfaces;
using Interfaces.Interfaces.Scrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Models;
using Models.Models.Payment;
using Models.Models.Scrapper;
using Newtonsoft.Json;
using LogLevel = Infrastructure.Data.Entities.LogLevel;

namespace Business.Services
{
    public class BookingService : IBookingService
    {
        private readonly TennisDbContext tennisDb;
        private readonly ILogService logService;
        private readonly IScrapperManagerService scrapperManager;
        private readonly INotificationService notificationService;
        private readonly IConfiguration configuration;
        private readonly IAcquiringService acquiringService;
        protected ILogger Logger { get; set; }
        public BookingService(TennisDbContext tennisDb, ILogger<UserService> logger, ILogService logService,
            INotificationService notificationService, IScrapperManagerService scrapperManager,
            IAcquiringService acquiringService, IConfiguration configuration)
        {
            this.tennisDb = tennisDb;
            Logger = logger;
            this.logService = logService;
            this.notificationService = notificationService;
            this.scrapperManager = scrapperManager;
            this.acquiringService = acquiringService;
            this.configuration = configuration;
        }
        //public async Task<BookResponse> Book(BookRequest request)
        //{
        //    // Валидация входных данных
        //    if (request == null || request.ScheduleId <= 0 || request.UserId <= 0)
        //        return new BookResponse { Success = false, Message = "Invalid booking request" };

        //    var executionStrategy = tennisDb.Database.CreateExecutionStrategy();

        //    try
        //    {
        //        await executionStrategy.ExecuteAsync(async () =>
        //        {
        //            using (var transaction = await tennisDb.Database.BeginTransactionAsync())
        //            {
        //                try
        //                {
        //                    // Проверка наличия бронирования
        //                    var isBooked = await tennisDb.Bookings.FirstOrDefaultAsync(_ => _.ScheduleId == request.ScheduleId && _.Date.Date == request.Date.Date.ToUniversalTime());
        //                    if (isBooked != null)
        //                        throw new Exception("Court is already booked");

        //                    // Создание бронирования
        //                    var booking = await tennisDb.Bookings.AddAsync(new Data.Entities.Booking
        //                    {
        //                        BookingTime = DateTime.UtcNow,
        //                        ScheduleId = request.ScheduleId,
        //                        Date = request.Date.Date.ToUniversalTime(),
        //                        Status = Data.Entities.BookingStatus.Booked
        //                    });
        //                    await tennisDb.SaveChangesAsync();

        //                    // Добавление пользователя к бронированию
        //                    await AddUserBooking(request.UserId, booking.Entity.Id);
        //                    await tennisDb.SaveChangesAsync();


        //                    var game = new Data.Entities.Game
        //                    {
        //                        Status = GameStatus.Opened,
        //                        Type = 0,
        //                        Date = request.Date.Date.ToUniversalTime(),
        //                    };
        //                    await tennisDb.Games.AddAsync(game);
        //                    await tennisDb.SaveChangesAsync();
        //                    await tennisDb.GameOrders.AddAsync(new Data.Entities.GameOrder
        //                    {
        //                        GameId = game.Id,
        //                        UserId = request.UserId,
        //                        Status = GameOrderStatus.Opend,
        //                        UserStatus = GameOrderUserStatus.Creator,
        //                        BookingId = booking.Entity.Id,
        //                    });

        //                    await tennisDb.GamesHistory.AddAsync(new GameHistory
        //                    {
        //                        GameId = game.Id,
        //                        UserId = request.UserId,
        //                        UserStatus = GameOrderUserStatus.Creator,
        //                        Action = GameOrderStatus.Opend,
        //                        DateTime = DateTime.UtcNow,
        //                    });
        //                    await Invite(request.InviteIds, game, booking.Entity.Id);


        //                    await tennisDb.SaveChangesAsync();

        //                    // Подтверждение транзакции
        //                    await transaction.CommitAsync();

        //                    await logService.AddLog(new Log
        //                    {
        //                        Time = DateTime.UtcNow,
        //                        Request = JsonConvert.SerializeObject(request),
        //                        Controller = "BookingController/Book",
        //                        Service = "Book",
        //                        LogLevel = Data.Entities.LogLevel.Info,
        //                        UserId = request.UserId,
        //                        BookingId = booking.Entity.Id,
        //                    });
        //                }
        //                catch (Exception ex)
        //                {
        //                    // Откат транзакции при ошибке
        //                    await transaction.RollbackAsync();
        //                    await logService.AddLog(new Log
        //                    {
        //                        Time = DateTime.UtcNow,
        //                        Request = JsonConvert.SerializeObject(request),
        //                        Controller = "BookingController/Book",
        //                        Service = "Book",
        //                        LogLevel = Data.Entities.LogLevel.Error,
        //                        UserId = request.UserId,
        //                        Message = ex.Message,
        //                    });
        //                    throw; // Перебрасываем исключение наверх, чтобы обработать его внешним кодом
        //                }
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BookResponse { Success = false, ExceptionMess = ex.Message };
        //    }

        //    return new BookResponse { Success = true };
        //}

        public async Task<BookResponse> Book(BookRequest request)
        {
            // Валидация входных данных
            if (request == null || request.ScheduleIds.Count == 0 || request.UserId <= 0)
                return new BookResponse { Success = false, Message = "Invalid booking request" };

            var executionStrategy = tennisDb.Database.CreateExecutionStrategy();
            var game = new Game();
            string paymentLink = "";

            try
            {
                await executionStrategy.ExecuteAsync(async () =>
                {
                    await using (var transaction = await tennisDb.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // проверка наличия бронирования для всех запрошенных слотов в ScheduleOccupancy
                            var occupiedSchedules = await tennisDb.ScheduleOccupancies.Where(_ => request.ScheduleIds.Contains(_.Id) && _.Date == request.Date).ToListAsync();

                            if (occupiedSchedules.Any())
                                throw new Exception("Court is already booked");
                            // throw new Exception($"Some courts are already booked: {string.Join(", ", bookedSchedules.Select(b => string.Join(", ", b.Schedule.Select(s => s.Id))))}");

                            // Создание нового бронирования
                            var booking = new Booking
                            {
                                BookingTime = DateTime.UtcNow,
                                Date = request.Date.Date.ToUniversalTime(),
                                Status = BookingStatus.Pending
                            };

                            double price = 0;
                            // Добавление выбранных расписаний к бронированию и добавления расписания в занятые расписания (ScheduleOccupancies)
                            foreach (var scheduleId in request.ScheduleIds)
                            {
                                var schedule = await tennisDb.Schedules.FindAsync(scheduleId);
                                if (schedule != null)
                                    booking.Schedules.Add(schedule);
                                price += PriceForDay.GetPriceForDay(schedule.PriceJson, request.Date.DayOfWeek);
                                await tennisDb.ScheduleOccupancies.AddAsync(new ScheduleOccupancy
                                {
                                    ScheduleId = scheduleId,
                                    Date = request.Date,
                                    Reason = OccupancyReasons.BookedByOurApp,
                                    CreatedAt = DateTime.UtcNow

                                });

                            }

                            booking.Price = price;
                            await tennisDb.Bookings.AddAsync(booking);
                            await tennisDb.SaveChangesAsync();

                            // Добавление пользователя к бронированию
                            await AddUserBooking(request.UserId, booking.Id);
                            await tennisDb.SaveChangesAsync();


                            var playersAmount = 1;
                            if (request.InviteIds != null)
                                playersAmount += request.InviteIds.Count;

                            // Создание игры
                            game = new Game
                            {
                                Status = GameStatus.Opened,
                                Type = 0,
                                Date = request.Date.Date.ToUniversalTime(),
                                UpdateUserId = request.UserId,
                                PlayersAmount = playersAmount
                            };
                            await tennisDb.Games.AddAsync(game);
                            await tennisDb.SaveChangesAsync();
                            await tennisDb.GameOrders.AddAsync(new GameOrder
                            {
                                GameId = game.Id,
                                UserId = request.UserId,
                                Status = GameOrderStatus.Opend,
                                UserStatus = GameOrderUserStatus.Creator,
                                BookingId = booking.Id,
                            });

                            await tennisDb.GamesHistory.AddAsync(new GameHistory
                            {
                                GameId = game.Id,
                                UserId = request.UserId,
                                UserStatus = GameOrderUserStatus.Creator,
                                Action = GameOrderStatus.Opend,
                                DateTime = DateTime.UtcNow,
                            });
                            if (request.InviteIds != null)
                                await Invite(request.InviteIds, game, booking.Id);


                            await logService.AddLog(new Log
                            {
                                Time = DateTime.UtcNow,
                                Request = JsonConvert.SerializeObject(request),
                                Controller = "BookingController/Book",
                                Service = "Book",
                                LogLevel = LogLevel.Info,
                                UserId = request.UserId,
                                BookingId = booking.Id,
                            });

                            var user = await tennisDb.Users.FirstOrDefaultAsync(_ => _.Id == request.UserId);

                            var go2sportLink = await tennisDb.Schedules.Include(_ => _.Court).ThenInclude(_ => _.CourtOrganization)
                            .Where(_ => _.Id == request.ScheduleIds[0])
                            .Select(_ => _.Court.CourtOrganization.Go2SportLink).FirstOrDefaultAsync();


                            //if (go2sportLink != null)
                            //{
                            //    var scrapper = await scrapperManager.BookGo2Sport(new BookGo2SportRequest
                            //    {
                            //        Date = request.Date,
                            //        ScheduleIds = request.ScheduleIds,
                            //        FirstName = user.FirstName,
                            //        LastName = user.LastName,
                            //        PhoneNumber = user.PhoneNumber
                            //    });

                            //    booking.OrderId = scrapper.OrderId;
                            //    booking.PaymentLink = scrapper.PaymentLink;
                            //    paymentLink = scrapper.PaymentLink;
                            //}

                            var acquiringResponse = await acquiringService.CreatePayment(new CreatePaymentRequest
                            {
                                RedurectUrl = configuration["PaymentRedirectUrl"],
                                Price = booking.Price,
                                PhoneNumber = user.PhoneNumber
                            });

                            if (acquiringResponse.Success = true)
                            {
                                paymentLink = acquiringResponse.PaymentLink;
                                booking.PaymentLink = acquiringResponse.PaymentLink;
                                booking.PaymentId = acquiringResponse.PaymentId;
                            }

                            await notificationService.SendLinkedMessageUserId(request.UserId, "Ссылка на оплату: ", "Ссылка на оплату", paymentLink);

                            await tennisDb.SaveChangesAsync();

                            // Подтверждение транзакции
                            await transaction.CommitAsync();

                        }
                        catch (Exception ex)
                        {
                            // Откат транзакции при ошибке
                            await transaction.RollbackAsync();
                            await logService.AddLog(new Log
                            {
                                Time = DateTime.UtcNow,
                                Request = JsonConvert.SerializeObject(request),
                                Controller = "BookingController/Book",
                                Service = "Book",
                                LogLevel = LogLevel.Error,
                                UserId = request.UserId,
                                Message = ex.Message,
                            });
                            throw; // Перебрасываем исключение наверх, чтобы обработать его внешним кодом
                        }

                        // send notifications
                        //  await SendNotifications(request.InviteIds, game, request.ScheduleIds);
                        return new BookResponse { Success = true, PaymentLink = paymentLink };
                    }
                });
            }
            catch (Exception ex)
            {
                return new BookResponse { Success = false, ExceptionMess = ex.Message };
            }


            return new BookResponse { Success = true, PaymentLink = paymentLink };

        }
        private async Task<bool> SendNotifications(List<long>? userIds, Game game, List<long> scheduleIds)
        {
            if (userIds == null)
                return false;
            var creatorInfo = await tennisDb.Users
                .Where(_ => _.Id == game.UpdateUserId)
                .Select(_ => new
                {
                    FirstName = _.FirstName,
                    LastName = _.LastName,
                    TelegramUsername = _.TelegramUsername
                })
                .FirstOrDefaultAsync();

            var schedule = await tennisDb.Schedules.Include(_ => _.Court).ThenInclude(_ => _.CourtOrganization).FirstOrDefaultAsync(_ => _.Id == scheduleIds[0]);
            foreach (long userId in userIds)
            {
                await notificationService.SendLinkedMessageUserId(userId,
                   $"{creatorInfo.FirstName} {creatorInfo.LastName} @{creatorInfo.TelegramUsername} пригласил вас на игру\n" +
                   $"Корт: {schedule.Court.CourtOrganization.Name}", "Подробнее", "https://app.unicort.ru/games");
            }
            return true;
        }


        private async Task<bool> Invite(List<long>? userIds, Game game, long bookingId)
        {

            foreach (long userId in userIds)
            {
                await tennisDb.GameOrders.AddAsync(new GameOrder
                {
                    GameId = game.Id,
                    UserId = userId,
                    Status = GameOrderStatus.Opend,
                    UserStatus = GameOrderUserStatus.Invited,
                    BookingId = bookingId
                });
            }
            // await tennisDb.SaveChangesAsync();
            return true;
        }
        public async Task<ResponseBase> CancelBooking(long id)
        {
            try
            {
                var booking = await tennisDb.Bookings.FirstOrDefaultAsync(_ => _.Id == id);
                if (booking == null)
                    return new ResponseBase { Success = false, Message = "Booking doesn't exist" };
                booking.Status = BookingStatus.Cancelled;
                await tennisDb.SaveChangesAsync();

                // looking for user to add to log
                var user = await tennisDb.UserBookings.FirstOrDefaultAsync(_ => _.BookingId == id);
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(id),
                    Controller = "BookingController/CancelBooking",
                    Service = "CancelBooking",
                    LogLevel = LogLevel.Info,
                    UserId = user.Id,
                    BookingId = booking.Id,
                });
                return new ResponseBase { Success = true };
            }
            catch (Exception ex)
            {
                var user = await tennisDb.UserBookings.FirstOrDefaultAsync(_ => _.BookingId == id);
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(id),
                    Controller = "BookingController/CancelBooking",
                    Service = "CancelBooking",
                    LogLevel = LogLevel.Info,
                    UserId = user.Id,
                    BookingId = id,
                });
                return new ResponseBase { Success = false, ExceptionMess = ex.Message };
            }
        }

        private async Task<ResponseBase> AddUserBooking(long userId, long bookingId)
        {
            var userBooking = await tennisDb.UserBookings.AddAsync(new UserBooking
            {
                BookingId = bookingId,
                UserId = userId
            });
            return new ResponseBase { Success = true };
        }




    }
}
