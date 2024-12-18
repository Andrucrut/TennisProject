using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Scrapper
{
    public class ScrapperCourtResponse
    {
        [JsonProperty("courts")]
        public List<ScrapperCourt> Courts { get; set; }

        public ScrapperCourtResponse()
        {
            Courts = new List<ScrapperCourt>();
        }

        // ...
    }
}
