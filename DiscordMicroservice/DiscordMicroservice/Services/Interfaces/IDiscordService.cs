namespace DiscordMicroservice.Services.Interfaces
{
    public interface IDiscordService
    {
        public Task<(bool Success, string? Message)> SendMessageAsync(string target, string message);
    }
}
