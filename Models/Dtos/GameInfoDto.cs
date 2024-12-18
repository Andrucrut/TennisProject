using Infrastructure.Data.Entities;

namespace Models.Dtos
{
    public class GameInfoDto : BaseGameInfo
    {
        public int CancallationRule { get; set; }
        public int LeftTimeTillTheGame { get; set; }
        public List<GameOrderUserInfoDto>? UsersInfo { get; set; }
    }
}
