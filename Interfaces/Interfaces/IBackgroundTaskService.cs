using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Interfaces
{
    public interface IBackgroundTaskService
    {
        public Task<ResponseBase> ConvertGamesToPlayed();
        public Task<ResponseBase> UpdateOccupiedSchedules();
        public Task<ResponseBase> SendGameReminders();
        public Task<ResponseBase> CheckPaymentStatus();
        public Task<ResponseBase> CapturePayments();
    }
}
