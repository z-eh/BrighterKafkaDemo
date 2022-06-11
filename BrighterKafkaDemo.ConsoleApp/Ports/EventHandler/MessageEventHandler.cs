using BrighterKafkaDemo.Common.Ports.Events;
using Paramore.Brighter;

namespace BrighterKafkaDemo.ConsoleApp.Ports.EventHandler;

public class MessageEventHandler : RequestHandler<MessageEvent>
{
    private readonly MessageContainer _messageContainer;

    public MessageEventHandler(MessageContainer messageContainer)
    {
        _messageContainer = messageContainer;
    }

    public override MessageEvent Handle(MessageEvent command)
    {
        _messageContainer.Receive(command);
        return base.Handle(command);
    }
}