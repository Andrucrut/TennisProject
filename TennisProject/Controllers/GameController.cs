using Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.Models.Game;

namespace TennisProject.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class GameController: ControllerBase
    {
        private IGameService gameService {  get; set; }
        public GameController(IGameService gameService)
        {
            this.gameService = gameService;
        }

        [AllowAnonymous]
        [HttpPost("CreateGame")]
        public async Task<ResponseBase> CreateGame(CreateGameRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new ResponseBase { Success = false, Message = "Validation error" };

            return await gameService.CreateGame(request);
        }


        [AllowAnonymous]
        [HttpPost("RespondToInvatation")]
        public async Task<RespondToInvatationResponse> RespondToInvatation(RespondToGameRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            // я добавил action где, 0 - отказаться, 1 - принять. 
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new RespondToInvatationResponse { Success = false, Message = "Validation error" };

            return await gameService.RespondToInvatation(request);
        }

        [AllowAnonymous]
        [HttpPost("RespondToGame")]
        public async Task<RespondToGameResponse> RespondToGame(RespondToGameRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            // TO DO: перепроверить и исправить
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new RespondToGameResponse { Success = false, Message = "Validation error" };

            return await gameService.RespondToGame(request);
        }

        [AllowAnonymous]
        [HttpPost("ConfirmApplicant")]
        public async Task<ResponseBase> ConfirmApplicant(ConfirmApplicantRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new ResponseBase { Success = false, Message = "Validation error" };

            return await gameService.ConfirmApplicant(request);
        }

        /// GetGames ниже нужно переделать!!! код повторяется!!

        [AllowAnonymous]
        [HttpPost("GetAllUserGames")]
        public async Task<GetUserGamesResponse> GetAllUserGames(GetUserGamesRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetUserGamesResponse { Success = false, Message = "Validation error" };

            return await gameService.GetAllUserGames(request);
        }

        [AllowAnonymous]
        [HttpPost("GetAllInvitedGames")]
        public async Task<GetUserGamesResponse> GetAllInvitedGames(GetUserGamesRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetUserGamesResponse { Success = false, Message = "Validation error" };

            return await gameService.GetAllInvitedGames(request);
        }

        [AllowAnonymous]
        [HttpPost("GetAllActualGames")]
        public async Task<GetUserGamesResponse> GetAllActualGames(GetUserGamesRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetUserGamesResponse { Success = false, Message = "Validation error" };

            return await gameService.GetAllActualGames(request);
        }

        [AllowAnonymous]
        [HttpPost("GetAllCanceledGames")]
        public async Task<GetUserGamesResponse> GetAllCanceledGames(GetUserGamesRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetUserGamesResponse { Success = false, Message = "Validation error" };

            return await gameService.GetAllCanceledGames(request);
        }

        [AllowAnonymous]
        [HttpPost("GetAllPlayedGames")]
        public async Task<GetPlayedUserGamesResponse> GetAllPlayedGames(GetUserGamesRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetPlayedUserGamesResponse { Success = false, Message = "Validation error" };

            return await gameService.GetAllPlayedGames(request);
        }



        [AllowAnonymous]
        [HttpPost("GetGameById")]
        public async Task<GetGameByIdResponse> GetGameById([FromBody] long gameId, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetGameByIdResponse { Success = false, Message = "Validation error" };

            return await gameService.GetGameById(gameId);
        }

        [AllowAnonymous]
        [HttpPost("InviteToGame")]
        public async Task<ResponseBase> InviteToGame(InviteToGameRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new ResponseBase { Success = false, Message = "Validation error" };

            return await gameService.InviteToGame(request);
        }

        [AllowAnonymous]
        [HttpPost("CancelGame")]
        public async Task<CancelGameResponse> CancelGame(CancelGameRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new CancelGameResponse { Success = false, Message = "Validation error" };

            return await gameService.CancelGame(request);
        }


    }
}
