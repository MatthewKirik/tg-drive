using DriveServices;
using TgGateway.Abstractions;

namespace TgFrontend.Menus;

public class RootMenu : MenuBase
{
    private readonly IDirectoryService _directoryService;

    public RootMenu(
        IBotClient botClient,
        IDirectoryService directoryService)
        : base(botClient)
    {
        _directoryService = directoryService;
    }

    public async Task Open(long chatId)
    {
        throw new NotImplementedException();
    }
}
