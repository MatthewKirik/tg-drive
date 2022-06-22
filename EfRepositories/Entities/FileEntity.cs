namespace EfRepositories.Entities;

public class FileEntity
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public long AddedByUserId { get; set; }
    public long MessageId { get; set; }
    public long ChannelId { get; set; }
    public string? ReadAccessKey { get; set; }

    public long DirectoryId { get; set; }
    public DirectoryEntity Directory { get; set; }

    public FileEntity()
    {
        Directory = default!;
        Name = default!;
    }
}
