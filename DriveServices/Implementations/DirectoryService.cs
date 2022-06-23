using DataTransfer.Objects;
using Repositories;

namespace DriveServices.Implementations;

public class DirectoryService : IDirectoryService
{
    private readonly IDirectoryRepository _directoryRepository;

    public DirectoryService(IDirectoryRepository directoryRepository)
    {
        _directoryRepository = directoryRepository;
    }

    public async Task<DirectoryDto> AddDirectory(DirectoryDto directory)
    {
        var added = await _directoryRepository.AddDirectory(directory);
        return added;
    }

    public async Task<DirectoryDto> GetDirectory(long directoryId)
    {
        var directory = await _directoryRepository.GetDirectory(directoryId);
        return directory;
    }

    public async Task<IEnumerable<DirectoryDto>> GetChildren(long directoryId)
    {
        var children = await _directoryRepository.GetChildren(directoryId);
        return children;
    }

    public async Task<IEnumerable<DirectoryDto>> GetRoot(long userId)
    {
        var rootDirectories = await _directoryRepository.GetRoot(userId);
        return rootDirectories;
    }

    public async Task<DirectoryDto> RenameDirectory(long directoryId, string newName)
    {
        var renamed = await _directoryRepository.RenameDirectory(directoryId, newName);
        return renamed;
    }

    public async Task<DirectoryDto> Remove(long directoryId)
    {
        var removed = await _directoryRepository.Remove(directoryId);
        return removed;
    }

    public async Task SetAccessRights(
        long directoryId,
        long userId,
        bool? read,
        bool? write)
    {
        await _directoryRepository.SetAccessRights(directoryId, userId, read, write);
    }
}
