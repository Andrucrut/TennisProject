using Models.Dtos;

namespace Models.Models
{
    public class GetCourtOrganizationByIdResponse : ResponseBase
    {
        public CourtOrganizationDto Organization { get; set; }
        public List<int> CourtIds { get; set; }

    }
}
