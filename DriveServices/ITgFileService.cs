using DataTransfer.Objects;

namespace DriveServices;

public interface ITgFileService
{
    Task<FileDto> AddFile(FileDto file, long destinationChatId);
    Task<long> SendFile(long fileId, long destinationChatId);

    Task<IEnumerable<long>> SendFiles(
        long destinationChatId,
        long? directoryId = null,
        int? skip = null,
        int? take = null);

    Task<IEnumerable<long>> SendFilesByName(
        string name,
        long destinationChatId,
        long? directoryId = null,
        int? skip = null,
        int? take = null);

    Task<IEnumerable<long>> SendFilesByDescription(
        string description,
        long destinationChatId,
        long? directoryId = null,
        int? skip = null,
        int? take = null);
}
