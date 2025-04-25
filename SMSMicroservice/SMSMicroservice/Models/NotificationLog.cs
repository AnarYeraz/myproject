namespace SMSMicroservice.Models
{
    public class NotificationLog
    {
        public string Type { get; set; } = string.Empty;
        public StatusSending Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public enum StatusSending
    {
        Success,
        Failed
    }
}
