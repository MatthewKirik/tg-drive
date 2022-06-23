using DataTransfer.Objects;
using DriveServices;
using TgFrontend.Models;
using TgGateway.Abstractions;
using TgGateway.Models;

namespace TgFrontend.Menus;

[TgMenu("dir")]
public class DirectoryMenu : MenuBase
{
    private readonly IDirectoryService _directoryService;
    private readonly IFileService _fileService;
    private readonly FileMenu _fileMenu;
    private readonly RootMenu _rootMenu;

    public DirectoryMenu(
        IBotClient botClient,
        IDirectoryService directoryService,
        IFileService fileService,
        FileMenu fileMenu,
        RootMenu rootMenu)
        : base(botClient)
    {
        _directoryService = directoryService;
        _fileService = fileService;
        _fileMenu = fileMenu;
        _rootMenu = rootMenu;
    }

    [TgButtonCallback("ren")]
    private async Task MenuBtn_RenameDirectory(
        long chatId,
        IEnumerable<string> parameters)
    {
        await _botClient.SendText(chatId, "Enter new directory name:");
    }

    [TgMessageResponse("ren")]
    private async Task RenameDirectory(
        long chatId,
        IEnumerable<string> parameters,
        TgMessage message)
    {
        if (message.Text == null)
        {
            await _botClient.SendText(
                chatId,
                "Please, send text message with new directory name");
            return;
        }

        var directoryId = long.Parse(parameters.First());
        _ = await _directoryService.RenameDirectory(directoryId, message.Text);
        await _botClient.SendText(chatId, "Renamed successfully!");
    }

    [TgButtonCallback("ga")]
    private async Task MenuBtn_GiveAccess(long chatId, IEnumerable<string> parameters)
    {
        await _botClient.SendText(chatId, "Send id of user to give access to:");
    }

    [TgMessageResponse("ga")]
    private async Task GiveAccess(
        long chatId,
        IEnumerable<string> parameters,
        TgMessage message)
    {
        var directoryId = long.Parse(parameters.First());
        await _directoryService.SetAccessRights(directoryId,
            message.SenderId,
            true,
            true);
        await _botClient.SendText(chatId, "Successfully changed access rights!");
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
        await Open(chatId, directoryId);
    }

    [TgButtonCallback("of")]
    private async Task MenuBtn_OpenFile(long chatId, IEnumerable<string> parameters)
    {
        var fileId = long.Parse(parameters.First());
        await _fileMenu.Open(chatId, fileId);
    }

    [TgButtonCallback("back")]
    private async Task MenuBtn_GoBack(long chatId, IEnumerable<string> parameters)
    {
        await _rootMenu.Open(chatId);
    }

    private TgKeyboard GetKeyboard(
        IEnumerable<DirectoryDto> subdirs,
        IEnumerable<FileDto> files)
    {
        var buttons = new List<TgMenuButton>
        {
            new("Rename", MenuBtn_RenameDirectory),
            new("Give access", MenuBtn_GiveAccess),
            new("Add subdirectory", MenuBtn_AddSubdir)
        };
        foreach (var subdir in subdirs)
        {
            buttons.Add(new TgMenuButton(subdir.Name, MenuBtn_OpenSubdir, subdir.Id));
        }

        foreach (var file in files)
        {
            buttons.Add(new TgMenuButton(file.Name, MenuBtn_OpenFile, file.Id));
        }

        buttons.Add(new TgMenuButton("Back", MenuBtn_GoBack));
        var keyboard = GetKeyboard(buttons);
        return keyboard;
    }

    public async Task Open(long chatId, long directoryId)
    {
        var dir = await _directoryService.GetDirectory(directoryId);
        var subdirs = await _directoryService.GetChildren(directoryId);
        var files = await _fileService.GetFiles(directoryId);
        var keyboard = GetKeyboard(subdirs, files);
        await _botClient.SendMenu(chatId, new MenuData(dir.Name, keyboard));
    }
}
