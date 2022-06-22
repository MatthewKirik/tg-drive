namespace TgGateway.Models.Updates;

public abstract record TgUpdate(
    long SenderId,
    long ChatId,
    DateTime DateTime
);
