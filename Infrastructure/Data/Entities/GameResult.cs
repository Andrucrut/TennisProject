using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Entities
{
    public class GameResult
    {
        public long Id { get; set; }

        public long? GameId { get; set; }
        public Game? Game { get; set; }

        // кто внес результат
        public long? UserId { get; set; }

        public User? User { get; set; }
        //[Column(TypeName = "jsonb")]
        //public string? Results { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<GameScore> ScoreResults { get; } = new List<GameScore>();
        public ICollection<User> Winners { get; } = new List<User>();
    }
}
