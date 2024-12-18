using Business.Mappings.Maps;
using Business.Services;
using Business.Services.Background;
using Business.Services.Notification;
using Business.Services.Scrapper;
using Infrastructure.Data;
using Interfaces.Interfaces;
using Interfaces.Interfaces.Scrapper;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using TennisProject;

var builder = WebApplication.CreateBuilder(args);

var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
builder.Services.Configure<BotConfiguration>(botConfigurationSection);

var botConfiguration = botConfigurationSection.Get<BotConfiguration>();

builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

builder.Services.AddHttpClient<IScrapperManagerService, ScrapperManagerService>().SetHandlerLifetime(TimeSpan.FromMinutes(5));
builder.Services.AddHttpClient<IAcquiringService, AcquiringService>().SetHandlerLifetime(TimeSpan.FromMinutes(5));

//builder.Services.AddScoped<UpdateHandlers>();

// Dummy business-logic service

builder.Services.AddScoped<InitDataValidator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourtService, CourtService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IBookingService, BookingService>();
//builder.Services.AddScoped<ITelegramNotificationService, TelegramNotificationService>();

//builder.Services.AddScoped<ILogService, ElasticSearchService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IBackgroundTaskService, BackgroundTaskService>();
builder.Services.AddScoped<IScrapperManagerService, ScrapperManagerService>();
builder.Services.AddScoped<IAcquiringService, AcquiringService>();
builder.Services.AddScoped<IBookingProcessingBotService, BookingProcessingBotService>();


builder.Services.AddScoped<INotificationService, NotificationService>();



//Mapping
builder.Services.AddScoped<FindUserMap>();
builder.Services.AddScoped<GameInfoMap>();
builder.Services.AddScoped<PlayedGameInfoMap>();
builder.Services.AddScoped<GameOrderUserInfoMap>();
builder.Services.AddScoped<FriendshipRequestsMap>();
builder.Services.AddScoped<CourtOrganizationMap>();
builder.Services.AddScoped<FriendMap>();
builder.Services.AddScoped<UserMap>();

// There are several strategies for completing asynchronous tasks during startup.
// Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
// We are going to use IHostedService to add and later remove Webhook


// telegram bot
//builder.Services.AddHostedService<ConfigureWebhook>();

// The Telegram.Bot library heavily depends on Newtonsoft.Json library to deserialize
// incoming webhook updates and send serialized responses back.
// Read more about adding Newtonsoft.Json to ASP.NET Core pipeline:
//   https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-6.0#add-newtonsoftjson-based-json-format-support
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddCors(options =>
{
    //options.AddPolicy("AllowOrigin",
    //    builder => builder
    //        .WithOrigins("https://8260-5-18-178-37.ngrok-free.app") // Замените на ваш домен фронтенда
    //        .AllowAnyHeader()
    //        .AllowAnyMethod());
    options.AddPolicy("AllowOrigin",
           builder => builder.AllowAnyOrigin() // Allow any origin
                             .AllowAnyHeader()
                             .AllowAnyMethod());
});

builder.Services.AddSwaggerGen();

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
    builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<TennisDbContext>(options =>
{
    options.UseNpgsql(connectionString,
        builder =>
        {
            builder.MigrationsAssembly(typeof(TennisDbContext).Assembly.FullName);
            builder.EnableRetryOnFailure();
        });
});

// Configure Serilog


builder.Services.AddCoreAdmin();

var app = builder.Build();


// Construct webhook route from the Route configuration parameter
// It is expected that BotController has single method accepting Update

// telegram bot
//app.MapBotWebhookRoute<BotController>(route: botConfiguration.Route);
app.UseCors("AllowOrigin");
//app.UseCors("AllowLocalDevelopment");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseStaticFiles();
app.MapDefaultControllerRoute();


app.Run();

namespace TennisProject
{
    public class BotConfiguration
#pragma warning restore RCS1110 // Declare type inside namespace.
#pragma warning restore CA1050 // Declare types in namespaces
    {
        public static readonly string Configuration = "BotConfiguration";

        public string BotToken { get; init; } = default!;
        public string HostAddress { get; init; } = default!;
        public string Route { get; init; } = default!;
        public string SecretToken { get; init; } = default!;
    }
}