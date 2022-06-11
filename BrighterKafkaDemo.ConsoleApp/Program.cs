using BrighterKafkaDemo.Common.Configuration;
using BrighterKafkaDemo.Common.Ports.Events;
using BrighterKafkaDemo.ConsoleApp;
using BrighterKafkaDemo.ConsoleApp.Views;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Paramore.Brighter;
using Paramore.Brighter.MessagingGateway.Kafka;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;
using Paramore.Brighter.ServiceActivator.Extensions.Hosting;
using Terminal.Gui;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddEnvironmentVariables()
            .Build();

        services.Configure<MessageGatewayConfiguration>(configuration.GetSection(nameof(MessageGatewayConfiguration)));

        var messageGatewayConfiguration = configuration.GetSection(nameof(MessageGatewayConfiguration))
            .Get<MessageGatewayConfiguration>();

        var subscriptions = new List<KafkaSubscription>();
        subscriptions.AddRange(messageGatewayConfiguration.Subscriptions.Select(
            subscription => new KafkaSubscription<MessageEvent>(
                new SubscriptionName(subscription.Name),
                new ChannelName(subscription.ChannelName),
                new RoutingKey(subscription.RoutingKey),
                subscription.GroupId,
                timeoutInMilliseconds: subscription.TimeoutInMilliseconds,
                offsetDefault: Enum.TryParse(subscription.OffsetDefault, out AutoOffsetReset defaultOffset)
                    ? defaultOffset
                    : AutoOffsetReset.Earliest,
                commitBatchSize: subscription.CommitBatchSize
            )
        ));

        var consumerFactory = new KafkaMessageConsumerFactory(
            new KafkaMessagingGatewayConfiguration
            {
                Name = messageGatewayConfiguration.Name,
                BootStrapServers = messageGatewayConfiguration.BootstrapServers.ToArray()
            }
        );

        services
            .AddServiceActivator(options =>
            {
                options.Subscriptions = subscriptions;
                options.ChannelFactory = new ChannelFactory(consumerFactory);
            })
            .AutoFromAssemblies();
        
        services.AddHostedService<ServiceActivatorHostedService>();
        
        services.AddSingleton<MessageContainer>();
        services.AddSingleton<MainWindow>();
    })
    .UseConsoleLifetime()
    .Build();

await host.StartAsync();

Application.Init();

var mainWindow = host.Services.GetService<MainWindow>();
mainWindow.InitStyles();
mainWindow.InitControls();

Application.Run(mainWindow);
Application.Shutdown();

await host.StopAsync();