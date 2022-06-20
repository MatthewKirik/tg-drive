using DataTransfer.Objects;

namespace DriveServices;

public interface IFileService
{
    Task<FileDto> ChangeDescription(long id, string newDescription);
    Task<FileDto> Remove(long fileId, bool hardDelete = false);

    Task<IEnumerable<FileDto>> GetFiles(
        long? directoryId = null,
        int? skip = null,
        int? take = null);

    Task<IEnumerable<FileDto>> GetFilesByName(
        string name,
        long? directoryId = null,
        int? skip = null,
        int? take = null);

    Task<IEnumerable<FileDto>> GetFilesByDescription(
        string description,
        long? directoryId = null,
        int? skip = null,
        int? take = null);
}