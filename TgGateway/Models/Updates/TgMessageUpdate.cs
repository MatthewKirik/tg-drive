namespace TgGateway.Models.Updates;

public record TgMessageUpdate : TgUpdate
{
    public TgMessage Message { get; init; }
}