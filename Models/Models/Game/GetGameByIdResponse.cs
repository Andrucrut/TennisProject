using Infrastructure.Data.Entities;

namespace Models.Models.Game
{
    public class GetGameByIdResponse : ResponseBase
    {
        public Infrastructure.Data.Entities.Game? Game { get; set; }
        public List<GameOrder>? GameOrders { get; set; }
    }
}
