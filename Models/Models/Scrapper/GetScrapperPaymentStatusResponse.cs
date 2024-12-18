using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Scrapper
{
    public class GetScrapperPaymentStatusResponse
    {
        public ScrapperPaymentStatus PaymentStatus { get; set; }
    }

    public enum ScrapperPaymentStatus
    {
        [EnumMember(Value = "pending")]
        Pending,
        [EnumMember(Value = "declined")]
        Declined,
        [EnumMember(Value = "accepted")]
        Accepted,
        [EnumMember(Value = "refunded")]
        Refunded,
        [EnumMember(Value = "unknown")]
        Unknown,

    }
   
}
