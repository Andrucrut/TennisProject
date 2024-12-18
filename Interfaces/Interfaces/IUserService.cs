using Models.Dtos;
using Models.Models;
using Models.Models.Paging;

namespace Interfaces.Interfaces;

public interface IUserService
{
    public Task<GetUserResponse> GetUser(GetUserRequest request);
    public Task<ResponseBase> UpdateUserProfile(UpdateUserProfileRequest request);
    public Task<PagedList<FindUserDto>> FindUsers(FindUsersRequest request);
    public Task<PagedList<FindUserDto>> AllUsers(AllUsersRequest request);

    public Task<ResponseBase> DeleteUserProfile(long requestUserId);
}