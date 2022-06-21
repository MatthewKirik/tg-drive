namespace TgGateway.Models.Updates;

public abstract record TgUpdate
{
    public long SenderId { get; init; }
    public long ChatId { get; init; }
    public DateTime DateTime { get; init; }
}