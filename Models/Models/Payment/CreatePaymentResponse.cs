using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Payment
{
    public class CreatePaymentResponse : ResponseBase
    {
        public string PaymentLink { get; set; }
        public string PaymentId { get; set; }
    }
}
