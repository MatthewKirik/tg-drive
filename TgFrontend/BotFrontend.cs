using TgFrontend.Menus;
using TgGateway.Abstractions;
using TgGateway.Models.Updates;

namespace TgFrontend;

public class BotFrontend : IUpdateHandler
{
    private readonly IBotClient _botClient;
    private readonly IEnumerable<MenuBase> _menus;

    public BotFrontend(IBotClient botClient, DirectoryMenu directoryMenu)
    {
        _botClient = botClient;
        _menus = new[] {directoryMenu};
    }

    public void Start()
    {
        _botClient.StartReceiving(this);
    }

    public async Task HandleCallback(TgCallbackUpdate update)
    {
        var menu = GetMenu(update.MenuId);
        if (menu == null)
        {
            return;
        }

        await menu.ProcessCallback(update);
    }

    public async Task HandleMessage(TgMessageUpdate update)
    {
        update.State.TryGetValue("lastMenuId", out var menuId);
        if (menuId == null)
        {
            return;
        }

        var menu = GetMenu(menuId);
        if (menu == null)
        {
            return;
        }

        await menu.ProcessMessage(update);
    }

    public async Task HandleCommand(TgCommandUpdate update)
    {
        switch (update.Command)
        {
            case "start":
                // await RootMenu.Open(chatId);
                break;
            default: return;
        }
    }

    public Task HandleError(Exception error)
    {
        Console.WriteLine(error);
        return Task.CompletedTask;
    }

    private MenuBase? GetMenu(string menuId)
    {
        return _menus.FirstOrDefault(m => m.FitsCallbackId(menuId));
    }
}
