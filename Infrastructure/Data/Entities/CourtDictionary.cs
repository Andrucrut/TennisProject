namespace Infrastructure.Data.Entities
{
    public class CourtDictionary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CityId { get; set; }
        public City? City { get; set; }
        public string? Address { get; set; }
        public string? SiteLink { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
