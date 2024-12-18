using Business.Mappings.Base;
using Infrastructure.Data.Entities;
using Models.Dtos;
using Newtonsoft.Json;

namespace Business.Mappings.Maps
{
    public class CourtOrganizationMap : MapBase<CourtOrganization, CourtOrganizationDto, MapOptions>
    {
        public override void MapCore(CourtOrganization source, CourtOrganization destination, MapOptions options = null)
        {
            throw new NotImplementedException();
        }

        public override CourtOrganizationDto MapCore(CourtOrganization source, MapOptions options = null)
        {
            if (source == null)
                return null;

            options = options ?? new MapOptions();

            var result = new CourtOrganizationDto();
            result.Id = source.Id;
            if (options.MapProperties)
            {
                result.Name = source.Name;
                result.Description = source.Description;
                result.PhoneNumber = source.PhoneNumber;
                result.Metro = source.Metro;
                result.WebSiteLink = source.WebSiteLink;
                result.Address = source.Address;
                result.CityId = source.CityId;
                result.CityName = source.City?.Name;
                result.DistrictId = source.DistrictId;
                result.DistricName = source.District?.Name;
            }
            if (source.Photo != null)
                result.Photo = JsonConvert.DeserializeObject(source.Photo).ToString();

            return result;
        }

        public override CourtOrganization ReverseMapCore(CourtOrganizationDto source, MapOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
