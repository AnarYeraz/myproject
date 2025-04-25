using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using SMSMicroservice.Models.Configurations;
using SMSMicroservice.Services.Interfaces;

namespace SMSMicroservice.Services
{
    public class SMSService : ISmsSender
    {
        private readonly SmsSetting setting;
        public SMSService(IOptions<SmsSetting> options)
        {
            setting = options.Value;
        }
        public async Task<(bool Success, string? Message)> SendSMSAsync(string phone, string message)
        {
            using var client = new HttpClient();
            var url = $"https://sms.ru/sms/send?api_id={setting.ApiKey}&to={phone}&msg={Uri.EscapeDataString(message)}&json=1";

            try
            {
                var response = await client.GetStringAsync(url);
                Console.WriteLine($"[SMS Service] API Response: {response}");

                return (true, "Смс отправлено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SMS Service] Error sending SMS: {ex.Message}");
                return (false, "Ошибка при отправке смс");
            }
        }
    }
}
