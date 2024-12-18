using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Entities
{
    public class GameScore
    {
        public long Id { get; set; }
        public long? GameId { get; set; }
        public Game? Game { get; set; }

        // тот кто вносит счет
        public long? CreatorId { get; set; }
        public User? Creator { get; set; }

        public long? OpponentId { get; set; }
        public User? Opponent { get; set; }

        public int SetNumber { get; set; }
        public int CreatorScore { get; set; }
        public int OpponentScore { get; set; }
    }
}
