using Infrastructure.Data.Entities;
using Models.Models.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos
{
    public class PlayedGameInfoDto : BaseGameInfo
    {
        public bool IsReviewed { get; set; }
        public bool IsWinnerChosen { get; set; }
        public bool IsScoreEntered { get; set; }
        public int? Rating { get; set; }

        public List<CourtDisappointmentEnum>? Disappointments { get; set; }
        public List<CourtSatisfactionEnum>? Satisfactions { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<GameOrderUserInfoDto>? UsersInfo { get; set; }

        public List<long> WinnerIds { get; set; }
        public List<ScoreResult> ScoreResults { get; set; }

       // public List<PlayedGameOrderUserInfoDto>? UsersInfo { get; set; }

    }

    public class ScoreResult
    {
        public int SetNumber { get; set; }
        public int UserScore {get;set;}
        public int OpponentScore { get; set; }
    }
}
