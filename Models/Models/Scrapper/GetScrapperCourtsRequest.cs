using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Scrapper
{
    public class GetScrapperCourtsRequest
    {
        [JsonProperty("target_date")]
        public DateTime TargetDate { get; set; }
        [JsonProperty("club_id")]
        public int ClubId { get; set; }
    }
}
