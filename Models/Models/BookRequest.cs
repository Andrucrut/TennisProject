namespace Models.Models
{
    public class BookRequest
    {
        public List<long> ScheduleIds { get; set; }
        public long UserId {  get; set; }
        public DateTime Date { get; set; }
        public List<long>? InviteIds {  get; set; }
    }
}
