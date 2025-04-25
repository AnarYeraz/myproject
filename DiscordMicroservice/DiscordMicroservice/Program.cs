using DiscordMicroservice.Models.Confidurations;
using DiscordMicroservice.Services;
using DiscordMicroservice.Services.Interfaces;
using EmailMicroservice.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddHostedService<RabbitConsumerService>();
builder.Services.AddSingleton<IDiscordService, DiscordService>();

// Connfigurations
builder.Services.Configure<RabbitConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<DiscordConfig>(builder.Configuration.GetSection("Discord"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
