using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Scrapper
{
    public class ScrapperBookResponse
    {
        [JsonProperty("payment_link")]
        public string PaymentLink { get; set; }
        [JsonProperty("order_id")]
        public long OrderId { get; set; }
    }
}
