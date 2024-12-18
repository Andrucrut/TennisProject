using Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos
{
    public class BaseGameInfo
    {
        public long? GameId { get; set; }
        public long? GameOrderId { get; set; }
        public long? BookingId { get; set; }
        public DateTime? Date { get; set; }
        public int? CourtOrganizationId { get; set; }
        public string? CourtOrganizationName { get; set; }
        public string? Metro { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        //     public long? ScheduleId { get; set; }
        public BookingStatus? BookingStatus { get; set; }
        public TimeOnly? StartTime { get; set; } // to do change to date time?? this coment is from Schedule.cs
        public TimeOnly? EndTime { get; set; }
        public double? Price { get; set; }
        public int? CourtId { get; set; }
        public int HowLong { get; set; }
        public int? CourtNumber { get; set; }
        public SurfaceType? SurfaceType { get; set; }
        public CourtType? CourtType { get; set; }
        public bool? HasAdditionalServices { get; set; }
        public double? CourtRating { get; set; }
        public GameStatus? GameStatus { get; set; }
        public int PlayersAmount { get; set; }
    }
}
