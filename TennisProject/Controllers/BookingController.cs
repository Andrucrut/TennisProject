using Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;

namespace TennisProject.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private IBookingService bookingService {  get; set; }
        public BookingController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [AllowAnonymous]
        [HttpPost("Book")]
        public async Task<BookResponse> Book(BookRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new BookResponse { Success = false, Message = "Validation error" };


            return await bookingService.Book(request);
        }

        [AllowAnonymous]
        [HttpDelete("CancelBooking")]
        public async Task<ResponseBase> CancelBooking(long id, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new BookResponse { Success = false, Message = "Validation error" };


            return await bookingService.CancelBooking(id);
        }
    }
}
