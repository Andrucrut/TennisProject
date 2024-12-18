namespace Models.Models
{
    public class FindUsersRequest : Paging.Paging
    {
        public long UserId { get; set; }
        public string? Query { get; set; }
    }
}
