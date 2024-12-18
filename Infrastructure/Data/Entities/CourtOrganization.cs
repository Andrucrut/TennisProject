using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities
{
    public class CourtOrganization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string? Metro { get; set; }
        public string? WebSiteLink { get; set; }
        public string Address { get; set; }
        public string? Go2SportLink { get; set; }
        public int? Go2SportId { get; set; }
        public bool HasWidget { get; set; }
        public bool HasAdditionalServices { get; set; }

        [Column(TypeName = "jsonb")]
        public AdditionalServices AdditionalServices { get; set; }
        /// <summary>
        /// за сколько часов можно бесплатно отменить бронь
        /// </summary>
        public int? CancallationRule { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public City? City { get; set; }
        public District? District { get; set; }
        [Column(TypeName = "jsonb")]
        public string? Photo { get; set; }

        [Column(TypeName = "jsonb")]
        public GeoData? GeoData { get; set; }

        [Column(TypeName = "jsonb")]
        public List<string>? PhotoUrls { get; set; } = new List<string>();


        // TO DO: ADD RATING
    }

    public class GeoData
    {
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
    }
    
    public class AdditionalServices
    {
        public bool Parking { get; set; }
        public bool EquipmentRental { get; set; }
        public bool Trainer { get; set; }
        public bool Locker { get; set; }
        public bool Shower { get; set; }
    }
}
