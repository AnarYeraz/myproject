using NotificationStatusMicroserviceervice.Models.Enums;

namespace NotificationStatusMicroserviceervice.Models.Entity
{
    public class NotificationLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;
        public StatusSending Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
