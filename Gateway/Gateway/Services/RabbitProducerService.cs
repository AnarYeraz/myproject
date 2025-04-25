using Gateway.Models;
using Gateway.Models.Configurations;
using Gateway.Services.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Gateway.Services
{
    public class RabbitProducerService : IRabbitProducer
    {
        private readonly ConnectionFactory factory;
        private readonly RabbitConfiguration configuration;
        public RabbitProducerService(IOptions<RabbitConfiguration> config)
        {
            configuration = config.Value;
            factory = new ConnectionFactory
            {
                HostName = configuration.HostName,
                Port = configuration.Port,
                UserName = configuration.UserName,
                Password = configuration.Password,
            };
        }
        public (bool Success, string? Message) Publish(NotificationRequest request)
        {
            try
            {
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: configuration.Exchange, type: ExchangeType.Direct, durable: true);

                var json = JsonSerializer.Serialize(request);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(
                    exchange: configuration.Exchange,
                    routingKey: request.Type,
                    basicProperties: properties,
                    body: body
                );
                return (true, "Сообщение отправлено");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (false, "Ошибка при отправке уведомления");
            }
        }
    }
}
