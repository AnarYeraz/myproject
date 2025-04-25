namespace SMSMicroservice.Models.Configurations
{
    public class RabbitConfiguration
    {
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
    }
}
