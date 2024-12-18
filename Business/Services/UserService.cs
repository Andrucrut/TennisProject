using System.Globalization;
using Business.Mappings.Maps;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Dtos;
using Models.Models;
using Models.Models.Paging;

namespace Business.Services
{
    public class UserService : IUserService
    {
        private readonly TennisDbContext tennisDb;
        private readonly ILogService logService;
        private readonly FindUserMap findUserMap;
        private readonly UserMap userMap;

        protected ILogger Logger { get; set; }
        public UserService(TennisDbContext tennisDb, ILogger<UserService> logger, ILogService logService, FindUserMap findUserMap, UserMap userMap)
        {
            this.tennisDb = tennisDb;
            Logger = logger;
            this.logService = logService;
            this.findUserMap = findUserMap;
            this.userMap = userMap;
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            var user = new User();
            if (request.Id != null)
            {
                var userById = await tennisDb.Users
                    .Include(_ => _.City)
                    .Include(_ => _.CourtDictionary)
                    .FirstOrDefaultAsync(_ => _.Id == request.Id && _.IsRegistered == true);
                if (userById == null)
                    return new GetUserResponse { Success = false, ExceptionMess = "User by user ID not found"};

                var userDto = userMap.MapCore(userById);

                return new GetUserResponse { User = userDto, Success = true, Message = "Success" };
            }
            else if (request.TelegramId != null)
            {
                var userByTelegram = await tennisDb.Users
                    .Include(_ => _.City)
                    .Include(_ => _.CourtDictionary)
                    .FirstOrDefaultAsync(_ => _.TelegramId == request.TelegramId && _.IsRegistered == true);

                if (userByTelegram == null)
                    return new GetUserResponse { Success = false, ExceptionMess = "User by user telegramID not found" };

                var userDto = userMap.MapCore(userByTelegram);
                return new GetUserResponse { User = userDto, Success = true, Message = "Success" };
            }
            return new GetUserResponse { Success = false, ExceptionMess = "User not found or other error" };
        }

        public async Task<PagedList<FindUserDto>> FindUsers(FindUsersRequest request)
        {
            if (string.IsNullOrEmpty(request.Query))
                return new PagedList<FindUserDto> { Result = new List<FindUserDto>(), PageCount = 1, Total = 0 };
            var lowerSearch = request.Query.ToLower();
            var query = tennisDb.Users
                .Include(u => u.City)
                .Include(u => u.CourtDictionary)
                .AsQueryable();

            // TO DO: проработать с продуктом как лучше организовать поиск со стороны пользователя?
            if (!string.IsNullOrEmpty(lowerSearch))
            {
                // Используем EF.Functions.Like для сравнения без учета регистра
                query = query.Where(user =>
                    (EF.Functions.Like(user.TelegramUsername.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(user.FirstName.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(user.LastName.ToLower(), $"%{lowerSearch}%")) &
                    user.FirstName != null & user.Id != request.UserId);
            }


            var pagedResult = query.Skip(request.Skip.Value).Take(request.Take.Value);

            var total = await query.CountAsync();
            var users = await pagedResult.ToListAsync();


            var friendshipSent = new List<long?>();
            var friendshipReceived = new List<long?>();

            //Проверка на дружбу
            friendshipSent = await tennisDb.Friendships.Where(_ => _.UserId == request.UserId && _.Status == FriendshipStatus.Accepted).Select(_ => _.FriendId).ToListAsync();
            friendshipReceived = await tennisDb.Friendships.Where(_ => _.FriendId == request.UserId && _.Status == FriendshipStatus.Accepted).Select(_ => _.UserId).ToListAsync();

            var sentRequests = new List<long?>();
            var receivedRequests = new List<long?>();

            // Проверка наличия запросов на дружбу, отправленных текущим пользователем
            sentRequests = await tennisDb.Friendships.Where(_ => _.UserId == request.UserId && _.Status == FriendshipStatus.Pending).Select(_ => _.FriendId).ToListAsync();

            // Проверка наличия запросов на дружбу, отправленных найденными пользователями
            receivedRequests = await tennisDb.Friendships.Where(_ => _.FriendId == request.UserId && _.Status == FriendshipStatus.Pending).Select(_ => _.UserId).ToListAsync();

            // Обновление списка пользователей

            var dtos = users.Select(user => findUserMap.MapCore(user)).ToList();
            foreach (var dto in dtos)
            {
                dto.IsFriend = friendshipSent.Contains(dto.Id) || friendshipReceived.Contains(dto.Id);
                dto.FriendshipRequestSent = sentRequests.Contains(dto.Id);
                dto.ReceivedFriendshipRequest = receivedRequests.Contains(dto.Id);
            }
            return new PagedList<FindUserDto>(dtos, total, request);
        }


        public async Task<PagedList<FindUserDto>> AllUsers(AllUsersRequest request)
        {
            var friendshipIds = await tennisDb.Friendships
                .Where(f => f.UserId == request.UserId || f.FriendId == request.UserId)
                .Select(f => f.UserId != request.UserId ? f.UserId : f.FriendId)
                .ToListAsync();

            var sendingFriendshipIds = await tennisDb.Friendships.Where(f => f.UserId == request.UserId && f.Status == FriendshipStatus.Pending).Select(f => f.FriendId).ToListAsync();

            friendshipIds = friendshipIds.Except(sendingFriendshipIds).ToList();

            var query = tennisDb.Users
                .Include(u => u.City)
                .Include(u => u.CourtDictionary)
                .AsQueryable()
                .Where(user => user.IsRegistered == true && !friendshipIds.Contains(user.Id) && user.Id != request.UserId)
                .OrderByDescending(user => user.LastLogInDateTime); 

            var pagedResult = query.Skip(request.Skip.Value).Take(request.Take.Value);

            var total = await query.CountAsync();
            var users = await pagedResult.ToListAsync();

            var dtos = users.Select(user => findUserMap.MapCore(user)).ToList();
            foreach (var dto in dtos)
            {
                dto.FriendshipRequestSent = sendingFriendshipIds.Contains(dto.Id);
                dto.TelegramUsername = null;
            }
            return new PagedList<FindUserDto>(dtos, total, request);

        }

        public async Task<ResponseBase> UpdateUserProfile(UpdateUserProfileRequest request)
        {
            var user = await tennisDb.Users.FirstOrDefaultAsync(_ => _.Id == request.UserId);

            if(user == null)
                return new ResponseBase { Success = false, ExceptionMess = "User not found" };

            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.CityId = request.CityId ?? user.CityId;
            
            if (request.Birthday != null)
                if (DateTime.TryParseExact(request.Birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    var birthday = parsedDate;
                    birthday = DateTime.SpecifyKind(birthday, DateTimeKind.Utc);
                    user.Birthday = birthday;
                }

            user.Birthday = user.Birthday;
            user.Sex = request.Sex ?? user.Sex;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.TennisLevel = request.TennisLevel ?? user.TennisLevel;
            user.CourtDictionaryId = request.CourtDictionaryId ?? user.CourtDictionaryId;
            user.AboutMe = request.AboutMe ?? user.AboutMe;

            await tennisDb.SaveChangesAsync();
            return new ResponseBase { Success = true };

        }
        public async Task<ResponseBase> DeleteUserProfile(long userId)
        {
            var user = await tennisDb.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return new ResponseBase { Success = false, ExceptionMess = "User not found" };
            user.IsRegistered = false;
            await  tennisDb.SaveChangesAsync();
            return new ResponseBase { Success = true };
        }
    }
}
