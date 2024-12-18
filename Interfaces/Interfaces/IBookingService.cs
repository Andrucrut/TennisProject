using Models.Models;

namespace Interfaces.Interfaces
{
    public interface IBookingService
    {
        public Task<BookResponse> Book(BookRequest request);
        public Task<ResponseBase> CancelBooking(long id);
     //   public Task<ResponseBase> CheckPaymentStatus();
    }
}
