using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities
{
    public class Log
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public DateTime? Time { get; set; }
        [Column(TypeName = "jsonb")]
        public string? Request { get; set; }
        [Column(TypeName = "jsonb")]
        public string? Response { get; set; }
        public string? Controller { get; set; }
        public string? Service { get; set; }
        public LogLevel? LogLevel { get; set; }
        public string? Message { get; set; }
        public long? GameId { get; set; }
        public long? BookingId { get; set; }
        public Game? Game { get; set; }
        public Booking? Booking { get; set; }
        public User? User { get; set; }
    }

    public enum LogLevel : int
    {
        Info = 0,
        Error = 1,
        Warn = 2,
    }
    public enum LogState : int
    {
        Success = 0,
        Error = 1,
    }
}
