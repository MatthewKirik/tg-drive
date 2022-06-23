namespace TgGateway.Models.Updates;

public record TgCommandUpdate(
        string Command,
        long SenderId,
        long ChatId,
        DateTime DateTime,
        Dictionary<string, string> State)
    : TgUpdate(SenderId, ChatId, DateTime, State);
