using Models.Dtos;

namespace Models.Models.Game
{
    public class GetUserGamesResponse : ResponseBase
    {
        public List<GameInfoDto>? Games { get; set; }
    }


}