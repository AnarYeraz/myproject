using EmailMicroservice.Models;

namespace EmailMicroservice.Services.Interfaces
{
    public interface IRabbitProducer
    {
        public (bool Success, string? Message) Publish(NotificationLog request);
    }
}
