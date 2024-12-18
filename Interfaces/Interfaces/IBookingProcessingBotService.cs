using Infrastructure.Data.Entities;
using Models.Models;

namespace Interfaces.Interfaces
{
    public interface IBookingProcessingBotService
    {
        public Task<string> ProcessBookingResultAsync(ProcessBookingResultRequest request);
        public Task<string> SendBookingRequestToBot(long bookingId/*Booking booking*/);
    }
}