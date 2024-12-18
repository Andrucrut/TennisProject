using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Business.Services;
using Interfaces.Interfaces;
using Models.Models;

namespace TennisProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingProcessingBotController : ControllerBase
    {
        private readonly IBookingProcessingBotService _bookingProcessingBotService;

        public BookingProcessingBotController(IBookingProcessingBotService bookingProcessingBotService)
        {
            _bookingProcessingBotService = bookingProcessingBotService;
        }

        [HttpPost("Result")]
        public async Task<IActionResult> ProcessBookingResult([FromBody] BookingResultRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request.");

            var result = await _bookingProcessingBotService.ProcessBookingResultAsync(new ProcessBookingResultRequest(request.BookingId, request.Action));

            if (result == "Booking not found.")
                return NotFound(result);

            if (result == "Invalid action.")
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Tesst")]
        public async Task<IActionResult> SendBookingRequestToBot([FromBody]long bookingId/*Booking booking*/)
        {
            var result = await _bookingProcessingBotService.SendBookingRequestToBot(bookingId);
            return Ok(result);
        }
    }
    

    public class BookingResultRequest
    {
        public long BookingId { get; set; }
        public BookingAction Action { get; set; }
    }
}