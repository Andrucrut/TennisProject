using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Models;
using Newtonsoft.Json;
using System.Globalization;
using Interfaces.Interfaces;
using Telegram.Bot;
using LogLevel = Infrastructure.Data.Entities.LogLevel;

namespace Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly TennisDbContext tennisDb;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogService logService;
        protected ILogger Logger { get; set; }

        public AuthService(TennisDbContext tennisDb, ITelegramBotClient botClient, ILogger<AuthService> logger, ILogService logService)
        {
            this.tennisDb = tennisDb;
            _botClient = botClient;
            Logger = logger;
            this.logService = logService;
        }

        public async Task<bool> OnStartAddId(long id, string? username)
        {
            var userByTelegramId = await tennisDb.Users.FirstOrDefaultAsync(_ => _.TelegramId == id);
            if (userByTelegramId == null)
            {
                try
                {
                    await tennisDb.Users.AddAsync(new User { TelegramId = id, FirstStartDate = DateTime.UtcNow, TelegramUsername = username });
                    Logger.LogInformation("OnStartAddId => Added userTelegramId to DB", id);
                    await tennisDb.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError("OnStartAddId => Couldn't add userTelegramId to DB", id);
                }
                
            }
            userByTelegramId.TelegramUsername = username != null ? username : userByTelegramId.TelegramUsername;

            return true;
        }

        public async Task<bool> Register(AuthRegisterRequest request, ValidatorResponse validatorResponse)
        {
            try
            {
                var user = await tennisDb.Users.FirstOrDefaultAsync(_ => _.TelegramId == request.TelegramId);
                if (user != null && (user.IsRegistered == false || user.IsRegistered == null))
                {
                    user.IsRegistered = true;
                }

                //костыль, пока /start не записывает в бд новых пользователей
                var isInDB = true;
                if (user == null)
                {
                    user = new User { TelegramId = request.TelegramId, FirstStartDate = DateTime.UtcNow, TelegramUsername = request.TelegramUsername };
                    user.FirstStartDate = DateTime.UtcNow;
                    isInDB = false;
                }
                // конец костыля
                DateTime birthday = new DateTime();
                if (request.Birthday != null)
                    if (DateTime.TryParseExact(request.Birthday, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                    {
                        birthday = parsedDate;
                        birthday = DateTime.SpecifyKind(birthday, DateTimeKind.Utc);
                    }

                var photo = await GetUserTelegramPhoto(request.TelegramId);

                if (user != null)
                {
                    if (!string.IsNullOrEmpty(request.PhoneNumber))
                    {
                        request.PhoneNumber = request.PhoneNumber.Replace("(", "");
                        request.PhoneNumber = request.PhoneNumber.Replace(")", "");
                        request.PhoneNumber = request.PhoneNumber.Replace(" ", "");
                        request.PhoneNumber = request.PhoneNumber.Replace("-", "");
                    }

                    user.Birthday = birthday;
                    user.Sex = request.Sex;
                    user.Occupation = request.Occupation;
                    user.TennisLevel = request.TennisLevel;
                    user.TelegramUsername = validatorResponse.TelegramUsername;
                    user.FirstName = request.FirstName;
                    user.LastName = request.LastName;
                    user.CityId = request.CityId;
                    user.CourtDictionaryId = request?.HomeCourtId;

                    user.HomeCourt = request?.HomeCourt;
                    user.HowOftenPlay = request.HowOftenPlay;
                    user.PhoneNumber = request.PhoneNumber;
                    user.Email = request.Email;
                    user.Photo = photo;
                    user.RegistrationDate = DateTime.UtcNow;
                    user.IsRegistered = true;
                    user.LastLogInDateTime = DateTime.UtcNow;

                    if (request.TennisLevel == null)
                        user.TennisLevel = 1;

                    // костыль описанные выше
                    if (isInDB == false)
                        await tennisDb.Users.AddAsync(user);


                    if (request.InterestIds == null || request.InterestIds.Count == 0)
                    {
                        await tennisDb.SaveChangesAsync();
                        await logService.AddLog(new Log
                        {
                            Time = DateTime.UtcNow,
                            Request = JsonConvert.SerializeObject(request),
                            Controller = "AuthController/Register",
                            Service = "Register",
                            LogLevel = LogLevel.Info,
                            UserId = user.Id,
                            Response = JsonConvert.SerializeObject(validatorResponse.Message) //init data
                        });
                        return true;
                    }
                    foreach (var interest in request.InterestIds)
                        await tennisDb.UserInterests.AddAsync(new UserInterest { InterestId = interest, UserId = user.Id });


                    // for beta test, delete after test
                    //   await tennisDb.Friendships.AddAsync(new Friendship { UserId = user.Id, FriendId = 49, Status = FriendshipStatus.Accepted, CreationTime =  DateTime.UtcNow, UpdateTime = DateTime.UtcNow });
                    // await tennisDb.Friendships.AddAsync(new Friendship { UserId = user.Id, FriendId = 47, Status = FriendshipStatus.Accepted, CreationTime =  DateTime.UtcNow, UpdateTime = DateTime.UtcNow });
                    // await tennisDb.SaveChangesAsync();
                    //

                    await logService.AddLog(new Log
                    {
                        Time = DateTime.UtcNow,
                        Request = JsonConvert.SerializeObject(request),
                        Controller = "AuthController/Register",
                        Service = "Register",
                        LogLevel = LogLevel.Info,
                        UserId = user.Id,
                        Response = JsonConvert.SerializeObject(validatorResponse.Message) // init data
                    });
                    await tennisDb.SaveChangesAsync();
                    return true;

                }
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "AuthController/Register",
                    Service = "Register",
                    LogLevel = LogLevel.Warn,
                    Message = $"User not found, telegramId = {request.TelegramId}",
                });
                return false;
            }
            catch(Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "AuthController/Register",
                    Service = "Register",
                    LogLevel = LogLevel.Error,
                    Message = ex.Message,
                });
                return false;
            }
            
        }

        public async Task<IsRegisteredResponse> IsRegistered (long telegramId, string initData)
        {
            var user = await tennisDb.Users.FirstOrDefaultAsync(_ => _.TelegramId ==  telegramId);
            if (user == null || user.IsRegistered == false)
                return new IsRegisteredResponse { IsRegistered = false };
            if (user.FirstName == null)
                return new IsRegisteredResponse { IsRegistered = false };
            
            user.LastLogInDateTime = DateTime.UtcNow;
            await tennisDb.SaveChangesAsync();
            await logService.AddLog(new Log
            {
                Time = DateTime.UtcNow,
                Request = JsonConvert.SerializeObject(telegramId),
                Controller = "AuthController/IsRegistered",
                Service = "IsRegistered",
                LogLevel = LogLevel.Info,
                Response = JsonConvert.SerializeObject(initData)

            });
            return new IsRegisteredResponse { IsRegistered = true };
            
        }

        public async Task<string> GetUserTelegramPhoto(long telegramId)
        {
            try
            {
                var photos = await _botClient.GetUserProfilePhotosAsync(telegramId);
                var photo = await _botClient.GetFileAsync(photos.Photos[0][0].FileId);

                //string fileUrl = $"https://api.telegram.org/file/bot{botClient.Token}/{filePath}";

                //// Download the photo and convert to Base64 string
                //using (var httpClient = new HttpClient())
                //using (var response = await httpClient.GetAsync(fileUrl))
                string base64String = "";
                using (var memoryStream = new MemoryStream())
                {
                    // Download the file directly into the MemoryStream
                    await _botClient.DownloadFileAsync(photo.FilePath, memoryStream);

                    // Reset the position of the MemoryStream to the beginning
                    memoryStream.Position = 0;

                    // Convert the MemoryStream to a byte array
                    byte[] bytes = memoryStream.ToArray();

                    // Encode the byte array as a Base64 string
                    base64String = Convert.ToBase64String(bytes);
                }
                return JsonConvert.SerializeObject(base64String);
            }
            catch(Exception ex)
            {
                return null;
            }

        }


        private async Task<bool> AddUserInterests(long userId,List<int>? interests)
        {
            if(interests == null || interests.Count == 0)
                return false;
            foreach (var interest in interests)
                await tennisDb.UserInterests.AddAsync(new UserInterest { InterestId = interest, UserId = userId });
            await tennisDb.SaveChangesAsync();
            return true;
        }


    }
}
