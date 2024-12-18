using Infrastructure.Data.Entities;

namespace Models.Models
{
    public class GetCourtByIdResponse : ResponseBase
    {
        public Court Court { get; set; }
    }
}
