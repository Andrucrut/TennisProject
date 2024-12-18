namespace Models.Models.Game
{
    public class CreateGameRequest
    {
        public long? UserId { get; set; }
        /// <summary>
        /// Json like, List of TimeOnly
        /// for ex:
        /// {
        ///     [10:00, 11:00, 20:00]
        /// }
        /// </summary>
        public List<TimeOnly>? TimeSlots { get; set; }
        public DateTime? Date { get; set; }
        public int? WhoPays { get; set; }
        public int? Expectations { get; set; }
        public double? GameLevelMin { get; set; }
        public double? GameLevelMax { get; set; }
        public string? Comment { get; set; }
        public int? Type { set; get; } // opend, not opend        
        public int? CourtOrganizationId { get; set; }
        public List<long>? InviteIds { get; set; }
    }
}
