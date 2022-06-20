using DataTransfer.Objects;

namespace DriveServices;

public interface IDirectoryService
{
    Task<DirectoryDto> AddDirectory(DirectoryDto directory);
    Task<DirectoryDto> GetDirectory(long directoryId);
    Task<IEnumerable<DirectoryDto>> GetChildren(long directoryId);
    Task<IEnumerable<DirectoryDto>> GetRoot(long userId);
    Task<DirectoryDto> RenameDirectory(long directoryId, string newName);
    Task<DirectoryDto> Remove(long directoryId);
    Task SetAccessRights(long directoryId, long userId, bool? read, bool? write);
}