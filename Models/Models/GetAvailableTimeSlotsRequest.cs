namespace Models.Models
{
    public class GetAvailableTimeSlotsRequest
    {
        public List<int>? CourtsIds { get; set; }
        public DateTime Date { get; set; }
      //  public List<DateTime> Dates { get; set; }
        public List<int>? OrganizationIds { get; set; }
    }
}
