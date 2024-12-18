using Models.Dtos;

namespace Models.Models
{
    public class GetCourtDictionaryByCityResponse : ResponseBase
    {
        public List<CourtDictInfo>? CourtDicts { get; set; }
    }
}
