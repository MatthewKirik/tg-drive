using TgGateway.Models;

namespace TgFrontend.Helpers;

public static class KeyboardBuilder
{
    public static TgKeyboard FromColumn(
        string menuCallbackId,
        TgButton[] buttons)
    {
        var keys = new List<List<TgButton>>();
        foreach (var btn in buttons)
        {
            var inlineBtn = new TgButton(btn.Text,
                $"{menuCallbackId} {btn.Callback}",
                btn.args);
            keys.Add(new List<TgButton> {inlineBtn});
        }

        var keyboard = new TgKeyboard(keys);
        return keyboard;
    }
}
