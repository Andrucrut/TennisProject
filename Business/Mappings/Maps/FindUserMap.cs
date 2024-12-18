using Business.Helpers;
using Business.Mappings.Base;
using Infrastructure.Data.Entities;
using Models.Dtos;
using Newtonsoft.Json;

namespace Business.Mappings.Maps
{
    public class FindUserMap : MapBase<User, FindUserDto, MapOptions>
    {
        public override void MapCore(User source, User destination, MapOptions options = null)
        {
            if (source == null || destination == null)
                return;
            throw new NotImplementedException();
        }

        public override FindUserDto MapCore(User source, MapOptions options = null)
        {
            if (source == null)
                return null;

            options = options ?? new MapOptions();

            var result = new FindUserDto();
            result.Id = source.Id;
            if (options.MapProperties)
            {
                result.FirstName = source.FirstName;
                result.LastName = source.LastName;

                result.CityId = source.CityId;
                result.CityName = source.City?.Name;
                result.TelegramUsername = source.TelegramUsername;
                result.Age = AgeCounter.Count(source.Birthday);
                result.Sex = source.Sex;
                if (source.Photo != null)
                    result.Photo = JsonConvert.DeserializeObject(source.Photo).ToString();
                else
                    result.Photo = null;
                result.TennisLevel = source.TennisLevel;
                result.Email = source.Email;
                result.PhoneNumber = source.PhoneNumber;
                result.AboutMe = source.AboutMe;
                result.LastLogInDateTime = source.LastLogInDateTime;
                result.HomeCourtName = source.CourtDictionary?.Name;
                result.HomeCourtAddress = source.CourtDictionary?.Address;
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

        public override User ReverseMapCore(FindUserDto source, MapOptions options = null)
        {
            if (source == null)
                return null;

            options = options ?? new MapOptions();

            var result = new User();
            result.Id = source.Id;

            if (options.MapProperties)
            {
                result.FirstName = source.FirstName;
                result.LastName = source.LastName;

                result.CityId = source.CityId;
                result.TelegramUsername = source.TelegramUsername;
                // result.Birthday = source.Birthday;
                result.Sex = source.Sex;
                result.Photo = source.Photo;
                result.TennisLevel = source.TennisLevel;
                result.Email = source.Email;
                result.PhoneNumber = source.PhoneNumber;
            }
            return result;
        }
    }
}
