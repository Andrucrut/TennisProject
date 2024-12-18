using Models.Models;
using Models.Models.Game;

namespace Interfaces.Interfaces
{
    public interface IGameService
    {
        public Task<ResponseBase> CreateGame(CreateGameRequest request);
        public Task<RespondToGameResponse> RespondToGame(RespondToGameRequest request);
        public Task<RespondToInvatationResponse> RespondToInvatation(RespondToGameRequest request);
        public Task<ResponseBase> ConfirmApplicant(ConfirmApplicantRequest request);
        public Task<GetUserGamesResponse> GetAllUserGames(GetUserGamesRequest request);
        public Task<GetUserGamesResponse> GetAllInvitedGames(GetUserGamesRequest request);
        public Task<GetUserGamesResponse> GetAllActualGames(GetUserGamesRequest request);
        public Task<GetUserGamesResponse> GetAllCanceledGames(GetUserGamesRequest request);
        public Task<GetPlayedUserGamesResponse> GetAllPlayedGames(GetUserGamesRequest request);
        public Task<GetGameByIdResponse> GetGameById(long gameId);
        public Task<ResponseBase> InviteToGame(InviteToGameRequest request);
        public Task<CancelGameResponse> CancelGame(CancelGameRequest request);
    }
}
