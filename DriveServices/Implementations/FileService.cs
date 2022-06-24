using DataTransfer.Objects;
using Repositories;

namespace DriveServices.Implementations;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;

    public FileService(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<FileDto> ChangeDescription(long fileId, string newDescription)
    {
        var edited = await _fileRepository.ChangeDescription(fileId, newDescription);
        return edited;
    }

    public async Task<FileDto> Remove(long fileId)
    {
        var removed = await _fileRepository.Remove(fileId);
        return removed;
    }

    public async Task<FileDto> GetFile(long fileId)
    {
        var file = await _fileRepository.GetFile(fileId);
        return file;
    }

    public async Task<IEnumerable<FileDto>> GetFiles(
        long? directoryId = null,
        int? skip = null,
        int? take = null)
    {
        var files = await _fileRepository.GetFiles(directoryId, skip, take);
        return files;
    }

    public async Task<IEnumerable<FileDto>> GetFilesByName(
        string name,
        long? directoryId = null,
        int? skip = null,
        int? take = null)
    {
        var files = await _fileRepository.GetFilesByName(name, directoryId, skip, take);
        return files;
    }

    public async Task<IEnumerable<FileDto>> GetFilesByDescription(
        string description,
        long? directoryId = null,
        int? skip = null,
        int? take = null)
    {
        var files =
            await _fileRepository.GetFilesByName(description, directoryId, skip, take);
        return files;
    }
}
