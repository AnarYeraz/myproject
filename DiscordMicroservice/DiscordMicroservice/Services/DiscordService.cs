using Discord;
using Discord.WebSocket;
using DiscordMicroservice.Models.Confidurations;
using DiscordMicroservice.Services.Interfaces;
using Microsoft.Extensions.Options;
using DiscordConfig = DiscordMicroservice.Models.Confidurations.DiscordConfig;

namespace DiscordMicroservice.Services
{
    public class DiscordService : IDiscordService
{
    private readonly DiscordSocketClient client;
    private readonly string botToken;
    private bool isReady = false;

    public DiscordService(IOptions<DiscordConfig> options)
    {
        botToken = options.Value.Token;
        client = new DiscordSocketClient();
        
        client.Ready += async () =>
        {
            isReady = true;
            Console.WriteLine("Бот готов к работе");
        };
    }

    public async Task<(bool Success, string? Message)> SendMessageAsync(string target, string message)
    {
        try
        {
            await client.LoginAsync(TokenType.Bot, botToken);
            await client.StartAsync();

            var waitTime = 30000; 
            var interval = 500; 
            while (!isReady && waitTime > 0)
            {
                await Task.Delay(interval);
                waitTime -= interval;
            }

            if (!isReady)
            {
                await client.StopAsync();
                return (false, "Бот не смог подключиться за отведенное время");
            }

            bool messageSent = false;

            if (target.StartsWith("channel:"))
            {
                ulong channelId = ulong.Parse(target.Replace("channel:", ""));
                var channel = client.GetChannel(channelId) as IMessageChannel;

                if (channel != null)
                {
                    await channel.SendMessageAsync(message);
                    messageSent = true;
                }
            }
            else if (target.StartsWith("user:"))
            {
                ulong userId = ulong.Parse(target.Replace("user:", ""));
                var user = client.GetUser(userId);

                if (user != null)
                {
                    var dmChannel = await user.CreateDMChannelAsync();
                    await dmChannel.SendMessageAsync(message);
                    messageSent = true;
                }
            }

            await client.StopAsync();
            
            return messageSent 
                ? (true, "Сообщение успешно отправлено") 
                : (false, "Не удалось найти ни канал, ни пользователя");
        }
        catch (Exception ex)
        {
            await client.StopAsync();
            return (false, $"Ошибка при отправке: {ex.Message}");
        }
    }
}
}
