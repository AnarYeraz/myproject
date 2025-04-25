using Microsoft.AspNetCore.Connections;
using System.Net.Mail;
using System.Net;
using EmailMicroservice.Models.Configurations;
using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using System.Text;
using EmailMicroservice.Models;
using EmailMicroservice.Services.Interfaces;

namespace EmailMicroservice.Services
{
    public class RabbitConsumerService : BackgroundService
    {
        private readonly RabbitConfiguration configuration;
        private IConnection connection;
        private IModel channel;
        private readonly IEmailService emailService;
        private readonly IRabbitProducer producer;

        public RabbitConsumerService(IOptions<RabbitConfiguration> config, IEmailService service, IRabbitProducer producer)
        {
            this.producer = producer;
            emailService = service;
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

            channel.ExchangeDeclare(exchange: configuration.Exchange, type: ExchangeType.Direct, durable: true);

            channel.QueueDeclare(queue: "email",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            channel.QueueBind(queue: "email", exchange: configuration.Exchange, routingKey: "email");
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
                    var resultSave = await emailService.SendEmailAsync(message.Target, message.Message);
                    if (!resultSave.Success)
                    {
                        NotificationLog log = new NotificationLog()
                        {
                            Type = message.Type,
                            Status = StatusSending.Success,
                            Message = "Сообщение отправлено успешно"
                        };
                        producer.Publish(log);
                        Console.WriteLine($"Ошибка при отправке: {resultSave.message}");
                    }

                    NotificationLog log2 = new NotificationLog()
                    {
                        Type = message.Type,
                        Status = StatusSending.Failed,
                        Message = "Ошибка при отправке сообщения"
                    };
                    producer.Publish(log2);
                }


                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "email", autoAck: false, consumer: consumer);

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
