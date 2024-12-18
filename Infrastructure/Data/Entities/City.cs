using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        [Column(TypeName = "jsonb")]
        public GeoData? GeoData { get; set; }
    }
}
