

namespace Infrastructure.Data.Entities
{
    public class Court
    {
        public int Id { get; set; }
        public int? CourtOrganizationId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public int? Go2SportId { get; set; }
        public SurfaceType SurfaceType { get; set; }
        public CourtType CourtType { get; set; }
        public CourtOrganization? CourtOrganization { get; set; }
        public double? Rating { get; set; }
    }

    public enum CourtType : int
    {
        Outdoor = 0,
        InDoor = 1
    }
    public enum SurfaceType : int
    {
        Grass = 0,
        Hard = 1,
        /// <summary>
        /// грунт
        /// </summary>
        Clay = 2,
    }
}
