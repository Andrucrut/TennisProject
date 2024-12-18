using Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;

namespace TennisProject.Controllers
{

    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private IAuthService AuthService { get; set; }
        

        public AuthController(IAuthService authService)
        {
            this.AuthService = authService;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
       // [CheckInitData]
        public async Task<IActionResult> Register([FromBody] AuthRegisterRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            //var message = $"User registered successfully id = {user.Id}, username = {user.Username}";
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return Ok("not validated");


            request.TelegramId = (long)validatorResponse.TelegramId;
            validatorResponse.Message = initData;
            var response = await AuthService.Register(request, validatorResponse);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("InitDataValidate")]
        public async Task<ValidatorResponse> InitDataValidate([FromBody] string initData, [FromServices] InitDataValidator initDataValidator)
        {
            return await initDataValidator.Validate(initData);
        }

        [AllowAnonymous]
        [HttpGet("IsRegistered")]
        public async Task<IsRegisteredResponse> IsRegistered([FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new IsRegisteredResponse { Success = false, Message = "Not validated" };
            return await AuthService.IsRegistered((long)validatorResponse.TelegramId, initData);
        }

        //[AllowAnonymous]
        //[HttpGet("GetUserTelegramPhoto")]
        //public async Task<string> GetUserTelegramPhoto([FromServices] InitDataValidator initDataValidator)
        //{
        //    var initData = Request.Headers["InitData"];
        //    var validatorResponse = await initDataValidator.Validate(initData);
        //   // if (validatorResponse.IsValidated == false)
        //       // return new IsRegisteredResponse { Success = false, Message = "Not validated" };
        //    return await AuthService.GetUserTelegramPhoto((long)validatorResponse.TelegramId);
        //}
    }

    //public class User
    //{
    //    public int? Id { get; set; }
    //    public string? Username { get; set; }
    //    public string? Password { get; set; }
    //    public string? InitData { get; set; }
    //}
}
