
namespace Infrastructure.Data.Entities
{
    public class Friendship
    {
        public long Id { get; set; }
        /// <summary>
        /// Идентификатор пользователя, который инициировал дружбу
        /// </summary>
        public long? UserId { get; set; }
        public long? FriendId { get; set; }
        public User? User { get; set; }
        public User? Friend { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
    public enum FriendshipStatus : int
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2
    }
}
