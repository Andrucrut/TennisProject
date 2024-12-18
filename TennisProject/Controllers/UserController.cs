using Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Models.Models;
using Models.Models.Paging;

namespace TennisProject.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService userService { get; set; }

        public UserController(IUserService userService) 
        { 
            this.userService = userService;
        }
        [HttpPost("GetProfile")]
        public async Task<GetUserResponse> GetUser([FromBody] GetUserRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetUserResponse { Success = false, ExceptionMess = "InitData not validated" };
            request.TelegramId = validatorResponse.TelegramId;
            return await userService.GetUser(request);
        }

        [HttpPut("UpdateUserProfile")]
        public async Task<ResponseBase> UpdateUserProfile([FromBody] UpdateUserProfileRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new ResponseBase { Success = false, ExceptionMess = "InitData not validated" };
            
            if(request.AboutMe?.Length > 255)
                return new ResponseBase { Success = false, ExceptionMess = "Length of about me is more than 255 characters" };

            return await userService.UpdateUserProfile(request);
        }

        [HttpDelete("DeleteUserProfile")]
        public async Task<ResponseBase> DeleteUserProfile([FromBody] DeleteUserProfileRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (!validatorResponse.IsValidated)
                return new ResponseBase { Success = false, ExceptionMess = "InitData not validated" };

            return await userService.DeleteUserProfile(request.UserId);
        }

        [HttpPost("FindUsers")]
        public async Task<PagedList<FindUserDto>> FindUsers([FromBody] FindUsersRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            request.Calculate();
            return await userService.FindUsers(request);
        }

        [HttpPost("AllUsers")]
        public async Task<PagedList<FindUserDto>> AllUsers([FromBody] AllUsersRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            request.Calculate();
            return await userService.AllUsers(request);
        }



        private class GetAllUsersRequest()
        {
            public Paging Paging { get; set; }
        }

    }
}
