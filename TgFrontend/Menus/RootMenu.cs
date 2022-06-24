using DataTransfer.Objects;
using DriveServices;
using TgFrontend.Abstractions;
using TgFrontend.Models;
using TgGateway.Abstractions;
using TgGateway.Models;

namespace TgFrontend.Menus;

public class RootMenu : MenuBase
{
    private readonly IDirectoryService _directoryService;
    private readonly IRedirectHandler _redirectHandler;

    public RootMenu(
        IBotClient botClient,
        IDirectoryService directoryService,
        IRedirectHandler redirectHandler)
        : base(botClient)
    {
        _directoryService = directoryService;
        _redirectHandler = redirectHandler;
    }


    [TgButtonCallback("as")]
    private async Task MenuBtn_AddSubdir(long chatId, IEnumerable<string> parameters)
    {
        await _botClient.SendText(chatId, "Send name of the new directory:");
    }

    [TgMessageResponse("as")]
    private async Task AddSubdir(
        long chatId,
        IEnumerable<string> parameters,
        TgMessage message)
    {
        if (message.Text == null)
        {
            await _botClient.SendText(
                chatId,
                "Please, send text message with a name for the new directory");
            return;
        }

        var parentId = long.Parse(parameters.First());
        var newDirectory = new DirectoryDto
        {
            Name = message.Text, OwnerId = message.SenderId, ParentId = parentId
        };
        await _directoryService.AddDirectory(newDirectory);
    }

    [TgButtonCallback("os")]
    private async Task MenuBtn_OpenSubdir(long chatId, IEnumerable<string> parameters)
    {
        var directoryId = long.Parse(parameters.First());
        await _redirectHandler.Redirect(chatId, typeof(DirectoryMenu), directoryId);
    }

    private TgKeyboard GetKeyboard(IEnumerable<DirectoryDto> subdirs)
    {
        var buttons = new List<TgMenuButton> {new("Add directory", MenuBtn_AddSubdir)};
        foreach (var subdir in subdirs)
        {
            buttons.Add(new TgMenuButton(subdir.Name, MenuBtn_OpenSubdir, subdir.Id));
        }

        var keyboard = GetKeyboard(buttons);
        return keyboard;
    }

    public async Task Open(long chatId)
    {
        var dirs = await _directoryService.GetRoot(chatId);
        var keyboard = GetKeyboard(dirs);
        var menu = new MenuData("You are in the root of your storage.", keyboard);
        await _botClient.SendMenu(chatId, menu);
    }
}
