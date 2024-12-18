using Business.Helpers;
using Business.Mappings.Base;
using Infrastructure.Data.Entities;
using Models.Dtos;
using Newtonsoft.Json;

namespace Business.Mappings.Maps
{
    public class FriendMap : MapBase<User, FriendDto, MapOptions>
    {
        public override void MapCore(User source, User destination, MapOptions options = null)
        {
            throw new NotImplementedException();
        }

        public override FriendDto MapCore(User source, MapOptions options = null)
        {
            if (source == null)
                return null;

            options = options ?? new MapOptions();

            var result = new FriendDto();
            result.UserId = source.Id;
            if (options.MapProperties)
            {
                result.TelegramId = source.TelegramId;
                result.FirstName = source.FirstName;
                result.LastName = source.LastName;
                result.Age = AgeCounter.Count(source.Birthday);
                result.TelegramUsername = source.TelegramUsername;
                result.TennisLevel = source.TennisLevel;
                result.CityId = source.CityId;
                result.CityName = source.City?.Name;
                result.IsFriend = true;
                if(source.Photo != null)
                    result.Photo = JsonConvert.DeserializeObject(source.Photo).ToString();
                else 
                    result.Photo = null;

                result.AboutMe = source.AboutMe;
                result.LastLogInDateTime = source.LastLogInDateTime;
                result.HomeCourtName = source.CourtDictionary?.Name;
                result.HomeCourtAddress = source.CourtDictionary?.Address;
            }

            return result;
        }
        public override User ReverseMapCore(FriendDto source, MapOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
