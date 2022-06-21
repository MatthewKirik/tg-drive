using TgGateway.Models;

namespace TgChatsStorage.Models;

public class ChatDocument
{
    public long ChatId { get; set; }
    public List<TgMessage> Messages { get; set; } = new();
}