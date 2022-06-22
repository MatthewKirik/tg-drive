using TgGateway.Abstractions;
using TgGateway.Models;
using TgGateway.Models.Updates;

namespace DummyTest;

public class DummyHandler : IUpdateHandler
{
    private readonly IBotClient _bot;

    private readonly MenuData mainMenu = new(
        "Path:  /data/you/are",
        MenuBase.CreateKeyboard("main",
            ("Subfolder 1", "clb1"),
            ("file.txt", "clb1"),
            ("porn.txt", "clb1"))
    );

    private readonly MenuData subMenu = new(
        "Path:  /data/you/are/Subfolder 1",
        MenuBase.CreateKeyboard("sub",
            ("Subfolder 1", "clb1"),
            ("amogus.txt", "clb1"),
            ("magogus.txt", "clb1"),
            ("Back", "back")
        )
    );

    public DummyHandler(IBotClient bot)
    {
        _bot = bot;
    }

    public async Task HandleCallback(TgCallbackUpdate update)
    {
        switch (update.MenuId)
        {
            case "main":
                await _bot.SendMenu(update.ChatId, subMenu);
                break;
            case "sub":
                await _bot.SendMenu(update.ChatId, mainMenu);
                break;
        }
    }

    public Task HandleMessage(TgMessageUpdate update)
    {
        Console.WriteLine(update.Message);
        return Task.CompletedTask;
    }

    public async Task HandleCommand(TgCommandUpdate update)
    {
        Console.WriteLine("Command " + update.Command);
        switch (update.Command)
        {
            case "start":
                await _bot.SendMenu(update.ChatId, mainMenu);
                break;
        }
    }

    public Task HandleError(Exception exception)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }
}
