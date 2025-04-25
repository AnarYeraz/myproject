using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using System.Text;
using NotificationStatusMicroserviceervice.Models.Configurations;
using NotificationStatusMicroserviceervice.Models;
using NotificationStatusMicroserviceervice.Data;
using NotificationStatusMicroserviceervice.Models.Entity;

namespace EmailMicroservice.Services
{
    public class RabbitConsumerService : BackgroundService
    {
        private readonly RabbitConfiguration configuration;
        private IConnection connection;
        private IModel channel;
        private readonly AppDbContext context;

        public RabbitConsumerService(IOptions<RabbitConfiguration> config, AppDbContext context)
        {
            this.context = context;
            configuration = config.Value;

            var factory = new ConnectionFactory
            {
                HostName = configuration.HostName,
                Port = configuration.Port,
                UserName = configuration.UserName,
                Password = configuration.Password
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: configuration.Exchange, type: ExchangeType.Topic, durable: true);

            channel.QueueDeclare(queue: "status_log_queue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            channel.QueueBind(queue: "status_log_queue", exchange: configuration.Exchange, routingKey: "#");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var message = System.Text.Json.JsonSerializer.Deserialize<NotificationRequest>(messageJson);
                if (message != null)
                {
                    NotificationLog log = new NotificationLog()
                    {
                        Id = Guid.NewGuid(),
                        Type = message.Type,
                        Status = message.Status,
                        Message = message.Message
                    };
                    await context.NotificationLogs.AddAsync(log);
                    await context.SaveChangesAsync();
                }


                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "status_log_queue", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            channel?.Close();
            connection?.Close();
            base.Dispose();
        }
    }
}
