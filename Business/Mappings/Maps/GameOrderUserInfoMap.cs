using Business.Helpers;
using Business.Mappings.Base;
using Infrastructure.Data.Entities;
using Models.Dtos;
using Newtonsoft.Json;

namespace Business.Mappings.Maps
{
    public class GameOrderUserInfoMap : MapBase<GameOrder, GameOrderUserInfoDto, MapOptions>
    {
        public override void MapCore(GameOrder source, GameOrder destination, MapOptions options = null)
        {
            throw new NotImplementedException();
        }

        public override GameOrderUserInfoDto MapCore(GameOrder source, MapOptions options = null)
        {
            if (source == null)
                return null;

            options = options ?? new MapOptions();

            var result = new GameOrderUserInfoDto();
            result.Id = source.Id;
            if (options.MapProperties)
            {
                result.UserId = source.UserId;
                result.TelegramUsername = source.User?.TelegramUsername;
                result.FirstName = source.User?.FirstName;
                result.LastName = source.User?.LastName;
                result.GameOrderStatus = source.Status;
                result.UserStatus = source.UserStatus;
                result.TennisLevel = source.User?.TennisLevel;
                result.CityId = source.User?.CityId;
                result.Age = AgeCounter.Count(source.User?.Birthday);

                if(source.User?.Photo != null)
                    result.Photo = JsonConvert.DeserializeObject(source.User?.Photo).ToString();
                else
                    result.Photo = null;
            }
            return result;
        }
        public override GameOrder ReverseMapCore(GameOrderUserInfoDto source, MapOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
