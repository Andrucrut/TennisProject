namespace Models.Models
{
    public class ValidatorResponse
    {
        public bool IsValidated { get; set; }
        public long? TelegramId { get; set; }
        public string? TelegramUsername { get; set; }
        public string? Message { get; set; }
    }
}
