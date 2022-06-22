namespace TgGateway.Models.Updates;

public record TgErrorUpdate(
        Exception Error,
        long SenderId,
        long ChatId,
        DateTime DateTime)
    : TgUpdate(SenderId, ChatId, DateTime);