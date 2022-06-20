using DataTransfer.Objects;

namespace DriveServices;

public interface ITgFileService
{
    Task<FileDto> AddFile(FileDto file);
    Task<FileDto> GetFile(long fileId, long destinationChatId);
    Task<IEnumerable<FileDto>> SendFiles(
        long destinationChatId,
        long? directoryId = null,
        int? skip = null,
        int? take = null);
    Task<IEnumerable<FileDto>> SendFilesByName(
        string name,
        long destinationChatId,
        long? directoryId = null,
        int? skip = null,
        int? take = null);
    Task<IEnumerable<FileDto>> SendFilesByDescription(
        string description,
        long destinationChatId,
        long? directoryId = null,
        int? skip = null,
        int? take = null);
}