using Models.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Interfaces
{
    public interface IAcquiringService
    {
        public Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request);
        public Task<GetPaymentStatusResponse> GetPaymentStatus(GetPaymentStatusRequest request);
        public Task<CancelPaymentResponse> CancelPayment(RefundPaymentRequest request);
        public Task<GetPaymentStatusResponse> CapturePayment(GetPaymentStatusRequest request);
    }
}
