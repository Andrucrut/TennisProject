namespace Infrastructure.Data.Entities
{
    public class UserBooking
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public User? User { get; set; }
        public long? BookingId { get; set; }
        public Booking? Booking { get; set; }
    }
}
