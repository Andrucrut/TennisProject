using Models.Dtos;

namespace Models.Models
{
    public class GetUserResponse : ResponseBase
    {
        public UserDto User { get; set; }
    }
}
