namespace TgGateway.Models.Updates;

public record TgErrorUpdate(
        Exception Error,
        long SenderId,
        long ChatId,
        DateTime DateTime,
        Dictionary<string, string> state)
    : TgUpdate(SenderId, ChatId, DateTime, state);
