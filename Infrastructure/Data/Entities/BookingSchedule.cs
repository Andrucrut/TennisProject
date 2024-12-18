namespace Infrastructure.Data.Entities
{
    public class BookingSchedule
    {
        public long Id { get; set; }
        public long BookingId { get; set; }
        public long ScheduleId { get; set; }
    }
}
