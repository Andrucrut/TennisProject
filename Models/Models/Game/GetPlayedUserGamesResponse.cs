using Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Game
{
    public class GetPlayedUserGamesResponse : ResponseBase
    {
        public List<PlayedGameInfoDto>? Games { get; set; }
    }
}
