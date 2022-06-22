namespace EfRepositories.Entities;

public class DirectoryAccess
{
    public long UserId { get; set; }
    public long DirectoryId { get; set; }
    public DirectoryEntity Directory { get; set; }
    public bool HasWriteAccess { get; set; }
    public bool HasReadAccess { get; set; }
}
