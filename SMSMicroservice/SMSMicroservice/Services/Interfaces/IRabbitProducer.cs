using SMSMicroservice.Models;

namespace SMSMicroservice.Services.Interfaces
{
    public interface IRabbitProducer
    {
        public (bool Success, string? Message) Publish(NotificationLog request);
    }
}
