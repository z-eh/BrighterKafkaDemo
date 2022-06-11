namespace BrighterKafkaDemo.Common.Configuration;

public class MessageGatewayConfiguration
{
    public string Name { get; set; }

    public List<string> BootstrapServers { get; set; }

    public List<SubscriptionConfiguration> Subscriptions { get; set; }
}