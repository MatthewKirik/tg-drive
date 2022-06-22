using AutoMapper;
using DataTransfer.Objects;
using EfRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Exceptions;

namespace EfRepositories.Repositories;

public class FileRepository : IFileRepository
{
    private readonly TgDriveContext _db;
    private readonly IMapper _mapper;

    public FileRepository(TgDriveContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<FileDto> AddFile(FileDto file)
    {
        var directory =
            await _db.Directories.FirstOrDefaultAsync(d => d.Id == file.DirectoryId);
        if (directory == null)
        {
            throw new TgDirectoryNotFoundException(file.DirectoryId);
        }

        var fileEntity = _mapper.Map<FileEntity>(file);
        _db.Files.Add(fileEntity);
        await _db.SaveChangesAsync();
        return _mapper.Map<FileDto>(fileEntity);
    }

    public async Task<FileDto> GetFile(long fileId)
    {
        var file = await _db.Files.FirstOrDefaultAsync(f => f.Id == fileId);
        if (file == null)
        {
            throw new TgFileNotFoundException(fileId);
        }

        var fileEntity = _mapper.Map<FileEntity>(file);
        return _mapper.Map<FileDto>(fileEntity);
    }

    public async Task<IEnumerable<FileDto>> GetFiles(
        long? directoryId = null,
        int? skip = null,
        int? take = null)
    {
        var filesQuery =
            _db.Files.Where(f => directoryId == null || f.DirectoryId == directoryId);
        if (skip != null)
        {
            filesQuery = filesQuery.Skip(skip.Value);
        }

        if (take != null)
        {
            filesQuery = filesQuery.Take(take.Value);
        }

        var files = await filesQuery.ToListAsync();
        return _mapper.Map<IEnumerable<FileDto>>(files);
    }

    public async Task<IEnumerable<FileDto>> GetFilesByName(
        string name,
        long? directoryId = null,
        int? skip = null,
        int? take = null)
    {
        var filesQuery =
            _db.Files.Where(f => directoryId == null || f.DirectoryId == directoryId)
                .Where(f => EF.Functions.Like(f.Name, $"%{name}%"));
        if (skip != null)
        {
            filesQuery = filesQuery.Skip(skip.Value);
        }

        if (take != null)
        {
            filesQuery = filesQuery.Take(take.Value);
        }

        var files = await filesQuery.ToListAsync();
        return _mapper.Map<IEnumerable<FileDto>>(files);
    }

    public async Task<IEnumerable<FileDto>> GetFilesByDescription(
        string description,
        long? directoryId = null,
        int? skip = null,
        int? take = null)
    {
        var filesQuery =
            _db.Files.Where(f => directoryId == null || f.DirectoryId == directoryId)
                .Where(f =>
                    f.Description != null &&
                    EF.Functions.Like(f.Description, $"%{description}%"));
        if (skip != null)
        {
            filesQuery = filesQuery.Skip(skip.Value);
        }

        if (take != null)
        {
            filesQuery = filesQuery.Take(take.Value);
        }

        var files = await filesQuery.ToListAsync();
        return _mapper.Map<IEnumerable<FileDto>>(files);
    }

    public async Task<FileDto> ChangeDescription(long fileId, string newDescription)
    {
        var file = await _db.Files.FirstOrDefaultAsync(f => f.Id == fileId);
        if (file == null)
        {
            throw new TgFileNotFoundException(fileId);
        }

        file.Description = newDescription;
        await _db.SaveChangesAsync();
        return _mapper.Map<FileDto>(file);
    }

    public async Task<FileDto> Remove(long fileId)
    {
        var file = await _db.Files.FirstOrDefaultAsync(f => f.Id == fileId);
        if (file == null)
        {
            throw new TgFileNotFoundException(fileId);
        }

        _db.Files.Remove(file);
        await _db.SaveChangesAsync();
        return _mapper.Map<FileDto>(file);
    }
}
