namespace Models.Dtos
{
    public class CourtOrganizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string? Metro { get; set; }
        public string? WebSiteLink { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public string? CityName { get; set; }
        public string? DistricName { get; set; }
        public int CourtsAmount { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? Photo { get; set; }

    }
}
