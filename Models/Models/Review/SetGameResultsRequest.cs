using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Review
{
    public class SetGameResultsRequest
    {
        public long GameId { get; set; }
        public long UserId { get; set; }
        public List<long>? WinnerIds { get; set; }
        public List<SetsResults>? ResultsOfSets { get; set; }
    }

    public class SetsResults
    {
        public int SetNumber { get; set; }
        public int MyScore { get; set; }
        public int OpponentScore { get; set; }

    }
}
