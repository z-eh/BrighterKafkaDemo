using Paramore.Brighter;

namespace BrighterKafkaDemo.Common.Ports.Events;

public class MessageEvent : Event
{
    public MessageEvent(string message) : base(Guid.NewGuid())
    {
        Message = message;
    }

    public string Message { get; }
}