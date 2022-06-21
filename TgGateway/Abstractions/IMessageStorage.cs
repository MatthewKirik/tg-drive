using TgGateway.Models;

namespace TgGateway.Abstractions;

public interface IMessageStorage
{
    Task<TgMessage?> GetMenuMessage(long chatId);
    Task SaveMessage(TgMessage msg);
    Task<IEnumerable<TgMessage>> GetMessages(IEnumerable<TgMessagePurpose> filter);
    Task DeleteMessages(IEnumerable<long> messageIds);
}