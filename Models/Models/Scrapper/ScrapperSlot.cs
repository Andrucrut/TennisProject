using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Scrapper
{
    public class ScrapperSlot
    {


        [JsonProperty("time_from")]
        public TimeSpan TimeFrom { get; set; }

        [JsonProperty("time_to")]
        public TimeSpan TimeTo { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
