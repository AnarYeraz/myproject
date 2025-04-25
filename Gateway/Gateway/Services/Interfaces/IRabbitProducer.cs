using Gateway.Models;

namespace Gateway.Services.Interfaces
{
    public interface IRabbitProducer
    {
        public (bool Success, string? Message) Publish(NotificationRequest request);
    }
}
