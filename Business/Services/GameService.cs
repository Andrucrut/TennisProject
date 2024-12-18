using Business.Mappings.Maps;
using Business.Services.Notification;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Interfaces.Interfaces;
using Interfaces.Interfaces.Scrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Dtos;
using Models.Models;
using Models.Models.Game;
using Models.Models.Payment;
using Models.Models.Scrapper;
using Newtonsoft.Json;
using System.Linq;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Business.Services
{
    public class GameService : IGameService
    {
        private readonly TennisDbContext tennisDb;
        private readonly ILogService logService;
        private readonly GameInfoMap gameInfoMap;
        private readonly PlayedGameInfoMap playedGameInfoMap;
        private readonly GameOrderUserInfoMap gameOrderUserInfoMap;
        private readonly IScrapperManagerService scrapperManager;
        private readonly IAcquiringService acquiringService;

        private readonly INotificationService notificationService;

        protected ILogger Logger { get; set; }
        public GameService(TennisDbContext tennisDb, ILogger<UserService> logger, INotificationService notificationService, ILogService logService,
            GameInfoMap gameInfoMap,
            GameOrderUserInfoMap gameOrderUserInfoMap,
            PlayedGameInfoMap playedGameInfoMap,
            IScrapperManagerService scrapperManager,
            IAcquiringService acquiringService)
        {
            this.tennisDb = tennisDb;
            Logger = logger;
            this.notificationService = notificationService;
            this.logService = logService;
            this.gameInfoMap = gameInfoMap;
            this.gameOrderUserInfoMap = gameOrderUserInfoMap;
            this.playedGameInfoMap = playedGameInfoMap;
            this.scrapperManager = scrapperManager;
            this.acquiringService = acquiringService;
        }

        public async Task<ResponseBase> CreateGame(CreateGameRequest request)
        {
            // should we validate request data??? i guess so
            // to do: insert mapper


            //twice usage of SaveChanges - perhaps not good
            // Is we save Game but not saved GameOrder??? Solve it later
            try
            {
                var game = new Game
                {
                    //  CityId = request.CityId,
                    Status = GameStatus.Opened,

                    Type = request?.Type,
                    CourtOrganizationId = request?.CourtOrganizationId,
                    Date = request?.Date.Value.ToUniversalTime(),
                };
                await tennisDb.Games.AddAsync(game);
                await tennisDb.SaveChangesAsync();
                await tennisDb.GameOrders.AddAsync(new GameOrder
                {
                    GameId = game.Id,
                    UserId = request.UserId,
                    Status = GameOrderStatus.Opend,
                    UserStatus = GameOrderUserStatus.Creator,
                    // COURT ID!!            
                });
                await Invite(request.InviteIds, game.Id);
                await tennisDb.SaveChangesAsync();
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "GameController/CreateGame",
                    Service = "CreateGame",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                    UserId = request.UserId,
                    GameId = game.Id
                });
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "GameController/CreateGame",
                    Service = "CreateGame",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                    UserId = request.UserId,
                    Message = ex.Message,
                });
                return new ResponseBase { ExceptionMess = ex.Message, Success = false, Message = "An error occurred while processing the request." };
            }


            return new ResponseBase { Success = true, Message = "Game created successfully." };
        }

        private async Task<bool> Invite(List<long>? UserIds, long gameId)
        {
            if (UserIds == null)
                return false;
            foreach (long UserId in UserIds)
            {
                await tennisDb.GameOrders.AddAsync(new GameOrder
                {
                    GameId = gameId,
                    UserId = UserId,
                    Status = GameOrderStatus.Opend,
                    UserStatus = GameOrderUserStatus.Invited,
                    // COURT ID!!            
                });

                // to do: send notifications!!!!
            }
            // await tennisDb.SaveChangesAsync();
            return true;
        }

        public async Task<RespondToInvatationResponse> RespondToInvatation(RespondToGameRequest request)
        {
            // TO DO: проработать изменение статуса игры, если заинвачено несколько пользователей
            var game = await tennisDb.Games.FirstOrDefaultAsync(_ => _.Id == request.GameId);
            if (game == null)
                return new RespondToInvatationResponse { Success = false, Message = "Game not found" };



            var gameOrderResponder = await tennisDb.GameOrders.Include(_ => _.User).FirstOrDefaultAsync(_ => _.GameId == request.GameId && _.UserId == request.UserId/* && _.Status == GameOrderStatus.Opend*/);
            if (gameOrderResponder == null)
                return new RespondToInvatationResponse { Success = false, Message = "Game order for responder not found" };
            // to do: logs

            var gameOrderCreator = await tennisDb.GameOrders.Include(_ => _.Booking).ThenInclude(_ => _.Schedules).FirstOrDefaultAsync(_ => _.GameId == request.GameId && _.UserStatus == GameOrderUserStatus.Creator);

            if (gameOrderCreator == null)
                return new RespondToInvatationResponse { Success = false, Message = "Game order for creator not found" };

            // we gotta check for other users if one accepted what are we doing with others? here is logic for only 2 users game!!!!!!
            //coution

            var startTime = gameOrderCreator.Booking.Schedules.Select(_ => _.StartTime).Min().ToShortTimeString();
            var endTime = gameOrderCreator.Booking.Schedules.Select(_ => _.EndTime).Max().ToShortTimeString();

            var gameOrderOthers = await tennisDb.GameOrders.Include(_ => _.User).Where(_ => _.GameId == request.GameId && _.UserId != request.UserId && _.UserId != gameOrderCreator.UserId)
                .ToListAsync();

            if (request.Action == ActionEnum.Accept)
            {
                gameOrderResponder.Status = GameOrderStatus.Confirmed;
                if (gameOrderOthers == null || gameOrderOthers.Count == 0)
                {
                    gameOrderCreator.Status = GameOrderStatus.Confirmed;
                    game.Status = GameStatus.Ready;
                }


                var invitedFriendsMessage = "\n";

                if (gameOrderOthers != null && gameOrderOthers.Count > 0)
                {
                    var isAllAccepted = false;
                    var counter = 1;
                    gameOrderOthers.ForEach(item =>
                    {
                        if (item.Status == GameOrderStatus.Confirmed)
                            counter++;
                    });

                    if (counter == game.PlayersAmount)
                    {
                        gameOrderCreator.Status = GameOrderStatus.Confirmed;
                        game.Status = GameStatus.Ready;
                    }

                    invitedFriendsMessage += "Приглашенные друзья: ";
                    foreach (var other in gameOrderOthers)
                        invitedFriendsMessage += $"{other.User.FirstName} {other.User.LastName} @{other.User.TelegramUsername}\n";

                }


                await notificationService.SendLinkedMessageUserId((long)gameOrderCreator.UserId,
                    $"{gameOrderResponder.User.FirstName} {gameOrderResponder.User.LastName} @{gameOrderResponder.User.TelegramUsername} принял приглашение на игру\n" +
                    $"Дата: {game.Date.Value.ToString("dd/MM/yyyy").Replace('/', '.')}\n" +
                    $"Время: {startTime} - {endTime}" +
                    $"{invitedFriendsMessage}",
                    "Подробнее",
                    "https://app.unicort.ru/games");


            }
            else if (request.Action == ActionEnum.Deny)
            {
                gameOrderResponder.Status = GameOrderStatus.Denied;
                //gameOrderCreator.Status = GameOrderStatus.Denied;
                //if (gameOrderOthers != null && gameOrderOthers.Count > 0)
                //    gameOrderOthers.ForEach(_ => _.Status = GameOrderStatus.Denied);
                //gameOrderCreator.Booking.Status = BookingStatus.Cancelled;

                await notificationService.SendLinkedMessageUserId((long)gameOrderCreator.UserId,
                    $"{gameOrderResponder.User.FirstName} {gameOrderResponder.User.LastName} @{gameOrderResponder.User.TelegramUsername} отказался от игры {game.Date.Value.ToString("dd/MM/yyyy").Replace('/', '.')} в {startTime}",
                    "Подробнее",
                    "https://app.unicort.ru/games");
            }

            game.UpdateUserId = gameOrderResponder.UserId;

            await tennisDb.GamesHistory.AddAsync(new GameHistory
            {
                Action = gameOrderResponder.Status,
                GameId = game.Id,
                UserId = gameOrderResponder.UserId,
                UserStatus = gameOrderResponder.UserStatus,
                DateTime = DateTime.UtcNow
            });

            await tennisDb.SaveChangesAsync();
            await logService.AddLog(new Log
            {
                Time = DateTime.UtcNow,
                Request = JsonConvert.SerializeObject(request),
                Controller = "GameController/RespondToInvatation",
                Service = "RespondToInvatation",
                LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                UserId = request.UserId,
                GameId = game.Id
            });
            return new RespondToInvatationResponse { GameId = game.Id, Success = true };
        }

        //NOT ACTUAL FOR PRE MVP!!!!!!!!!!
        // THIS SHIT IS FOR MATCHING PARTNERS
        public async Task<RespondToGameResponse> RespondToGame(RespondToGameRequest request)
        {
            var game = await tennisDb.Games.FirstOrDefaultAsync(_ => _.Id == request.GameId);
            if (game == null)
                return new RespondToGameResponse { Success = false, Message = "Game not found" };
            if (game.Status == GameStatus.Opened || game.Status == GameStatus.UserFound)
            {
                try
                {
                    var addOrderResponse = await tennisDb.GameOrders.AddAsync(new GameOrder
                    {
                        GameId = game.Id,
                        UserId = request.UserId,
                        Status = GameOrderStatus.Opend,
                        UserStatus = GameOrderUserStatus.Applicant
                    });
                    game.Status = GameStatus.UserFound;
                    await tennisDb.SaveChangesAsync();
                    var gameOrderCreator = await tennisDb.GameOrders.Include(_ => _.User).FirstOrDefaultAsync(_ => _.GameId == game.Id && _.UserStatus == GameOrderUserStatus.Creator);
                    //  await notificationService.SendMessage(gameOrderCreator.User.TelegramId, "Кто-то откликнулся на вашу игру, посмотрите");
                    await logService.AddLog(new Log
                    {
                        Time = DateTime.UtcNow,
                        Request = JsonConvert.SerializeObject(request),
                        Controller = "GameController/RespondToGame",
                        Service = "RespondToGame",
                        LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                        UserId = request.UserId,
                        GameId = game.Id
                    });
                }
                catch (Exception ex)
                {
                    await logService.AddLog(new Log
                    {
                        Time = DateTime.UtcNow,
                        Request = JsonConvert.SerializeObject(request),
                        Controller = "GameController/RespondToGame",
                        Service = "RespondToGame",
                        LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                        UserId = request.UserId,
                        GameId = game.Id,
                        Message = ex.Message
                    });
                    return new RespondToGameResponse { ExceptionMess = ex.Message, Success = false, Message = "Exception while adding to GameOrders table" };
                }

            }
            return new RespondToGameResponse { GameId = game.Id, Success = true };
        }

        public async Task<ResponseBase> ConfirmApplicant(ConfirmApplicantRequest request)
        {
            try
            {
                var gameOrder = await tennisDb.GameOrders.Where(_ => _.GameId == request.GameId && _.UserStatus == GameOrderUserStatus.Applicant).ToListAsync();
                var applicantGameOrder = gameOrder.FirstOrDefault(_ => _.UserId == request.ApplicantId);
                var user = await tennisDb.GameOrders.FirstOrDefaultAsync(_ => _.GameId == request.GameId && _.UserStatus == GameOrderUserStatus.Creator);

                var game = await tennisDb.Games.FirstOrDefaultAsync(_ => _.Id == request.GameId);
                if (game == null)
                    return new ResponseBase { Success = false, Message = "Game not found" };
                if (gameOrder == null)
                    return new ResponseBase { Success = false, Message = "GameOrders not found" };
                if (applicantGameOrder == null)
                    return new ResponseBase { Success = false, Message = "Applicant GameOrder not found" };
                var gameOrderCreator = await tennisDb.GameOrders.FirstOrDefaultAsync(_ => _.GameId == request.GameId && _.UserStatus == GameOrderUserStatus.Creator);


                if (request.Action == 0)
                {
                    applicantGameOrder.Status = GameOrderStatus.Confirmed;
                    game.Status = GameStatus.Ready; // или сразу на Ready?

                    // do some payment shit
                    // send notification

                }
                else if (request.Action == 1)
                {
                    applicantGameOrder.Status = GameOrderStatus.Denied;
                    if (gameOrder.Count <= 1) // проверяем есть ли другие отклики
                        game.Status = GameStatus.Opened;

                    // send notification
                }

                await tennisDb.SaveChangesAsync();
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "GameController/ConfirmApplicant",
                    Service = "ConfirmApplicant",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                    UserId = user.Id,
                    GameId = game.Id
                });
                return new ResponseBase { Success = true };
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "GameController/ConfirmApplicant",
                    Service = "ConfirmApplicant",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                    GameId = request.GameId,
                    Message = ex.Message
                });
                return new ResponseBase { Success = false, ExceptionMess = ex.Message };
            }

        }

        public async Task<GetUserGamesResponse> GetAllUserGames(GetUserGamesRequest request)
        {
            var userGameOrders = await tennisDb.GameOrders.Include(_ => _.Game)
                .Include(_ => _.Booking)
                .ThenInclude(_ => _.Schedules)
                .ThenInclude(_ => _.Court)
                .ThenInclude(_ => _.CourtOrganization)
                .Where(_ => _.UserId == request.UserId)
                .ToListAsync();
            var dtos = userGameOrders.Select(gameOrder => gameInfoMap.MapCore(gameOrder)).ToList();

            var gameOrders = new List<GameOrder>();
            foreach (var dto in dtos)
            {
                gameOrders = await tennisDb.GameOrders.Include(_ => _.User)
                                                    .Where(_ => _.GameId == dto.GameId && _.UserId != request.UserId)
                                                    .ToListAsync();
                dto.UsersInfo = gameOrders.Select(gameOrders => gameOrderUserInfoMap.MapCore(gameOrders)).ToList();
                dto.UsersInfo = dto.UsersInfo.OrderBy(_ => _.UserStatus).ToList();
            }
            return new GetUserGamesResponse { Games = dtos };

        }

        public async Task<GetUserGamesResponse> GetAllInvitedGames(GetUserGamesRequest request)
        {
            // тут только игры в которые меня пригласили и нужно ответить на запрос
            return await GetUserGamesByStatus(request, GameStatus.Opened);
        }

        public async Task<GetUserGamesResponse> GetAllActualGames(GetUserGamesRequest request)
        {
            return await GetUserGamesByStatus(request, GameStatus.Ready);

        }




        //public async Task<GetUserGamesResponse> GetUserGames(GetUserGamesRequest request)
        //{
        //    var query = tennisDb.GameOrders.Include(_ => _.Game).ThenInclude(game => game.CourtOrganization).Where(_ => _.UserId == request.UserId);

        //    if (request.UserStatus.HasValue)
        //    {
        //        query = query.Where(_ => _.UserStatus == request.UserStatus);
        //    }
        //    if (request.GameOrderStatus.HasValue)
        //    {
        //        query = query.Where(_ => _.Status == request.GameOrderStatus);
        //    }
        //    if (request.GameStatus.HasValue)
        //    {
        //        query = query.Where(_ => _.Game.Status == request.GameStatus);
        //    }
        //    var gameOrders = await query.ToListAsync();
        //    return new GetUserGamesResponse { Games = gameOrders, Success = true };
        //}

        public async Task<GetGameByIdResponse> GetGameById(long gameId)
        {
            var game = await tennisDb.Games.FirstOrDefaultAsync(_ => _.Id == gameId);
            var gameOrders = await tennisDb.GameOrders.Include(_ => _.User).Where(_ => _.GameId == gameId && _.UserStatus == GameOrderUserStatus.Applicant).ToListAsync();
            return new GetGameByIdResponse { Game = game, GameOrders = gameOrders, Success = true };
        }

        public async Task<ResponseBase> InviteToGame(InviteToGameRequest request)
        {
            var response = await Invite(request.InviteIds, request.GameId);
            await tennisDb.SaveChangesAsync();
            return new ResponseBase { Success = response };
        }

        
        public async Task<CancelGameResponse> CancelGame(CancelGameRequest request)
        {
            var game = await tennisDb.Games.FirstOrDefaultAsync(_ => _.Id == request.GameId);
            var gameOrders = await tennisDb.GameOrders.Include(_ => _.Booking).Where(_ => _.GameId == request.GameId).ToListAsync();

            if (gameOrders != null)
            {
                // удаляем записи из ScheduleOccupancies
                var booking = gameOrders.FirstOrDefault().Booking;
                //bool refoundResult;
                //if (booking.OrderId != null)
                //{
                //    refoundResult = await scrapperManager.RefoundPayment(new ScrapperPaymentRequest { OrderId = (long)booking.OrderId });
                //    if (refoundResult == false)
                //        return new CancelGameResponse { Success = false, Message = "Scrapper returned false, please try again", GameId = game.Id };
                //}

                if (booking.PaymentId != null)
                {
                    var cancelPaymentResponse = await acquiringService.CancelPayment(new RefundPaymentRequest { PaymentId = booking.PaymentId, Price = booking.Price });
                    if (cancelPaymentResponse.Success == false)
                        return new CancelGameResponse { Success = false, ExceptionMess = cancelPaymentResponse.ExceptionMess };
                }
                    



                booking.Status = BookingStatus.Cancelled;
                var occupiedSchedulesIds = booking.Schedules.Select(s => s.Id).ToList();
                var occupiedSchedules = await tennisDb.ScheduleOccupancies
                    .Where(_ => occupiedSchedulesIds.Contains((long)_.ScheduleId) && _.Date == booking.Date).ToListAsync();

                tennisDb.ScheduleOccupancies.RemoveRange(occupiedSchedules);
                // 

                gameOrders.ForEach(_ => _.Status = GameOrderStatus.Denied);
                game.Status = GameStatus.Canceled;
                game.UpdateUserId = request.UserId;

                await tennisDb.GamesHistory.AddAsync(new GameHistory
                {
                    UserId = request.UserId,
                    GameId = game.Id,
                    Action = GameOrderStatus.Denied,
                    UserStatus = gameOrders.Where(_ => _.UserId == request.UserId).FirstOrDefault()?.UserStatus,
                    DateTime = DateTime.UtcNow,
                });
            }


           
            // to do: SEND NOTIFICATIONS!!

            await tennisDb.SaveChangesAsync();

            return new CancelGameResponse { Success = true, GameId = game.Id };
            //gameOrders.Status = GameOrderStatus.
        }

        public async Task<GetUserGamesResponse> GetAllCanceledGames(GetUserGamesRequest request)
        {
            return await GetUserGamesByStatus(request, GameStatus.Canceled);
        }


        public async Task<GetPlayedUserGamesResponse> GetAllPlayedGames(GetUserGamesRequest request)
        {
            var userGameOrders = await tennisDb.GameOrders.Include(_ => _.Game)
                .Include(_ => _.Booking)
                .ThenInclude(_ => _.Schedules)
                .ThenInclude(_ => _.Court)
                .ThenInclude(_ => _.CourtOrganization)
                .Where(_ => _.UserId == request.UserId && (_.Status == GameOrderStatus.Played)).ToListAsync();

            var courtReviews = await tennisDb.CourtReviews.Where(_ => _.ReviewerId == request.UserId).ToListAsync();
            var userReviews = await tennisDb.UserReviews.Where(_ => _.ReviewerId == request.UserId).ToListAsync();
            var gameResults = await tennisDb.GameResults.Include(_ => _.Winners).Include(_ => _.ScoreResults).Where(_ => _.UserId == request.UserId).ToListAsync();

            var dtos = userGameOrders.Select(gameOrder => playedGameInfoMap.MapCore(gameOrder)).ToList();
            var gameOrders = new List<GameOrder>();
            foreach (var dto in dtos)
            {
                var courtReview = courtReviews.FirstOrDefault(_ => _.GameId == dto.GameId && _.ReviewerId == request.UserId);
                
                if (courtReview != null)
                {
                    dto.CreatedAt = courtReview.CreatedAt;
                    dto.Comment = courtReview.Comment;
                    dto.Disappointments = courtReview.Disappointments;
                    dto.Rating = courtReview.Rating;
                    dto.IsReviewed = true;
                    dto.Satisfactions = courtReview.Satisfactions;

                }
                else
                    dto.IsReviewed = false;

                var gameResult = gameResults.FirstOrDefault(_ => _.GameId == dto.GameId && _.UserId == request.UserId);
                if(gameResult != null)
                {
                    if(gameResult.Winners.Any())
                    {
                        dto.IsWinnerChosen = true;
                        dto.WinnerIds = gameResult.Winners.Select(_ => _.Id).ToList();
                    }
                    else
                        dto.IsWinnerChosen= false;
                    
                    
                    if (gameResult.ScoreResults != null && gameResult.ScoreResults.Count > 0)
                    {
                        dto.IsScoreEntered = true;
                        dto.ScoreResults = new List<ScoreResult>();
                        foreach (var score in gameResult.ScoreResults)
                        {
                            var result = new ScoreResult
                            {
                                SetNumber = score.SetNumber,
                                UserScore = score.CreatorScore,
                                OpponentScore = score.OpponentScore
                            };
                            dto.ScoreResults.Add(result);
                        }
                    }
                    else
                        dto.IsScoreEntered = false;
                    
                }

                gameOrders = await tennisDb.GameOrders.Include(_ => _.User)
                    .Where(_ => _.GameId == dto.GameId)
                    .ToListAsync();
                dto.UsersInfo = gameOrders.Select(gameOrders => gameOrderUserInfoMap.MapCore(gameOrders)).ToList();
                dto.UsersInfo = dto.UsersInfo.OrderBy(_ => _.UserStatus).ToList();
            }




            return new GetPlayedUserGamesResponse { Games = dtos };
        }

        private async Task<GetUserGamesResponse> GetUserGamesByStatus(GetUserGamesRequest request, GameStatus status)
        {

            var userGameOrders = new List<GameOrder>();

            switch (status)
            {
                case GameStatus.Opened: //invited
                    userGameOrders = await tennisDb.GameOrders.Include(_ => _.Game)
                        .Include(_ => _.Booking)
                        .ThenInclude(_ => _.Schedules)
                        .ThenInclude(_ => _.Court)
                        .ThenInclude(_ => _.CourtOrganization)
                        .Where(_ => _.UserId == request.UserId && _.UserStatus == GameOrderUserStatus.Invited && _.Status == GameOrderStatus.Opend &&
                        (_.Game.Status == GameStatus.Opened || _.Game.Status == GameStatus.Ready) &&
                        (_.Booking.Status == BookingStatus.Booked))
                        .OrderByDescending(_ => _.UserStatus).ToListAsync();
                    break;
                case GameStatus.Ready: // actual
                    userGameOrders = await tennisDb.GameOrders.Include(_ => _.Game)
                        .Include(_ => _.Booking)
                        .ThenInclude(_ => _.Schedules)
                        .ThenInclude(_ => _.Court)
                        .ThenInclude(_ => _.CourtOrganization)
                        .Where(_ => _.UserId == request.UserId && ((_.UserStatus == GameOrderUserStatus.Invited && _.Booking.Status == BookingStatus.Booked) || (_.UserStatus == GameOrderUserStatus.Creator))
                            && (_.Status == GameOrderStatus.Opend || _.Status == GameOrderStatus.Confirmed))
                        .ToListAsync();
                    break;
                case GameStatus.Canceled: // cancales
                    userGameOrders = await tennisDb.GameOrders.Include(_ => _.Game)
                        .Include(_ => _.Booking)
                        .ThenInclude(_ => _.Schedules)
                        .ThenInclude(_ => _.Court)
                        .ThenInclude(_ => _.CourtOrganization)
                        .Where(_ => _.UserId == request.UserId && (_.Status == GameOrderStatus.Denied) &&
                        ((_.UserStatus == GameOrderUserStatus.Invited && _.Booking.Status != BookingStatus.NotPayedCancelled) || _.UserStatus == GameOrderUserStatus.Creator)).ToListAsync();
                    break;

                case GameStatus.Played: // played
                    userGameOrders = await tennisDb.GameOrders.Include(_ => _.Game)
                        .Include(_ => _.Booking)
                        .ThenInclude(_ => _.Schedules)
                        .ThenInclude(_ => _.Court)
                        .ThenInclude(_ => _.CourtOrganization)
                        .Where(_ => _.UserId == request.UserId && (_.Status == GameOrderStatus.Played)).ToListAsync();
                    break;
            }

            var dtos = userGameOrders.Select(gameOrder => gameInfoMap.MapCore(gameOrder)).ToList();

            var gameOrders = new List<GameOrder>();
            foreach (var dto in dtos)
            {
                gameOrders = await tennisDb.GameOrders.Include(_ => _.User)
                                                    .Where(_ => _.GameId == dto.GameId)
                                                    .ToListAsync();
                dto.UsersInfo = gameOrders.Select(gameOrders => gameOrderUserInfoMap.MapCore(gameOrders)).ToList();
                dto.UsersInfo = dto.UsersInfo.OrderBy(_ => _.UserStatus).ToList();
            }
            return new GetUserGamesResponse { Games = dtos };
        }

    }
}

