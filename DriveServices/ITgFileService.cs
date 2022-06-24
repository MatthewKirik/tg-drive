using DataTransfer.Objects;

namespace DriveServices;

public interface ITgFileService
{
    Task<FileDto> AddFile(FileDto file);
    Task<long> SendFile(long fileId);

    Task<IEnumerable<long>> SendFiles(
        long? directoryId = null,
        int? skip = null,
        int? take = null);

    Task<IEnumerable<long>> SendFilesByName(
        string name,
        long? directoryId = null,
        int? skip = null,
        int? take = null);

    Task<IEnumerable<long>> SendFilesByDescription(
        string description,
        long? directoryId = null,
        int? skip = null,
        int? take = null);
}
