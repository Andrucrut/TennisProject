using Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;

namespace TennisProject.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class FriendshipController : ControllerBase
    {
        private IFriendshipService friendshipService { get; set; }
        public FriendshipController(IFriendshipService friendshipService)
        {
            this.friendshipService = friendshipService;
        }
        [AllowAnonymous]
        [HttpPost("Add")]
        public async Task<AddFriendResponse> Add([FromBody] AddFriendRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new AddFriendResponse { Success = false, Message = "Not validated" };

            return await friendshipService.Add(request);
        }

        [AllowAnonymous]
        [HttpDelete("Delete")]
        public async Task<DeleteFriendResponse> Delete([FromBody] DeleteFriendRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new DeleteFriendResponse { Success = false, Message = "Not validated" };

            return await friendshipService.Delete(request);
        }

        [AllowAnonymous]
        [HttpPost("GetById")]
        public async Task<GetFriendsResponse> GetById([FromBody] GetFriendsByIdRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            // TO DO RETURN PAGEDLIST
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetFriendsResponse { Success = false, Message = "Not validated" };
            if (request.Id == 0 || request.Id == null)
                request.TelegramId = (long)validatorResponse.TelegramId;
            return await friendshipService.GetFriendsById(request);
        }

        [AllowAnonymous]
        [HttpPost("GetFriendRequests")]
        public async Task<GetFriendRequestsResponse> GetFriendRequests([FromBody] UserBaseRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetFriendRequestsResponse { Success = false, Message = "Not validated" };


            request.TelegramId = validatorResponse.TelegramId;
            return await friendshipService.GetFriendRequests(request);
        }

        [AllowAnonymous]
        [HttpPost("Respond")]
        public async Task<RespondToFriendRequestResponse> Respond([FromBody] RespondToFriendRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new RespondToFriendRequestResponse { Success = false, Message = "Not validated" };

          
            return await friendshipService.Respond(request);
        }
    }
}
