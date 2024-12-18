

namespace Infrastructure.Data.Entities
{
    public class CourtReview
    {
        public long? Id { get; set; }
        public int? CourtOrganizationId { get; set; }
        public CourtOrganization? CourtOrganization { get; set; }
        public long? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public long? GameId { get; set; }
        public Game? Game { get; set; }
        public int Rating { get; set; }
        public List<CourtDisappointmentEnum>? Disappointments { get; set; }
        public List<CourtSatisfactionEnum>? Satisfactions { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }


    }

    public enum CourtDisappointmentEnum : int
    {

    }

    public enum CourtSatisfactionEnum : int
    {

    }
}
