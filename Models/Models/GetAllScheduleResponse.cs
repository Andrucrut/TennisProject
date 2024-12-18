using Infrastructure.Data.Entities;

namespace Models.Models
{
    public class GetAllScheduleResponse : ResponseBase
    {
        public List<Schedule>? Schedules { get; set; }
    }
}
