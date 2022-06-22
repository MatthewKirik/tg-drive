namespace EfRepositories.Entities;

public class DirectoryEntity
{
    public DirectoryEntity()
    {
        Name = default!;
    }

    public long Id { get; set; }
    public string Name { get; set; }
    public long OwnerId { get; set; }
    public string? ReadAccessKey { get; set; }
    public string? WriteAccessKey { get; set; }

    public long? ParentId { get; set; }
    public DirectoryEntity? Parent { get; set; }

    public ICollection<DirectoryEntity> Children { get; } = new List<DirectoryEntity>();
    public ICollection<FileEntity> Files { get; } = new List<FileEntity>();
    public ICollection<DirectoryAccess> Accesses { get; } = new List<DirectoryAccess>();
}
