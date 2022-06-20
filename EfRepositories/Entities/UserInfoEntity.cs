namespace EfRepositories.Entities;

public class UserInfoEntity
{
    public long Id { get; set; }
    public long? StorageChannelId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
}