namespace TgGateway.Models.Updates;

public record TgCommandUpdate(
        string Command,
        long SenderId,
        long ChatId,
        DateTime DateTime)
    : TgUpdate(SenderId, ChatId, DateTime);