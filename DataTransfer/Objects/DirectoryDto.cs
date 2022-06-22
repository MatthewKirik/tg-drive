namespace DataTransfer.Objects;

public class DirectoryDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public long OwnerId { get; set; }
    public string? ReadAccessKey { get; set; }
    public string? WriteAccessKey { get; set; }
    public long? ParentId { get; set; }
}
