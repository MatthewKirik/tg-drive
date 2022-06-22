using Telegram.Bot.Types.ReplyMarkups;

namespace TgGateway.Models;

public abstract class MenuBase
{
    public abstract bool FitsCallbackId(string callbackData);
    public abstract Task Open();

    public static InlineKeyboardMarkup CreateKeyboard(
        string menuCallbackId,
        params (string text, string callback)[] buttons)
    {
        var keys = new List<List<InlineKeyboardButton>>();
        foreach (var btn in buttons)
        {
            var inlineBtn = new InlineKeyboardButton(btn.text)
            {
                CallbackData = $"{menuCallbackId} {btn.callback}"
            };
            keys.Add(new List<InlineKeyboardButton> {inlineBtn});
        }

        var keyboard = new InlineKeyboardMarkup(keys);
        return keyboard;
    }
}