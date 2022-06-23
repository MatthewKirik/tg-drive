using System.Reflection;
using TgFrontend.Helpers;
using TgFrontend.Models;
using TgGateway.Abstractions;
using TgGateway.Models;
using TgGateway.Models.Updates;

namespace TgFrontend;

public abstract class MenuBase
{
    protected readonly IBotClient _botClient;

    protected MenuBase(IBotClient botClient)
    {
        _botClient = botClient;
    }

    protected string GetSelfId()
    {
        var type = GetType();
        var attribute = type.GetCustomAttribute<TgMenuAttribute>(false);
        if (attribute == null)
        {
            throw new ApplicationException("Menu handler should specify TgMenuAttribute");
        }

        return attribute.MenuId;
    }

    protected string GetBtnId(ButtonCallbackHandler handler)
    {
        var methodInfo = handler.Method;
        var attribute = methodInfo.GetCustomAttribute<TgButtonCallbackAttribute>(false);
        if (attribute == null)
        {
            throw new ArgumentException("Passed method was of wrong signature!");
        }

        return attribute.ButtonId;
    }

    protected MessageResponseHandler? GetMessageResponseHandler(string btnId)
    {
        var methods = GetType()
            .GetMethods();
        var callbackHandlers = methods.Select(m => new
            {
                msgResponseAttribute = m
                    .GetCustomAttributes(typeof(TgMessageResponseAttribute), false)
                    .Cast<TgMessageResponseAttribute>()
                    .FirstOrDefault(),
                method = m
            })
            .Where(x => x.msgResponseAttribute != null);
        var matchingHandler =
            callbackHandlers.FirstOrDefault(
                h => h.msgResponseAttribute!.ButtonId == btnId);
        if (matchingHandler == null)
        {
            return null;
        }

        var resultDelegate = (MessageResponseHandler)Delegate.CreateDelegate(
            typeof(MessageResponseHandler),
            this,
            matchingHandler.method.Name);
        return resultDelegate;
    }

    protected ButtonCallbackHandler? GetButtonCallbackHandler(string btnId)
    {
        var methods = GetType()
            .GetMethods();
        var callbackHandlers = methods.Select(m => new
            {
                btnCallbackAttribute = m
                    .GetCustomAttributes(typeof(TgButtonCallbackAttribute), false)
                    .Cast<TgButtonCallbackAttribute>()
                    .FirstOrDefault(),
                method = m
            })
            .Where(x => x.btnCallbackAttribute != null);
        var matchingHandler =
            callbackHandlers.FirstOrDefault(
                h => h.btnCallbackAttribute!.ButtonId == btnId);
        if (matchingHandler == null)
        {
            return null;
        }

        var resultDelegate = (ButtonCallbackHandler)Delegate.CreateDelegate(
            typeof(ButtonCallbackHandler),
            this,
            matchingHandler.method.Name);
        return resultDelegate;
    }

    protected TgKeyboard GetKeyboard(IEnumerable<TgMenuButton> menuButtons)
    {
        var buttons = menuButtons
            .Select(mb => new TgButton(mb.Text, GetBtnId(mb.Handler), mb.Args));
        var keyboard = KeyboardBuilder.FromColumn(GetSelfId(), buttons.ToArray());
        return keyboard;
    }

    public virtual bool FitsCallbackId(string menuId)
    {
        return GetSelfId() == menuId;
    }

    public virtual async Task ProcessCallback(TgCallbackUpdate update)
    {
        var handler = GetButtonCallbackHandler(update.ButtonId);
        if (handler == null)
        {
            return;
        }

        await _botClient.SetState(update.ChatId,
            update.SenderId,
            new Dictionary<string, string>
            {
                {"lastMenuId", update.MenuId},
                {"lastBtnId", update.MenuId},
                {"lastArgs", update.Arguments.Aggregate("", (acc, str) => acc + str)}
            });
        await handler(update.ChatId, update.Arguments);
    }

    public async Task ProcessMessage(TgMessageUpdate update)
    {
        var lastBtnId = update.State["lastBtnId"];
        var lastArgs = update.State["lastArgs"]
            .Split();
        var handler = GetMessageResponseHandler(lastBtnId);
        if (handler == null)
        {
            return;
        }

        await handler(update.ChatId, lastArgs, update.Message);
    }
}
