using Telegram.Bot.Types.ReplyMarkups;

namespace TgGateway.Models;

public record MenuData
{
    public string Text { get; init; }
    public InlineKeyboardMarkup Keyboard { get; init; }
}