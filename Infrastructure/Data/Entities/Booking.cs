namespace Infrastructure.Data.Entities
{
    public class Booking
    {
        public long Id { get; set; }
        /// <summary>
        /// Когда было произведено бронирование
        /// </summary>
        public DateTime BookingTime { get; set; }
        /// <summary>
        /// На какую даты был забронирован корт
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// статус
        /// </summary>
        public double Price { get; set; }
        public string? PaymentLink { get; set; }
        public string? PaymentId { get; set; }
        public long? OrderId { get; set; }
        public BookingStatus? Status { get; set; }
        public ICollection<Schedule> Schedules { get; } = new List<Schedule>();
    }

    public enum BookingStatus : int
    {
        Booked = 0,
        Cancelled = 1,
        Pending = 2,
        NotPayedCancelled = 3,
        NotPayed = 4,
        Refaunded = 5,
    }
}
