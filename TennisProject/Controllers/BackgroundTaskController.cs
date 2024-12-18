using Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Models.Models;

namespace TennisProject.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class BackgroundTaskController : ControllerBase
    {
        private IBackgroundTaskService backgroundService;
        public BackgroundTaskController(IBackgroundTaskService backgroundService)
        {
            this.backgroundService = backgroundService;
        }

        [AllowAnonymous]
        [HttpGet("TestMethod")]
        public async Task<bool> TestMethod()
        {
            return true;
        }

        [AllowAnonymous]
        [HttpGet("ConvertGamesToPlayed")]
        public async Task<ResponseBase> ConvertGamesToPlayed()
        {
            return await backgroundService.ConvertGamesToPlayed();
        }

        [AllowAnonymous]
        [HttpGet("UpdateOccupiedSchedules")]
        public async Task<ResponseBase> UpdateOccupiedSchedules()
        {
            return await backgroundService.UpdateOccupiedSchedules();
        }

        [AllowAnonymous]
        [HttpGet("SendGameReminders")]
        public async Task<ResponseBase> SendGameReminders()
        {
            return await backgroundService.SendGameReminders();
        }

        [AllowAnonymous]
        [HttpGet("CheckPaymentStatuses")]
        public async Task<ResponseBase> CheckPaymentStatuses()
        {
            return await backgroundService.CheckPaymentStatus();
        }

        [AllowAnonymous]
        [HttpGet("CapturePayments")]
        public async Task<ResponseBase> CapturePayments()
        {
            return await backgroundService.CapturePayments();
        }
    }
}