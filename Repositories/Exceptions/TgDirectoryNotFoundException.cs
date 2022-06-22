namespace Repositories.Exceptions;

public class TgDirectoryNotFoundException : EntityNotFoundException
{
    public TgDirectoryNotFoundException(long directoryId)
        : base("Directory", directoryId)
    {
    }
}
