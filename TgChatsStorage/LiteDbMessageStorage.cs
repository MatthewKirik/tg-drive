using LiteDB;
using TgChatsStorage.Models;
using TgGateway.Abstractions;
using TgGateway.Models;

namespace TgChatsStorage;

public class LiteDbMessageStorage : IMessageStorage
{
    private readonly ILiteDatabase _db;
    private const string ChatsCollectionName = "chats";

    public LiteDbMessageStorage(ILiteDatabase db, string dataFolder)
    {
        _db = db;
    }

    public async Task<TgMessage?> GetMenuMessage(long chatId)
    {
        TgMessage? result = null;
        await Task.Run(() =>
        {
            var chats = _db.GetCollection<ChatDocument>(ChatsCollectionName);
            var chat = chats
                .FindOne(x => x.ChatId == chatId);
            result = chat.Messages
                .FirstOrDefault(x => x.Purpose == TgMessagePurpose.Menu);
        });
        return result;
    }

    public async Task SaveMessage(TgMessage msg)
    {
        await Task.Run(() =>
        {
            var chats = _db.GetCollection<ChatDocument>(ChatsCollectionName);
            var chat = chats
                .FindOne(x => x.ChatId == msg.ChatId);
            chat.Messages.Add(msg);
            chats.Update(chat);
        });
    }

    public async Task<IEnumerable<TgMessage>> GetMessages(long chatId, IEnumerable<TgMessagePurpose> filter)
    {
        IEnumerable<TgMessage> messages = new List<TgMessage>();
        await Task.Run(() =>
        {
            var chats = _db.GetCollection<ChatDocument>(ChatsCollectionName);
            var chat = chats
                .FindOne(x => x.ChatId == chatId);
            messages = chat.Messages
                .Where(x => filter.Contains(x.Purpose))
                .ToList();
        });
        return messages;
    }

    public async Task DeleteMessages(long chatId, IEnumerable<long> messageIds)
    {
        await Task.Run(() =>
        {
            var chats = _db.GetCollection<ChatDocument>(ChatsCollectionName);
            var chat = chats
                .FindOne(x => x.ChatId == chatId);
            chat.Messages = chat.Messages
                .Where(x => !messageIds.Contains(x.MessageId))
                .ToList();
            chats.Update(chat);
        });
    }
}