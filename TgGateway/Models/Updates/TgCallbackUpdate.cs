namespace TgGateway.Models.Updates;

public record TgCallbackUpdate(
        string MenuId,
        string ButtonId,
        IEnumerable<string> Arguments,
        long SenderId,
        long ChatId,
        DateTime DateTime,
        Dictionary<string, string> State)
    : TgUpdate(SenderId, ChatId, DateTime, State);
