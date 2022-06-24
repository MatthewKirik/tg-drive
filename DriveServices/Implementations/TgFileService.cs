using DataTransfer.Objects;
using Repositories;
using TgGateway.Abstractions;

namespace DriveServices.Implementations;

public class TgFileService : ITgFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IUserService _userService;
    private readonly IBotClient _botClient;

    public TgFileService(
        IFileRepository fileRepository,
        IUserService userService,
        IBotClient botClient)
    {
        _fileRepository = fileRepository;
        _userService = userService;
        _botClient = botClient;
    }

    public async Task<FileDto> AddFile(FileDto file)
    {
        var userInfo = await _userService.GetUserInfo(file.AddedByUserId);
        var forwardedId = await _botClient.CopyMessageUnmanaged(file.ChatId,
            (long)userInfo.StorageChannelId,
            file.MessageId);
        var forwardedFile = new FileDto
        {
            AddedByUserId = file.AddedByUserId,
            ChatId = (long)userInfo.StorageChannelId,
            Description = file.Description,
            DirectoryId = file.DirectoryId,
            MessageId = forwardedId,
            Name = file.Name,
            ReadAccessKey = file.ReadAccessKey
        };
        var savedFile = await _fileRepository.AddFile(forwardedFile);
        return savedFile;
    }

    public async Task<long> SendFile(long fileId)
    {
        var file = await _fileRepository.GetFile(fileId);
        var userInfo = await _userService.GetUserInfo(file.AddedByUserId);
        var copiedId =
            await _botClient.CopyMessageUnmanaged(file.ChatId,
                (long)userInfo.StorageChannelId,
                file.MessageId);
        return copiedId;
    }

    public async Task<IEnumerable<long>> SendFiles(
        long? directoryId = null,
        int? skip = null,
        int? take = null)
    {
        var files = await _fileRepository.GetFiles(directoryId, skip, take);
        if (!files.Any())
        {
            return new List<long>();
        }

        var userInfo = await _userService.GetUserInfo(files.First()
            .AddedByUserId);

        var chatId = files.First()
            .Id;
        var fileMessageIds = files.Select(x => x.MessageId);
        var sent =
            await _botClient
                .CopyMessagesUnmanaged(chatId,
                    (long)userInfo.StorageChannelId,
                    fileMessageIds)
                .ToListAsync();
        return sent;
    }

    public async Task<IEnumerable<long>> SendFilesByName(
        string name,
        long? directoryId = null,
        int? skip = null,
        int? take = null)
    {
        var files = await _fileRepository.GetFilesByName(name, directoryId, skip, take);
        if (!files.Any())
        {
            return new List<long>();
        }

        var userInfo = await _userService.GetUserInfo(files.First()
            .AddedByUserId);

        var chatId = files.First()
            .Id;
        var fileMessageIds = files.Select(x => x.MessageId);
        var sent =
            await _botClient
                .CopyMessagesUnmanaged(chatId,
                    (long)userInfo.StorageChannelId,
                    fileMessageIds)
                .ToListAsync();
        return sent;
    }

    public async Task<IEnumerable<long>> SendFilesByDescription(
        string description,
        long? directoryId = null,
        int? skip = null,
        int? take = null)
    {
        var files =
            await _fileRepository.GetFilesByDescription(description,
                directoryId,
                skip,
                take);
        if (!files.Any())
        {
            return new List<long>();
        }

        var userInfo = await _userService.GetUserInfo(files.First()
            .AddedByUserId);
        
        var chatId = files.First()
            .Id;
        var fileMessageIds = files.Select(x => x.MessageId);
        var sent =
            await _botClient
                .CopyMessagesUnmanaged(chatId,
                    (long)userInfo.StorageChannelId,
                    fileMessageIds)
                .ToListAsync();
        return sent;
    }
}
