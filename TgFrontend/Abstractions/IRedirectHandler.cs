using TgGateway.Abstractions;

namespace TgFrontend.Abstractions;

public interface IRedirectHandler
{
    void Initialize(IBotClient bot, IEnumerable<MenuBase> menus);
    Task Redirect(long chatId, Type menuType, params object[] args);
}
