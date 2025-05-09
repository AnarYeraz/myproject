﻿using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SMSMicroservice.Models;
using SMSMicroservice.Models.Configurations;
using SMSMicroservice.Services.Interfaces;
using System.Text;

namespace SMSMicroservice.Services
{
    public class RabbitConsumerService : BackgroundService
    {
        private readonly RabbitConfiguration configuration;
        private IConnection connection;
        private IModel channel;
        private readonly ISmsSender smsService;
        private readonly IRabbitProducer producer;

        public RabbitConsumerService(IOptions<RabbitConfiguration> config, ISmsSender sender, IRabbitProducer producer)
        {
            this.producer = producer;
            smsService = sender;
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

            channel.QueueDeclare(queue: "sms",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            channel.QueueBind(queue: "sms", exchange: configuration.Exchange, routingKey: "sms");
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

                    //var resultSave = await smsService.SendSMSAsync(message.Target, message.Message);
                    //if (!resultSave.Success)
                    //{
                    //    Console.WriteLine($"Ошибка при отправке: {resultSave.Message}");
                    //}

                    Console.WriteLine($"TargetPhone: {message.Target}\tMessage: {message.Message}");
                    NotificationLog log = new NotificationLog()
                    {
                        Type = message.Type,
                        Status = StatusSending.Success,
                        Message = "Сообщение отправлено успешно"
                    };
                    producer.Publish(log);
                }
                NotificationLog log2 = new NotificationLog()
                {
                    Type = message.Type,
                    Status = StatusSending.Failed,
                    Message = "Ошибка при отправке сообщения"
                };

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "sms", autoAck: false, consumer: consumer);

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
