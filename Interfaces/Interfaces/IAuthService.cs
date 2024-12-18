using Models.Models;

namespace Interfaces.Interfaces
{
    public interface IAuthService
    {
        public Task<bool> OnStartAddId(long id, string? username);
        public Task<bool> Register(AuthRegisterRequest request, ValidatorResponse validatorResponse);
        public Task<IsRegisteredResponse> IsRegistered(long telegramId, string initData);
        public Task<string> GetUserTelegramPhoto(long telegramId);
    }
}
