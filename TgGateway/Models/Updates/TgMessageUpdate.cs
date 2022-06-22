namespace TgGateway.Models.Updates;

public record TgMessageUpdate(
        TgMessage Message,
        long SenderId,
        long ChatId,
        DateTime DateTime)
    : TgUpdate(SenderId, ChatId, DateTime);
