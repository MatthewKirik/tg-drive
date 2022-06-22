namespace Repositories.Exceptions;

public class TgFileNotFoundException : EntityNotFoundException
{
    public TgFileNotFoundException(long fileId)
        : base("File", fileId)
    {
    }
}
