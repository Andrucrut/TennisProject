using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Interfaces.Interfaces;

namespace Business.Services
{
    public class LogService :ILogService
    {
        private readonly TennisDbContext tennisDb;
        public LogService(TennisDbContext tennisDb)
        {
            this.tennisDb = tennisDb;
        }
        public async Task<bool> AddLog (Log log)
        {
            await tennisDb.Logs.AddAsync(log);
            await tennisDb.SaveChangesAsync();
            return true;
        }
    }
}
