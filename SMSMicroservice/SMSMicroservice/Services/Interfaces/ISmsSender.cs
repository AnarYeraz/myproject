namespace SMSMicroservice.Services.Interfaces
{
    public interface ISmsSender
    {
        public Task<(bool Success, string? Message)> SendSMSAsync(string phone, string message);
    }
}
