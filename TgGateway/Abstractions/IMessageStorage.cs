using TgGateway.Models;

namespace TgGateway.Abstractions;

public interface IMessageStorage
{
    Task<TgMessage?> GetMenuMessage(long chatId);

    Task<TgMessage?> GetLastMessage(
        long chatId,
        IEnumerable<TgMessagePurpose>? filter = null);

    Task SetUserState(long chatId, long userId, TgUserState state);
    Task<TgUserState> GetUserState(long chatId, long userId);
    Task SaveMessage(TgMessage msg);

    Task<IEnumerable<TgMessage>> GetMessages(
        long chatId,
        IEnumerable<TgMessagePurpose>? filter = null);

    Task DeleteMessages(long chatId, IEnumerable<long> messageIds);
    Task DeleteMessage(long chatId, long messageId);
}
