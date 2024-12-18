namespace Models.Models;

public class ProcessBookingResultRequest
{
    public long BookingId { get; set; }
    public BookingAction Action { get; set; }

    public ProcessBookingResultRequest(long bookingId, BookingAction action)
    {
        BookingId = bookingId;
        Action = action;
    }
}