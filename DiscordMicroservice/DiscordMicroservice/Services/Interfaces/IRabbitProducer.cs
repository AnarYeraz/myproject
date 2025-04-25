using DiscordMicroservice.Models;

namespace DiscordMicroservice.Services.Interfaces
{
    public interface IRabbitProducer
    {
        public (bool Success, string? Message) Publish(NotificationLog request);
    }
}
