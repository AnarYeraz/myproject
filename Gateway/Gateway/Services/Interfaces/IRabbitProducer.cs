namespace Gateway.Services.Interfaces
{
    public interface IRabbitProducer
    {
        public (bool Sucess, string? Message) Publish();
    }
}
