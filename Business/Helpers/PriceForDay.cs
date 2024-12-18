using Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers
{
    public static class PriceForDay
    {
        public static double GetPriceForDay(WeeklyPrices prices, DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => prices.Monday,
                DayOfWeek.Tuesday => prices.Tuesday,
                DayOfWeek.Wednesday => prices.Wednesday,
                DayOfWeek.Thursday => prices.Thursday,
                DayOfWeek.Friday => prices.Friday,
                DayOfWeek.Saturday => prices.Saturday,
                DayOfWeek.Sunday => prices.Sunday,
                _ => throw new ArgumentException("Invalid day of week", nameof(day))
            };
        }
    }
}
