using LiteDB;
using TgChatsStorage.Models;
using TgGateway.Abstractions;
using TgGateway.Models;

namespace TgChatsStorage;

public class LiteDbMessageStorage : IMessageStorage
{
    private const string ChatsCollectionName = "chats";
    private const string UsersCollectionName = "users";
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

    public async Task<TgMessage?> GetLastMessage(
        long chatId,
        IEnumerable<TgMessagePurpose>? filter = null)
    {
        TgMessage? result = null;
        await Task.Run(() =>
        {
            var chats = _db.GetCollection<ChatDocument>(ChatsCollectionName);
            var chat = chats
                .FindOne(x => x.ChatId == chatId);
            if (chat != null)
            {
                if (filter != null)
                {
                    result = chat.Messages
                        .LastOrDefault(m => filter.Contains(m.Purpose));
                }
                else
                {
                    result = chat.Messages
                        .LastOrDefault();
                }
            }
        });
        return result;
    }

    public async Task SetUserState(long chatId, long userId, TgUserState state)
    {
        await Task.Run(() =>
        {
            var document = new UserDocument
            {
                ChatId = state.chatId, UserId = state.userId, State = state.values
            };
            var users = _db.GetCollection<UserDocument>(UsersCollectionName);
            var existingState = users.FindOne(u => u.Id == document.Id);
            if (existingState != null)
            {
                existingState.State = state.values;
                users.Update(existingState);
            }
            else
            {
                users.Insert(document);
            }
        });
    }

    public async Task<TgUserState> GetUserState(long chatId, long userId)
    {
        TgUserState? result = null;
        await Task.Run(() =>
        {
            var users = _db.GetCollection<UserDocument>(UsersCollectionName);
            var document = new UserDocument {ChatId = chatId, UserId = userId};
            var existingState = users.FindOne(u => u.Id == document.Id);
            if (existingState != null)
            {
                result = new TgUserState(userId, chatId, existingState.State);
            }
            else
            {
                document.State = new Dictionary<string, string>();
                users.Insert(document);
                result = new TgUserState(userId, chatId, document.State);
            }
        });
        return result!;
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

    public async Task<IEnumerable<TgMessage>> GetMessages(
        long chatId,
        IEnumerable<TgMessagePurpose>? filter = null)
    {
        IEnumerable<TgMessage> messages = new List<TgMessage>();
        await Task.Run(() =>
        {
            var chats = _db.GetCollection<ChatDocument>(ChatsCollectionName);
            var chat = chats
                .FindOne(x => x.ChatId == chatId);
            if (chat != null)
            {
                if (filter != null)
                {
                    messages = chat.Messages
                        .Where(x => filter.Contains(x.Purpose))
                        .ToList();
                }
                else
                {
                    messages = chat.Messages;
                }
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
