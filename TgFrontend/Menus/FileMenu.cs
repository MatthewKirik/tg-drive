using DriveServices;
using TgFrontend.Models;
using TgGateway.Abstractions;
using TgGateway.Models;

namespace TgFrontend.Menus;

public class FileMenu : MenuBase
{
    private readonly IDirectoryService _directoryService;
    private readonly IFileService _fileService;
    private readonly ITgFileService _tgFileService;
    private readonly DirectoryMenu _directoryMenu;

    public FileMenu(
        IDirectoryService directoryService,
        IFileService fileService,
        ITgFileService tgFileService,
        DirectoryMenu directoryMenu,
        IBotClient botClient)
        : base(botClient)
    {
        _directoryService = directoryService;
        _fileService = fileService;
        _tgFileService = tgFileService;
        _directoryMenu = directoryMenu;
    }

    [TgButtonCallback("cd")]
    private async Task MenuBtn_ChangeDescription(
        long chatId,
        IEnumerable<string> parameters)
    {
        await _botClient.SendText(chatId, "Send new description for the file");
    }

    [TgMessageResponse("cd")]
    private async Task ChangeDescription(
        long chatId,
        IEnumerable<string> parameters,
        TgMessage message)
    {
        if (message.Text == null)
        {
            await _botClient.SendText(
                chatId,
                "Please, send text message with new directory description name");
            return;
        }

        var fileId = long.Parse(parameters.First());
        _ = await _fileService.ChangeDescription(fileId, message.Text);
        await _botClient.SendText(chatId, "Changed description successfully!");
    }

    [TgButtonCallback("rem")]
    private async Task MenuBtn_Remove(long chatId, IEnumerable<string> parameters)
    {
        var fileId = long.Parse(parameters.First());
        var file = await _fileService.Remove(fileId);
        await _botClient.SendText(chatId, "Deleted successfully!");
    }
    
    [TgButtonCallback("back")]
    private async Task MenuBtn_GoBack(long chatId, IEnumerable<string> parameters)
    {
        var fileId = long.Parse(parameters.First());
        var file = await _fileService.GetFile(fileId);
        await _directoryMenu.Open(chatId, file.Id);
    }

    private TgKeyboard GetKeyboard()
    {
        var buttons = new List<TgMenuButton>
        {
            new("Remove", MenuBtn_Remove),
            new("Change description", MenuBtn_ChangeDescription),
            new("Back", MenuBtn_GoBack)
        };
        var keyboard = GetKeyboard(buttons);
        return keyboard;
    }

    public async Task Open(long chatId, long fileId)
    {
        var file = await _fileService.GetFile(fileId);
        var keyboard = GetKeyboard();
        await _botClient.SendMenu(chatId, new MenuData(file.Name, keyboard));
        await _tgFileService.SendFile(fileId);
    }
}
