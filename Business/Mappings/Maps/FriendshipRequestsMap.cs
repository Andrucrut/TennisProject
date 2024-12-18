using Business.Helpers;
using Business.Mappings.Base;
using Infrastructure.Data.Entities;
using Models.Dtos;
using Newtonsoft.Json;

namespace Business.Mappings.Maps
{
    public class FriendshipRequestsMap : MapBase<Friendship, FriendshipRequestDto, MapOptions>
    {
        public override void MapCore(Friendship source, Friendship destination, MapOptions options = null)
        {
            throw new NotImplementedException();
        }

        public override FriendshipRequestDto MapCore(Friendship source, MapOptions options = null)
        {
            if (source == null)
                return null;

            options = options ?? new MapOptions();

            var result = new FriendshipRequestDto();
            result.UserId = source.UserId;
            if (options.MapProperties)
            {
                result.FirstName = source.User?.FirstName;
                result.LastName = source.User?.LastName;
                result.TelegramUsername = source.User?.TelegramUsername;
                result.TennisLevel = source.User?.TennisLevel;
                result.CityId = source.User?.CityId;
                result.CityName = source.User?.City?.Name;
                result.Age = AgeCounter.Count(source.User?.Birthday);
                if (source.User?.Photo != null)
                    result.Photo = JsonConvert.DeserializeObject(source.User?.Photo).ToString();
                else
                    result.Photo = null;

                result.AboutMe = source.User?.AboutMe;
                result.LastLogInDateTime = source.User?.LastLogInDateTime;
                result.HomeCourtName = source.User?.CourtDictionary?.Name;
                result.HomeCourtAddress = source.User?.CourtDictionary?.Address;
            }
            if (options.MapObjects)
            {
                // result.Tenant = mapContext.TenantMap.Map(source.Tenant, options);
                // result.Plan = mapContext.SubscribePlanMap.Map(source.Plan, options);
            }
            if (options.MapCollections)
            {
                //  result.Roles = mapContext.UserRoleMap.Map(source.Roles, options);
                //  result.Bonuses = mapContext.UserBonusMap.Map(source.Bonuses, options);
            }

            return result;
        }

        public override Friendship ReverseMapCore(FriendshipRequestDto source, MapOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
