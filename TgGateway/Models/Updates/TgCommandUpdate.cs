namespace TgGateway.Models.Updates;

public record TgCommandUpdate : TgUpdate
{
    public string Command { get; init; }
}