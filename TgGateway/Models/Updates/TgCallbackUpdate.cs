namespace TgGateway.Models.Updates;

public record TgCallbackUpdate : TgUpdate
{
    public string MenuId { get; init; }
    public string ButtonId { get; init; }
    public IEnumerable<string> Arguments { get; init; }
}