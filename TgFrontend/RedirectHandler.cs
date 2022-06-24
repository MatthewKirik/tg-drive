using TgFrontend.Abstractions;
using TgGateway.Abstractions;

namespace TgFrontend;

public class RedirectHandler : IRedirectHandler
{
    private IEnumerable<MenuBase> _menus = new List<MenuBase>();
    private IBotClient _bot;

    public void Initialize(IBotClient bot, IEnumerable<MenuBase> menus)
    {
        _bot = bot;
        _menus = menus;
    }

    public async Task Redirect(long chatId, Type menuType, params object[] args)
    {
        var menu = _menus.FirstOrDefault(m => m.GetType() == menuType);
        if (menu == null)
        {
            return;
        }

        var openMethod = menu.GetType()
            .GetMethod("Open");
        if (openMethod == null)
        {
            return;
        }

        var result = openMethod.Invoke(menu, args);
        if (result?.GetType() != typeof(Task))
        {
            return;
        }

        await (Task)result;
    }
}
