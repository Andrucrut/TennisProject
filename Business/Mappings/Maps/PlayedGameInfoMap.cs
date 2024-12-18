using Business.Mappings.Base;
using Infrastructure.Data.Entities;
using Models.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Mappings.Maps
{
    public class PlayedGameInfoMap : MapBase<GameOrder, PlayedGameInfoDto, MapOptions>
    {
        public override void MapCore(GameOrder source, GameOrder destination, MapOptions options = null)
        {
            throw new NotImplementedException();
        }

        public override PlayedGameInfoDto MapCore(GameOrder source, MapOptions options = default)
        {
            if (source == null)
                return null;

            options = options ?? new MapOptions();

            var result = new PlayedGameInfoDto();
            result.GameId = source.GameId;
            result.GameOrderId = source.Id;
            if (options.MapProperties)
            {
                result.Date = source.Booking?.Date;
                result.BookingId = source.BookingId;
                result.BookingStatus = source.Booking?.Status;
                result.Date = source.Booking?.Date;
                result.GameStatus = source.Game?.Status;


                var schedule = source.Booking.Schedules.FirstOrDefault();
                if (schedule.Court?.CourtOrganization?.Photo != null)
                    result.Photo = JsonConvert.DeserializeObject(schedule.Court?.CourtOrganization?.Photo).ToString();
                result.CourtOrganizationId = schedule.Court?.CourtOrganizationId;
                result.CourtOrganizationName = schedule.Court?.CourtOrganization?.Name;
                result.HasAdditionalServices = schedule.Court?.CourtOrganization?.HasAdditionalServices;
                result.Metro = schedule.Court?.CourtOrganization?.Metro;
                result.Address = schedule.Court?.CourtOrganization?.Address;
                result.StartTime = schedule.StartTime;
                result.EndTime = schedule.EndTime;
                result.HowLong = source.Booking.Schedules.Count()/2; //to do: считать не по кол-ву слотов а по кол-ву часов EndTime-StatTime
             //   result.Price = source.Booking.Schedules.Sum(_ => _.Price);
                result.CourtId = schedule.CourtId;
                result.CourtNumber = schedule.Court?.Number;
                result.SurfaceType = schedule.Court?.SurfaceType;
                result.CourtRating = schedule.Court?.Rating;
                result.CourtType = schedule.Court?.CourtType;
                result.PlayersAmount = (int)source.Game?.PlayersAmount;

            }
            return result;
        }

        public override GameOrder ReverseMapCore(PlayedGameInfoDto source, MapOptions options = default)
        {
            throw new NotImplementedException();
        }
    }
}
