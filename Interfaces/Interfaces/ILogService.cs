using Infrastructure.Data.Entities;

namespace Interfaces.Interfaces
{
    public interface ILogService
    {
        public Task<bool> AddLog(Log log);
    }
}
