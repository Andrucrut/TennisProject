using Infrastructure.Data.Entities;

namespace Models.Models.Review
{
    public class ReviewCourtRequest
    {
        public long? Id { get; set; }
        public int? CourtOrganizationId { get; set; }
        public long? ReviewerId { get; set; }
        public long? GameId { get; set; }
        public int Rating { get; set; }
        public List<CourtDisappointmentEnum>? Disappointments { get; set; }
        public List<CourtSatisfactionEnum>? Satisfactions { get; set; }
        public string? Comment { get; set; }
    }
}
