using NotificationStatusMicroserviceervice.Models.Enums;

namespace NotificationStatusMicroserviceervice.Models
{
    public class NotificationRequest
    {
        public string Type { get; set; } = string.Empty;
        public StatusSending Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
