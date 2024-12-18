using Infrastructure.Data.Entities;
using Models.Dtos;

namespace Models.Models
{
    public class GetAvailableTimeSlotsResponse : ResponseBase
    {
       // public List<Schedule> TimeSlots { get; set; }
        public List<ScheduleDto> TimeSlots { get; set; }
    }
    public class TimeSlot
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
