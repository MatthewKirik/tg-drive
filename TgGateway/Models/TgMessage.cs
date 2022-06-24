namespace TgGateway.Models;

public record TgMessage(
    long SenderId,
    long ChatId,
    long MessageId,
    DateTime DateTime,
    TgMessageType Type,
    TgMessagePurpose Purpose,
    string? Text
);
