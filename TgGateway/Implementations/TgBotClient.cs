using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgGateway.Abstractions;
using TgGateway.Models;
using IUpdateHandler = TgGateway.Abstractions.IUpdateHandler;

namespace TgGateway.Implementations;

public class TgBotClient : IBotClient
{
    private readonly IMessageStorage _storage;
    private readonly ITelegramBotClient _tgBotClient;

    private readonly TgMessagePurpose[] _unimportantPurposes =
    {
        TgMessagePurpose.Command, TgMessagePurpose.Message, TgMessagePurpose.Unknown
    };

    private UpdateParser? _updateParser;

    public TgBotClient(ITelegramBotClient tgBotClient, IMessageStorage storage)
    {
        _tgBotClient = tgBotClient;
        _storage = storage;
    }

    public void StartReceiving(IUpdateHandler handler)
    {
        _updateParser = new UpdateParser(_storage, handler);
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions();
        _tgBotClient.StartReceiving(
            _updateParser,
            receiverOptions,
            cancellationToken
        );
    }


    public async Task<long> SendMenu(long chatId, MenuData data)
    {
        // delete messages in background as it can take long time
        TryClearChatExceptMenu(chatId);
        var existingMenuMsg = await _storage.GetMenuMessage(chatId);
        var needToSendMenu = true;
        if (existingMenuMsg != null)
        {
            var edited = await TryEditMessage(
                chatId,
                existingMenuMsg.MessageId,
                data.Text,
                data.Keyboard);
            if (!edited)
            {
                await TryDeleteMessage(chatId, existingMenuMsg.MessageId);
            }

            needToSendMenu = !edited;
        }

        if (!needToSendMenu)
        {
            return existingMenuMsg!.MessageId;
        }

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
            Type: (TgMessageType)sentMenuMsg.Type
        ));
        return sentMenuMsg.MessageId;
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
            Type: (TgMessageType)message.Type
        ));
        return message.MessageId;
    }

    public async Task<long> ForwardMessageUnmanaged(long fromChat, long toChat, long messageId)
    {
        var message = await _tgBotClient.ForwardMessageAsync(
            new ChatId(toChat),
            new ChatId(fromChat),
            (int)messageId);
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
                (int)msgId);
            yield return message.MessageId;
        }
    }

    public async Task<long> CopyMessageUnmanaged(long fromChat, long toChat, long messageId)
    {
        var message = await _tgBotClient.CopyMessageAsync(
            new ChatId(toChat),
            new ChatId(fromChat),
            (int)messageId);
        return message.Id;
    }

    public async IAsyncEnumerable<long> CopyMessagesUnmanaged(long fromChat, long toChat, IEnumerable<long> messageIds)
    {
        foreach (var msgId in messageIds)
        {
            var message = await _tgBotClient.CopyMessageAsync(
                new ChatId(toChat),
                new ChatId(fromChat),
                (int)msgId);
            yield return message.Id;
        }
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
                (int)messageId,
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

    private async Task TryDeleteMessage(long chatId, long messageId)
    {
        try
        {
            await _tgBotClient.DeleteMessageAsync(
                new ChatId(chatId),
                (int)messageId);
            await _storage.DeleteMessage(chatId, messageId);
        }
        catch (Exception)
        {
            // ignored as the message might not exists
        }
    }


    private async void TryClearChatExceptMenu(long chatId)
    {
        var msgsToDelete = await _storage.GetMessages(chatId, _unimportantPurposes);
        foreach (var msg in msgsToDelete)
        {
            try
            {
                await _tgBotClient.DeleteMessageAsync(
                    new ChatId(chatId),
                    (int)msg.MessageId);
            }
            catch (Exception)
            {
                // ignored as the feature is unstable and not very important
            }
        }

        await _storage.DeleteMessages(chatId, msgsToDelete.Select(x => x.MessageId));
    }
}
