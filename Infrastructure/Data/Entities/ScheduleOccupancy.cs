using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Entities
{
    public class ScheduleOccupancy
    {
        public long Id { get; set; }
        public long? ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }
        public DateTime Date { get; set; }
        public OccupancyReasons Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum OccupancyReasons : int
    {
        BookedByOurApp = 0,
        BookedByWidget = 1,
        BookedByCourt = 2,
        TechnicalReasons = 3,
        BookedByGo2Sport = 4,
        Other = 5
    }
}
