using LiteDB;
using TgChatsStorage.Models;
using TgGateway.Abstractions;
using TgGateway.Models;

namespace TgChatsStorage;

public class LiteDbMessageStorage : IMessageStorage
{
    private const string ChatsCollectionName = "chats";
    private readonly ILiteDatabase _db;

    public LiteDbMessageStorage(ILiteDatabase db)
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
            if (chat != null)
            {
                result = chat.Messages
                    .FirstOrDefault(x => x.Purpose == TgMessagePurpose.Menu);
            }
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
            if (chat == null)
            {
                chat = new ChatDocument {ChatId = msg.ChatId};
                chats.Insert(chat);
            }

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
            if (chat != null)
            {
                messages = chat.Messages
                    .Where(x => filter.Contains(x.Purpose))
                    .ToList();
            }
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
            if (chat == null)
            {
                return;
            }

            chat.Messages = chat.Messages
                .Where(x => !messageIds.Contains(x.MessageId))
                .ToList();
            chats.Update(chat);
        });
    }

    public async Task DeleteMessage(long chatId, long messageId)
    {
        await Task.Run(() =>
        {
            var chats = _db.GetCollection<ChatDocument>(ChatsCollectionName);
            var chat = chats
                .FindOne(x => x.ChatId == chatId);
            if (chat == null)
            {
                return;
            }

            chat.Messages = chat.Messages
                .Where(x => x.MessageId != messageId)
                .ToList();
            chats.Update(chat);
        });
    }
}
