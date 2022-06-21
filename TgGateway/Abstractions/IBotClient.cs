using TgGateway.Models;

namespace TgGateway.Abstractions;

public interface IBotClient
{
    void StartReceiving(IUpdateHandler handler);
    Task<long> SendMenu(long chatId, MenuData data);
    Task<long> SendText(long chatId, string text, long? replyToMsgId = null);
    Task<long> ForwardMessageUnmanaged(long fromChat, long toChat, long messageId);
    Task<IEnumerable<long>> ForwardMessagesUnmanaged(long fromChat, long toChat, IEnumerable<long> messageIds);
    Task<long> CopyMessageUnmanaged(long fromChat, long toChat, long messageId);
    Task<IEnumerable<long>> CopyMessagesUnmanaged(long fromChat, long toChat, IEnumerable<long> messageIds);
}