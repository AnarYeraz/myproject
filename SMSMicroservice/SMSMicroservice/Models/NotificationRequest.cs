namespace SMSMicroservice.Models
{
    public class NotificationRequest
    {
        public string Type { get; set; }
        public string Target { get; set; }
        public string Message { get; set; }
    }
}
