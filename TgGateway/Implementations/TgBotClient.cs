using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgGateway.Abstractions;
using TgGateway.Models;
using TgGateway.Models.Updates;
using IUpdateHandler = TgGateway.Abstractions.IUpdateHandler;

namespace TgGateway.Implementations;

public class TgBotClient : IBotClient
{
    private readonly ITelegramBotClient _tgBotClient;
    private readonly IMessageStorage _storage;
    private IUpdateHandler? _updateHandler;

    public TgBotClient(ITelegramBotClient tgBotClient, IMessageStorage storage)
    {
        _tgBotClient = tgBotClient;
        _storage = storage;
    }

    public void StartReceiving(IUpdateHandler handler)
    {
        _updateHandler = handler;
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions();
        _tgBotClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
    }

    private async void HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken ct)
    {
        await _updateHandler!.HandleError(exception);
    }

    private async void HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                await ProcessMessage(update.Message!);
                break;
            case UpdateType.CallbackQuery:
                await ProcessCallbackQuery(update.CallbackQuery!);
                break;
            default:
                return;
        }
    }

    private async Task ProcessCallbackQuery(CallbackQuery callback)
    {
        if (string.IsNullOrEmpty(callback.Data)) return;
        var parts = callback.Data.Split();
        string? menuId, btnId;
        List<string> args;
        try
        {
            menuId = parts[0];
            btnId = parts[1];
            args = parts.Skip(2).ToList();
        }
        catch (Exception)
        {
            return;
        }

        await _updateHandler!.HandleCallback(new TgCallbackUpdate
        (
            DateTime: DateTime.Now,
            MenuId: menuId,
            ButtonId: btnId,
            Arguments: args,
            ChatId: callback.Message!.Chat.Id,
            SenderId: callback.From.Id
        ));
    }

    private async Task ProcessMessage(Message msg)
    {
        if (msg.From == null) return;
        if (msg.Text!.StartsWith("/"))
        {
            var command = new string(msg.Text.Skip(1).ToArray());
            if (command == string.Empty)
                return;
            await _updateHandler!.HandleCommand(new TgCommandUpdate
            (
                ChatId: msg.Chat.Id,
                DateTime: msg.Date,
                Command: command,
                SenderId: msg.From.Id
            ));
        }
        else
        {
            await _updateHandler!.HandleMessage(new TgMessageUpdate
            (
                ChatId: msg.Chat.Id,
                SenderId: msg.From.Id,
                DateTime: msg.Date,
                Message: new TgMessage
                (
                    ChatId: msg.Chat.Id,
                    MessageId: msg.MessageId,
                    DateTime: msg.Date,
                    SenderId: msg.From!.Id,
                    Type: (TgMessageType) msg.Type,
                    Purpose: TgMessagePurpose.Unknown
                )
            ));
        }
    }

    public async Task<long> SendMenu(long chatId, MenuData data)
    {
        // delete messages in background as it can take long time
        _ = TryClearChatExceptMenu(chatId);
        var existingMenuMsg = await _storage.GetMenuMessage(chatId);
        var needToSendMenu = true;
        if (existingMenuMsg != null)
        {
            var edited = await TryEditMessage(
                chatId,
                existingMenuMsg.MessageId,
                data.Text,
                data.Keyboard);
            needToSendMenu = !edited;
        }

        if (!needToSendMenu) return existingMenuMsg!.MessageId;
        var sentMenuMsg = await _tgBotClient.SendTextMessageAsync(
            chatId,
            data.Text,
            ParseMode.Html,
            replyMarkup: data.Keyboard);
        await _storage.SaveMessage(new TgMessage
        (
            ChatId: chatId,
            DateTime: sentMenuMsg.Date,
            MessageId: sentMenuMsg.MessageId,
            Purpose: TgMessagePurpose.Menu,
            SenderId: sentMenuMsg.From!.Id,
            Type: (TgMessageType) sentMenuMsg.Type
        ));
        return sentMenuMsg.MessageId;
    }

    private async Task<bool> TryEditMessage(
        long chatId,
        long messageId,
        string newText,
        InlineKeyboardMarkup keys)
    {
        try
        {
            _ = await _tgBotClient.EditMessageTextAsync(
                new ChatId(chatId),
                (int) messageId,
                newText,
                ParseMode.Html,
                replyMarkup: keys);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private readonly TgMessagePurpose[] _unimportantPurposes =
    {
        TgMessagePurpose.Command,
        TgMessagePurpose.Message,
        TgMessagePurpose.Unknown
    };

    private async Task TryClearChatExceptMenu(long chatId)
    {
        var msgsToDelete = await _storage.GetMessages(_unimportantPurposes);
        foreach (var msg in msgsToDelete)
            try
            {
                await _tgBotClient.DeleteMessageAsync(
                    new ChatId(chatId),
                    (int) msg.MessageId);
            }
            catch (Exception)
            {
                // ignored as the feature is unstable and not very important
            }
    }

    public async Task<long> SendText(long chatId, string text, long? replyToMsgId = null)
    {
        var message = await _tgBotClient.SendTextMessageAsync(new ChatId(chatId), text, ParseMode.Html);
        await _storage.SaveMessage(new TgMessage
        (
            ChatId: chatId,
            DateTime: message.Date,
            MessageId: message.MessageId,
            Purpose: TgMessagePurpose.Message,
            SenderId: message.From!.Id,
            Type: (TgMessageType) message.Type
        ));
        return message.MessageId;
    }

    public async Task<long> ForwardMessageUnmanaged(long fromChat, long toChat, long messageId)
    {
        var message = await _tgBotClient.ForwardMessageAsync(
            new ChatId(toChat),
            new ChatId(fromChat),
            (int) messageId);
        return message.MessageId;
    }

    public async IAsyncEnumerable<long> ForwardMessagesUnmanaged(long fromChat, long toChat,
        IEnumerable<long> messageIds)
    {
        foreach (var msgId in messageIds)
        {
            var message = await _tgBotClient.ForwardMessageAsync(
                new ChatId(toChat),
                new ChatId(fromChat),
                (int) msgId);
            yield return message.MessageId;
        }
    }

    public async Task<long> CopyMessageUnmanaged(long fromChat, long toChat, long messageId)
    {
        var message = await _tgBotClient.CopyMessageAsync(
            new ChatId(toChat),
            new ChatId(fromChat),
            (int) messageId);
        return message.Id;
    }

    public async IAsyncEnumerable<long> CopyMessagesUnmanaged(long fromChat, long toChat, IEnumerable<long> messageIds)
    {
        foreach (var msgId in messageIds)
        {
            var message = await _tgBotClient.CopyMessageAsync(
                new ChatId(toChat),
                new ChatId(fromChat),
                (int) msgId);
            yield return message.Id;
        }
    }
}