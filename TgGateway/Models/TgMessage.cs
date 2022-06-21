namespace TgGateway.Models;

public record TgMessage
{
    public long SenderId { get; init; }
    public long ChatId { get; init; }
    public long MessageId { get; init; }
    public DateTime ReceivedAt { get; init; }
    public TgMessageType Type { get; init; }
}