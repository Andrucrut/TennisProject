using Business.Mappings.Maps;
using Business.Services.Notification;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Models;
using Newtonsoft.Json;

namespace Business.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly TennisDbContext tennisDb;
        private readonly ILogService logService;
        private readonly FriendshipRequestsMap friendshipRequestsMap;
        private readonly FriendMap friendMap;

        private readonly INotificationService notificationService;

        protected ILogger Logger { get; set; }
        public FriendshipService(TennisDbContext tennisDb, ILogger<UserService> logger, ILogService logService, 
            FriendshipRequestsMap friendshipRequestsMap, FriendMap friendMap, INotificationService notificationService)
        {
            this.tennisDb = tennisDb;
            Logger = logger;
            this.logService = logService;
            this.friendshipRequestsMap = friendshipRequestsMap;
            this.friendMap = friendMap;
            this.notificationService = notificationService;
        }

        public async Task<AddFriendResponse> Add(AddFriendRequest request)
        {
            try
            {
                //user validation idk if needed but okay
                var isUser1 = await tennisDb.Users.AnyAsync(u => u.Id == request.UserId);
                var isUser2 = await tennisDb.Users.AnyAsync(u => u.Id == request.UserId);

                if (isUser1 == false)
                    return new AddFriendResponse { Success = false, ExceptionMess = $"No such record in Users table with telegram id = userId {request.UserId}" };
                if (isUser2 == false)
                    return new AddFriendResponse { Success = false, ExceptionMess = $"No such record in Users table with telegram id = userId {request.FriendId}" };

                //validation (if they are already friends)
                var friendship = await tennisDb.Friendships.FirstOrDefaultAsync(_ => (_.UserId == request.UserId && _.FriendId == request.FriendId) ||
                                                                 (_.UserId == request.FriendId && _.FriendId == request.UserId)); // change logic (perhaps we should check statuses)

                if (friendship != null)
                    if (friendship.Status != FriendshipStatus.Declined)
                        return new AddFriendResponse { Success = false, ExceptionMess = "They are already friends" };

                await tennisDb.Friendships.AddAsync(new Friendship
                {
                    CreationTime = DateTime.UtcNow,
                    UpdateTime = DateTime.UtcNow,
                    UserId = request.UserId,
                    FriendId = request.FriendId,
                    Status = FriendshipStatus.Pending
                });
                await tennisDb.SaveChangesAsync();

                var userInfo = await tennisDb.Users.Where(_ => _.Id == request.UserId).Select(_ => new
                {
                    FirstName = _.FirstName,
                    LastName = _.LastName,
                    TelegramUsername = _.TelegramUsername
                }).FirstOrDefaultAsync();

                await notificationService.SendLinkedMessageUserId(request.FriendId, $"{userInfo.FirstName} {userInfo.LastName} @{userInfo.TelegramUsername} хочет добавить вас в друзья",
                    "Подробнее", "https://app.unicort.ru/notification");

                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "FriendshipController/Add",
                    Service = "Add",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                    UserId = request.UserId,
                });

                return new AddFriendResponse {FriendId = request.FriendId, Success = true, Message = "Friendship added" };
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "FriendshipController/Add",
                    Service = "Add",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                    UserId = request.UserId,
                    Message = ex.Message,
                });
                return new AddFriendResponse { Success = false, ExceptionMess = ex.Message };
            }

        }

        public async Task<DeleteFriendResponse> Delete(DeleteFriendRequest request)
        {
            try
            {
                var friendship = await tennisDb.Friendships.FirstOrDefaultAsync(_ => (_.UserId == request.UserId && _.FriendId == request.FriendId) ||
                                                                 (_.UserId == request.FriendId && _.FriendId == request.UserId)); // change logic (perhaps we should check statuses)

                if (friendship == null)
                    return new DeleteFriendResponse { Success = false, ExceptionMess = "They are not friends" };

                tennisDb.Friendships.Remove(friendship);
                await tennisDb.SaveChangesAsync();

                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "FriendshipController/Delete",
                    Service = "Delete",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                    UserId = request.UserId,
                });
                return new DeleteFriendResponse { Success = true, Message = "Friendship has been deletes", FriendId = request.FriendId };
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "FriendshipController/Delete",
                    Service = "Delete",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                    UserId = request.UserId,
                    Message = ex.Message
                });
                return new DeleteFriendResponse { Success = false, ExceptionMess = ex.Message };
            }
        }

        public async Task<GetFriendsResponse> GetFriendsById(GetFriendsByIdRequest request)
        {
            try
            {
                // var friendships = tennisDb.Friendships.Where(_ => (_.UserId == userId || _.FriendId == userId) && (_.Status == Data.Entities.FriendshipStatus.Accepted)).ToList();

                if (request.Id == 0 || request.Id == null)
                    request.Id = await tennisDb.Users.Where(_ => _.TelegramId == request.TelegramId).Select(_ => _.Id).FirstOrDefaultAsync();
                var userId = request.Id;
                var friendshipInitiated = tennisDb.Friendships.Where(_ => _.UserId == userId && _.Status == FriendshipStatus.Accepted);
                var friendshipNotInitiated = tennisDb.Friendships.Where(_ => _.FriendId == userId && _.Status == FriendshipStatus.Accepted);

                var friendsInitiatedByUser = await tennisDb.Friendships.Where(f => f.UserId == userId && f.Status == FriendshipStatus.Accepted)
                .Select(f => f.FriendId)
                .ToListAsync();

                var friendsOfUser = await tennisDb.Friendships.Where(f => f.FriendId == userId && f.Status == FriendshipStatus.Accepted)
                    .Select(f => f.UserId)
                    .ToListAsync();

                var friendIds = friendsInitiatedByUser.Union(friendsOfUser).ToList();

                var friendProfiles = await tennisDb.Users.Include(u => u.City)
                    .Include(u => u.CourtDictionary)
                    .Where(_ => friendIds.Contains(_.Id)).ToListAsync();

                var friendDtos = friendProfiles.Select(_ => friendMap.MapCore(_)).ToList();

                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "FriendshipController/GetFriendsById",
                    Service = "GetFriendsById",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                    UserId = request.Id,
                    Response = JsonConvert.SerializeObject(friendDtos),
                });
                return new GetFriendsResponse { Friends = friendDtos, Success = true };
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "FriendshipController/GetFriendsById",
                    Service = "GetFriendsById",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                    UserId = request.Id,
                    Message = ex.Message
                });
                return new GetFriendsResponse { ExceptionMess = ex.Message, Success = false };
            }

        }

        public async Task<RespondToFriendRequestResponse> Respond(RespondToFriendRequest request)
        {
            try
            {
                var friendship = await tennisDb.Friendships.FirstOrDefaultAsync(_ => _.UserId == request.InitiatorId && _.FriendId == request.ResponderId && _.Status == FriendshipStatus.Pending);
                if (friendship == null)
                    return new RespondToFriendRequestResponse { Success = false, Message = "Friendship not found" };
                if (request.Action == FriendRespondAction.Accept)
                    friendship.Status = FriendshipStatus.Accepted;
                else
                    friendship.Status = FriendshipStatus.Declined;
                await tennisDb.SaveChangesAsync();

                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "FriendshipController/Respond",
                    Service = "Respond",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                    UserId = request.ResponderId,
                });
                return new RespondToFriendRequestResponse { FriendId = request.InitiatorId ,Success = true };
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "FriendshipController/Respond",
                    Service = "Respond",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                    UserId = request.ResponderId,
                    Message = ex.Message,
                });
                return new RespondToFriendRequestResponse { Success = false, ExceptionMess = ex.Message };
            }
            

        }

        public async Task<GetFriendRequestsResponse> GetFriendRequests(UserBaseRequest request)
        {
            try
            {
                if (request.Id == 0)
                    request.Id = await tennisDb.Users.Where(_ => _.TelegramId == request.TelegramId).Select(_ => _.Id).FirstOrDefaultAsync();
                var friendshipRequests = await tennisDb.Friendships.Include(f => f.User)
                    .ThenInclude(u => u.City)
                    .Include(f => f.User)
                    .ThenInclude(u => u.CourtDictionary)
                    .Where(_ => _.FriendId == request.Id && _.Status == FriendshipStatus.Pending).ToListAsync();

                var dtos = friendshipRequests.Select(_ =>  friendshipRequestsMap.MapCore(_)).ToList();

                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "FriendshipController/GetFriendRequests",
                    Service = "GetFriendRequests",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                    UserId = request.Id,
                    Response = JsonConvert.SerializeObject(dtos),
                });

                return new GetFriendRequestsResponse { Friendships = dtos, Success = true };
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "FriendshipController/GetFriendRequests",
                    Service = "GetFriendRequests",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                    UserId = request.Id,
                    Message = ex.Message,
                });
                return new GetFriendRequestsResponse { Success = false, ExceptionMess = ex.Message };
            }
            
        }


    }
}
