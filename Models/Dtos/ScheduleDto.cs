using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.Entities;

namespace Models.Dtos
{
    public class ScheduleDto
    {
      //  public List<long> Ids { get; set; }

        public List<long> Ids { get; set; }
        public TimeOnly StartTime { get; set; } // to do change to date time
        public TimeOnly EndTime { get; set; }

        public double Price { get; set; }
        public int CourtId { get; set; }
        public SurfaceType? SurfaceType { get; set; }
        public CourtType? CourtType { get; set; }
        public int? CourtNumber { get; set; }
        public string? CourtName { get; set; }
  //      public CourtDto Court { get; set; }

    }

    public class CourtDto
    {
        public int CourtId { get; set; }
        public SurfaceType SurfaceType { get; set; }
        public CourtType CourtType { get; set; }
        public int CourtNumber { get; set; }
        public string? CourtName { get; set; }
    }
}
