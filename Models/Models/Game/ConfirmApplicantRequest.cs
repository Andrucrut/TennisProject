namespace Models.Models.Game
{
    public class ConfirmApplicantRequest
    {
        public long GameId { get; set; }
        public long ApplicantId { get; set; }
        /// <summary>
        /// 0 - Confirm
        /// 1 - Deny
        /// </summary>
        public int Action { get; set; }
    }
}
