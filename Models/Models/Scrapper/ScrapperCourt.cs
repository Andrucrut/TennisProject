using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Scrapper
{
    public class ScrapperCourt
    {
        [JsonProperty("court_name")]
        public string CourtName { get; set; }

        [JsonProperty("go2sport_court_id")]
        public int Go2SportCourtId { get; set; }

        [JsonProperty("slots")]
        public List<ScrapperSlot> Slots { get; set; }

        // ...
    }
}
