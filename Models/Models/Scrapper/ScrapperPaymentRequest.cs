using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Models.Models.Scrapper
{
    public class ScrapperPaymentRequest
    {
        [JsonProperty("order_go2sport_id")]
        public long OrderId { get; set; }
    }
}
