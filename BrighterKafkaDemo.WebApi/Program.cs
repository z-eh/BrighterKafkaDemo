using BrighterKafkaDemo.Common.Configuration;
using BrighterKafkaDemo.Common.Ports.Events;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.Kafka;
using Polly;
using Polly.Registry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var retryPolicy = Policy
    .Handle<Exception>()
    .WaitAndRetry(new[]
    {
        TimeSpan.FromMilliseconds(50),
        TimeSpan.FromMilliseconds(100),
        TimeSpan.FromMilliseconds(150)
    });
var circuitBreakerPolicy = Policy
    .Handle<Exception>()
    .CircuitBreaker(1, TimeSpan.FromMilliseconds(500));
var retryPolicyAsync = Policy
    .Handle<Exception>()
    .WaitAndRetryAsync(new[]
    {
        TimeSpan.FromMilliseconds(50),
        TimeSpan.FromMilliseconds(100),
        TimeSpan.FromMilliseconds(150)
    });
;
var circuitBreakerPolicyAsync = Policy
    .Handle<Exception>()
    .CircuitBreakerAsync(1, TimeSpan.FromMilliseconds(500));
var policyRegistry = new PolicyRegistry
{
    { CommandProcessor.RETRYPOLICY, retryPolicy },
    { CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy },
    { CommandProcessor.RETRYPOLICYASYNC, retryPolicyAsync },
    { CommandProcessor.CIRCUITBREAKERASYNC, circuitBreakerPolicyAsync }
};

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false)
    .AddEnvironmentVariables()
    .Build();

builder.Services.Configure<MessageGatewayConfiguration>(configuration.GetSection(nameof(MessageGatewayConfiguration)));

var messageGatewayConfiguration =
    configuration.GetSection(nameof(MessageGatewayConfiguration)).Get<MessageGatewayConfiguration>();

builder.Services
    .AddBrighter(options =>
    {
        options.HandlerLifetime = ServiceLifetime.Scoped;
        options.MapperLifetime = ServiceLifetime.Singleton;
        options.PolicyRegistry = policyRegistry;
    })
    .UseExternalBus(
        new KafkaProducerRegistryFactory(
                new KafkaMessagingGatewayConfiguration
                {
                    Name = messageGatewayConfiguration.Name,
                    BootStrapServers = messageGatewayConfiguration.BootstrapServers.ToArray()
                },
                messageGatewayConfiguration.Subscriptions.Select(
                    subscription => new KafkaPublication
                    {
                        Topic = new RoutingKey(subscription.RoutingKey),
                        MaxOutStandingMessages = 5,
                        MaxOutStandingCheckIntervalMilliSeconds = 500,
                        MakeChannels = OnMissingChannel.Create,
                        Partitioner = Partitioner.Murmur2,
                        NumPartitions = 2
                    }
                )
            )
            .Create()
    )
    .AutoFromAssemblies();

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

app.MapGet("post/{message}", (string message, IAmACommandProcessor commandProcessor) =>
{
    commandProcessor.Post(new MessageEvent(message));
});

app.MapGet("send/{message}", (string message, IAmACommandProcessor commandProcessor) =>
{
    commandProcessor.Send(new MessageEvent(message));
});

app.MapGet("publish/{message}", (string message, IAmACommandProcessor commandProcessor) =>
{
    commandProcessor.Publish(new MessageEvent(message));
});

app.Run();