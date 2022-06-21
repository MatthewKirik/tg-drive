using Telegram.Bot.Types.ReplyMarkups;

namespace TgGateway.Models;

public record MenuData(
    string Text,
    InlineKeyboardMarkup Keyboard
);