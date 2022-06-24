namespace TgGateway.Models;

public record TgUserState(
    long userId,
    long chatId,
    Dictionary<string, string> values
);
