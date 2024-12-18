using Models.Models;

namespace Interfaces.Interfaces
{
    public interface IFriendshipService
    {
        public Task<AddFriendResponse> Add(AddFriendRequest request);
        public Task<DeleteFriendResponse> Delete(DeleteFriendRequest request);
        public Task<GetFriendsResponse> GetFriendsById(GetFriendsByIdRequest request);
        public Task<RespondToFriendRequestResponse> Respond(RespondToFriendRequest request);
        public Task<GetFriendRequestsResponse> GetFriendRequests(UserBaseRequest request);
    }
}
