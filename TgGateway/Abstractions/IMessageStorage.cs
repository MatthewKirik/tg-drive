using TgGateway.Models;

namespace TgGateway.Abstractions;

public interface IMessageStorage
{
    Task<TgMessage?> GetMenuMessage(long chatId);
    Task SaveMessage(TgMessage msg);
    Task<IEnumerable<TgMessage>> GetMessages(long chatId, IEnumerable<TgMessagePurpose> filter);
    Task DeleteMessages(long chatId, IEnumerable<long> messageIds);
    Task DeleteMessage(long chatId, long messageId);
}