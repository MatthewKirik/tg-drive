using AutoMapper;
using DataTransfer.Objects;
using EfRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Exceptions;

namespace EfRepositories.Repositories;

public class DirectoryRepository : IDirectoryRepository
{
    private readonly TgDriveContext _db;
    private readonly IMapper _mapper;

    public DirectoryRepository(TgDriveContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<DirectoryDto> AddDirectory(DirectoryDto directory)
    {
        var directoryEntity = _mapper.Map<DirectoryEntity>(directory);
        directoryEntity.Id = 0;
        _db.Directories.Add(directoryEntity);
        await _db.SaveChangesAsync();
        return _mapper.Map<DirectoryDto>(directoryEntity);
    }

    public async Task<DirectoryDto> GetDirectory(long directoryId)
    {
        var directory =
            await _db.Directories.FirstOrDefaultAsync(d => d.Id == directoryId);
        return _mapper.Map<DirectoryDto>(directory);
    }

    public async Task<IEnumerable<DirectoryDto>> GetChildren(long directoryId)
    {
        var children = await _db.Directories
            .Where(d => d.ParentId == directoryId)
            .ToListAsync();
        return _mapper.Map<List<DirectoryDto>>(children);
    }

    public async Task<IEnumerable<DirectoryDto>> GetRoot(long userId)
    {
        var rootMembers = await _db.Directories
            .Where(d => d.ParentId == null)
            .ToListAsync();
        return _mapper.Map<IEnumerable<DirectoryDto>>(rootMembers);
    }

    public async Task<DirectoryDto> RenameDirectory(long directoryId, string newName)
    {
        var directory =
            await _db.Directories.FirstOrDefaultAsync(d => d.Id == directoryId);
        if (directory == null)
        {
            throw new TgDirectoryNotFoundException(directoryId);
        }

        directory.Name = newName;
        await _db.SaveChangesAsync();
        return _mapper.Map<DirectoryDto>(directory);
    }

    public async Task<DirectoryDto> Remove(long directoryId)
    {
        var directory =
            await _db.Directories.FirstOrDefaultAsync(d => d.Id == directoryId);
        if (directory == null)
        {
            throw new TgDirectoryNotFoundException(directoryId);
        }

        _db.Remove(directory);
        await _db.SaveChangesAsync();
        return _mapper.Map<DirectoryDto>(directory);
    }

    public async Task SetAccessRights(
        long directoryId,
        long userId,
        bool? read,
        bool? write)
    {
        var directory =
            await _db.Directories.FirstOrDefaultAsync(d => d.Id == directoryId);
        if (directory == null)
        {
            throw new TgDirectoryNotFoundException(directoryId);
        }

        var access = await _db.DirectoriesAccesses.FirstOrDefaultAsync(da =>
            da.DirectoryId == directoryId && da.UserId == userId);
        if (access != null)
        {
            if (read != null)
            {
                access.HasReadAccess = read.Value;
            }

            if (write != null)
            {
                access.HasWriteAccess = write.Value;
            }
        }
        else
        {
            access = new DirectoryAccess
            {
                HasReadAccess = read ?? false, HasWriteAccess = write ?? false
            };
            _db.DirectoriesAccesses.Add(access);
        }

        await _db.SaveChangesAsync();
    }

    public async Task<bool> DoesUserHaveWriteAccess(long directoryId, long userId)
    {
        var directory =
            await _db.Directories.FirstOrDefaultAsync(d => d.Id == directoryId);
        if (directory == null)
        {
            throw new TgDirectoryNotFoundException(directoryId);
        }

        var access = await _db.DirectoriesAccesses.FirstOrDefaultAsync(da =>
            da.DirectoryId == directoryId && da.UserId == userId);
        return access is {HasWriteAccess: true};
    }

    public async Task<bool> DoesUserHaveReadAccess(long directoryId, long userId)
    {
        var directory =
            await _db.Directories.FirstOrDefaultAsync(d => d.Id == directoryId);
        if (directory == null)
        {
            throw new TgDirectoryNotFoundException(directoryId);
        }

        var access = await _db.DirectoriesAccesses.FirstOrDefaultAsync(da =>
            da.DirectoryId == directoryId && da.UserId == userId);
        return access is {HasReadAccess: true};
    }
}
