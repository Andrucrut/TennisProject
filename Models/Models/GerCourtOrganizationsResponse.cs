using Models.Dtos;
using Models.Models.Paging;

namespace Models.Models
{
    public class GerCourtOrganizationsResponse : ResponseBase
    {
        public PagedList<CourtOrganizationDto> CourtOrganizations { get; set; }
    }
}
