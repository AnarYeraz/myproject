using Gateway.Models.Configurations;
using Gateway.Services.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Gateway.Services
{
    public class RabbitProducerService : IRabbitProducer
    {
        private readonly ConnectionFactory factory;
        public RabbitProducerService(IOptions<RabbitConfiguration> configuration)
        {
            var config = configuration.Value;
            factory = new ConnectionFactory
            {
                HostName = config.HostName,
                Port = config.Port,
                UserName = config.UserName,
                Password = config.Password,
            };
        }
        public (bool Sucess, string? Message) Publish()
        {
            throw new NotImplementedException();
        }
    }
}
