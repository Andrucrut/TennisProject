using Models.Dtos;

namespace Models.Models
{
    public class GetFriendRequestsResponse : ResponseBase
    {
        public List<FriendshipRequestDto>? Friendships { get; set; }
    }
}
