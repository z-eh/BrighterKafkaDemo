namespace BrighterKafkaDemo.Common.Configuration;

public class SubscriptionConfiguration
{
    public string Name { get; set; }

    public string ChannelName { get; set; }

    public string RoutingKey { get; set; }

    public string GroupId { get; set; }

    public int TimeoutInMilliseconds { get; set; }

    public string OffsetDefault { get; set; }

    public int CommitBatchSize { get; set; }
}