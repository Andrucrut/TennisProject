using Business.Mappings.Base;
using Infrastructure.Data.Entities;
using Models.Dtos;
using Newtonsoft.Json;
using Telegram.Bot.Requests.Abstractions;

namespace Business.Mappings.Maps
{
    public class GameInfoMap : MapBase<GameOrder, GameInfoDto, MapOptions>
    {


        public override GameInfoDto MapCore(GameOrder source, MapOptions options = null)
        {
            if (source == null)
                return null;

            options = options ?? new MapOptions();

            var result = new GameInfoDto();
            result.GameId = source.GameId;
            result.GameOrderId = source.Id;

            var moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            var currentTimeMoscow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);
            var currentMoscowTimeOnly = TimeOnly.FromDateTime(currentTimeMoscow);

            var timeNow = DateTime.Today.AddTicks(currentMoscowTimeOnly.Ticks);

            var startTimeOfTheGame = source.Game?.Date.Value;
            


            if (options.MapProperties)
            {
                result.Date = source.Booking?.Date;
                result.BookingId = source.BookingId;
                result.BookingStatus = source.Booking?.Status;
                result.Date = source.Booking?.Date;
                result.GameStatus = source.Game?.Status;
                

                var schedule = source.Booking.Schedules.FirstOrDefault();
                if(schedule.Court?.CourtOrganization?.Photo != null)
                    result.Photo = JsonConvert.DeserializeObject(schedule.Court?.CourtOrganization?.Photo).ToString();
                result.CourtOrganizationId = schedule.Court?.CourtOrganizationId;
                result.CourtOrganizationName = schedule.Court?.CourtOrganization?.Name;
                result.HasAdditionalServices = schedule.Court?.CourtOrganization?.HasAdditionalServices;
                result.Metro = schedule.Court?.CourtOrganization?.Metro;
                result.Address = schedule.Court?.CourtOrganization?.Address;
                result.StartTime = schedule.StartTime;
                result.EndTime = schedule.EndTime;
                result.HowLong = source.Booking.Schedules.Count()/2; //to do: считать не по кол-ву слотов а по кол-ву часов EndTime-StatTime
              //  result.Price = source.Booking.Schedules.Sum(_ => _.Price);
                result.CourtId = schedule.CourtId;
                result.CourtNumber = schedule.Court?.Number;
                result.SurfaceType = schedule.Court?.SurfaceType;
                result.CourtRating = schedule.Court?.Rating;
                result.CourtType = schedule.Court?.CourtType;
                result.PlayersAmount = (int)source.Game?.PlayersAmount;

                result.CancallationRule = (int)schedule.Court?.CourtOrganization?.CancallationRule;


                startTimeOfTheGame = startTimeOfTheGame.Value.Date.Add(result.StartTime.Value.ToTimeSpan());


                var timeDifference = (startTimeOfTheGame - timeNow);

                //Преобразуем разницу во времени в часы
                //result.LeftTimeTillTheGame = Math.Max(timeDifference.hou, 0);

                result.LeftTimeTillTheGame = (startTimeOfTheGame - timeNow).Value.Hours;


            }
            return result;
        }
        public override void MapCore(GameOrder source, GameOrder destination, MapOptions options = null)
        {
            throw new NotImplementedException();
        }

        public override GameOrder ReverseMapCore(GameInfoDto source, MapOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
