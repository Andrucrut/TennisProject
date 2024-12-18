using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities
{
    public class Schedule
    {
        public long Id { get; set; }
        public TimeOnly StartTime { get; set; } // to do change to date time
        public TimeOnly EndTime { get; set; }

        [Column(TypeName = "jsonb")]
        public WeeklyPrices PriceJson { get; set; }
        public int? CourtId { get; set; }
        public Court? Court { get; set; }
        public ICollection<Booking> Bookings { get; } = new List<Booking>();
    }

    public class WeeklyPrices
    {
        public double Monday { get; set; }
        public double Tuesday { get; set; }
        public double Wednesday { get; set; }
        public double Thursday { get; set; }
        public double Friday { get; set; }
        public double Saturday { get; set; }
        public double Sunday { get; set; }
    }
}
