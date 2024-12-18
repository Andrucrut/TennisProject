using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Payment
{
    public class CreatePaymentRequest
    {
        public string RedurectUrl { get; set; }
        public double Price { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
