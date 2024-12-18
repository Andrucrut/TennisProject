using Infrastructure.Data.Entities;

namespace Models.Models
{
    public class AddScheduleRequest
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public WeeklyPrices Price { get; set; }

        public int CourtId { get; set; }
    }

    public class ScheduleLess
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public double Price { get; set; }
        public int CourtId { get; set; }
    }
}
