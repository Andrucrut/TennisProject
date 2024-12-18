using Models.Models.Scrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Interfaces.Scrapper
{
    public interface IScrapperManagerService
    {
        public Task<bool> UpdateSlots(GetScrapperCourtsRequest requestBody);

        public Task<ScrapperBookResponse> BookGo2Sport(BookGo2SportRequest request);

        public Task<GetScrapperPaymentStatusResponse> GetPaymentStatus(ScrapperPaymentRequest request);
        public Task<bool> RefoundPayment(ScrapperPaymentRequest request);
    }
}
