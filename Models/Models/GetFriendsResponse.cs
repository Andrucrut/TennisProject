using Models.Dtos;

namespace Models.Models
{
    public class GetFriendsResponse : ResponseBase
    {
        public List<FriendDto> Friends { get; set; }
    }
}
