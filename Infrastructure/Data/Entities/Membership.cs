namespace Infrastructure.Data.Entities
{
    public class Membership
    {
        public int Id { get; set; }
        public int AccessLevel { get; set; }
        public long? UserId { get; set; }
        public int? CourtOrganizationId { get; set; }
        public User? User { get; set; }
        public CourtOrganization? CourtOrganization { get; set; }

    }
}
