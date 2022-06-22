namespace TgGateway.Models.Updates;

public record TgCallbackUpdate(
        string MenuId,
        string ButtonId,
        IEnumerable<string> Arguments,
        long SenderId,
        long ChatId,
        DateTime DateTime)
    : TgUpdate(SenderId, ChatId, DateTime);
