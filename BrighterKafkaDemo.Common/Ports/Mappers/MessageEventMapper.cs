using System.Text.Json;
using BrighterKafkaDemo.Common.Ports.Events;
using Paramore.Brighter;

namespace BrighterKafkaDemo.Common.Ports.Mappers;

public class MessageEventMapper : IAmAMessageMapper<MessageEvent>
{
    public Message MapToMessage(MessageEvent request)
    {
        var header = new MessageHeader(messageId: request.Id, topic: "message.event",
            messageType: MessageType.MT_EVENT);
        var body = new MessageBody(JsonSerializer.Serialize(request, JsonSerialisationOptions.Options));
        header.PartitionKey = request.Id.ToString();
        
        return new Message(header, body);
    }

    public MessageEvent MapToRequest(Message message)
    {
        var messageEvent = JsonSerializer.Deserialize<MessageEvent>(message.Body.Value, JsonSerialisationOptions.Options);

        return messageEvent;
    }
}