using TgGateway.Models.Updates;

namespace TgGateway.Abstractions;

public interface IUpdateHandler
{
    Task HandleCallback(TgCallbackUpdate update);
    Task HandleMessage(TgMessageUpdate update);
    Task HandleCommand(TgCommandUpdate update);
    Task HandleError(Exception error);
}
