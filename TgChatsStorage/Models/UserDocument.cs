using LiteDB;

namespace TgChatsStorage.Models;

public class UserDocument
{
    [BsonId] public string Id => $"{ChatId}{UserId}";

    public long ChatId { get; set; }
    public long UserId { get; set; }
    public Dictionary<string, string> State { get; set; }
}
