using BrighterKafkaDemo.Common.Ports.Events;
using Terminal.Gui;

namespace BrighterKafkaDemo.ConsoleApp;

public class MessageContainer
{
    private readonly List<string> _messages = new();
    private ListView? _receiver;

    public void Receive(MessageEvent messageEvent)
    {
        _messages.Add(messageEvent.Message);
        _receiver?.SetSource(_messages);
    }

    public void RegisterReceiver(ListView receiver)
    {
        _receiver = receiver;
    }
}