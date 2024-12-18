using Microsoft.EntityFrameworkCore;
using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Infrastructure.Data
{
    public class TennisDbContext : DbContext
    {
        public DbSet<User>? Users { get; set; }
        public DbSet<CourtOrganization>? CourtOrganizations { get; set; }
        public DbSet<Court>? Courts { get; set; }
        public DbSet<Membership>? Memberships { get; set; }
        public DbSet<City>? Cities { get; set; }
        public DbSet<District>? Districts { get; set; }
        public DbSet<Game>? Games { get; set; }
        public DbSet<GameOrder>? GameOrders { get; set; }
        public DbSet<GameHistory>? GamesHistory { get; set; }
        public DbSet<Friendship>? Friendships { get; set; }
        public DbSet<Schedule>? Schedules { get; set; }
        public DbSet<Booking>? Bookings { get; set; }
        public DbSet<UserBooking>? UserBookings { get; set; }
        public DbSet<Interest>? Interests { get; set; }
        public DbSet<UserInterest>? UserInterests { get; set; }
        public DbSet<Log>? Logs { get; set; }
        public DbSet<CourtDictionary>? CourtDictionaries { get; set; }
        public DbSet<CourtReview>? CourtReviews { get; set; }
        public DbSet<UserReview>? UserReviews { get; set; }
        public DbSet<Notification>? Notifications { get; set; }
        public DbSet<ScheduleOccupancy>? ScheduleOccupancies { get; set; }
        public DbSet<GameResult>? GameResults { get; set; }
        public DbSet<GameScore> GameScores { get; set; }


        public TennisDbContext(DbContextOptions<TennisDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Schedules)
                .WithMany(s => s.Bookings).UsingEntity<BookingSchedule>();

            modelBuilder.Entity<GameResult>()
                .HasMany(g => g.Winners).WithMany().UsingEntity<GameUserResult>();
            modelBuilder.Entity<GameResult>()
                .HasMany(g => g.ScoreResults).WithMany().UsingEntity<GameScoreResult>();


            



            var weeklyPricesConverter = new ValueConverter<WeeklyPrices, string>(
                v => JsonConvert.SerializeObject(v), // Преобразование объекта в JSON строку при записи в базу
                v => JsonConvert.DeserializeObject<WeeklyPrices>(v)); // Преобразование JSON строки обратно в объект при чтении

            modelBuilder.Entity<Schedule>()
                .Property(e => e.PriceJson)
                .HasConversion(weeklyPricesConverter) // Используем конвертер
                .HasColumnType("jsonb"); // Указываем, что это jsonb поле в базе

            var geoDataConverter = new ValueConverter<GeoData, string>(
                v => JsonConvert.SerializeObject(v), // Преобразование объекта в JSON строку при записи в базу
                v => JsonConvert.DeserializeObject<GeoData>(v)); // Преобразование JSON строки обратно в объект при чтении

            modelBuilder.Entity<CourtOrganization>()
                .Property(e => e.GeoData)
                .HasConversion(geoDataConverter)
                .HasColumnType("jsonb");

            modelBuilder.Entity<City>()
                .Property(e => e.GeoData)
                .HasConversion(geoDataConverter)
                .HasColumnType("jsonb");

            var additionalServiceConverter = new ValueConverter<AdditionalServices, string>(
                v => JsonConvert.SerializeObject(v), // Преобразование объекта в JSON строку при записи в базу
                v => JsonConvert.DeserializeObject<AdditionalServices>(v)); // Преобразование JSON строки обратно в объект при чтении

            modelBuilder.Entity<CourtOrganization>()
                .Property(e => e.AdditionalServices)
                .HasConversion(additionalServiceConverter)
                .HasColumnType("jsonb");

            modelBuilder.Entity<CourtOrganization>()
                .Property(e => e.PhotoUrls)
                .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v))
                .HasColumnType("jsonb");
        }



    }
}
