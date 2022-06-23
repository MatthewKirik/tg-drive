using Repositories;
using TgFrontend.Models;
using TgGateway.Abstractions;
using TgGateway.Models;

namespace TgFrontend.Menus;

public class FileMenu : MenuBase
{
    private readonly IFileRepository _fileRepository;

    public FileMenu(IFileRepository fileRepository, IBotClient botClient)
        : base(botClient)
    {
        _fileRepository = fileRepository;
    }

    [TgButtonCallback("back")]
    private async Task MenuBtn_GoBack(long chatId, IEnumerable<string> parameters)
    {
        throw new NotImplementedException();
    }

    private TgKeyboard GetKeyboard()
    {
        var buttons = new List<TgMenuButton>
        {
            new("Remove", MenuBtn_GoBack),
            new("Give access", MenuBtn_GoBack),
            new("Back", MenuBtn_GoBack)
        };
        var keyboard = GetKeyboard(buttons);
        return keyboard;
    }

    public async Task Open(long chatId, long fileId)
    {
        var file = await _fileRepository.GetFile(fileId);
        var keyboard = GetKeyboard();
        await _botClient.SendMenu(chatId, new MenuData(file.Name, keyboard));
        await _botClient.CopyMessageUnmanaged(file.ChatId, chatId, file.MessageId);
    }
}
