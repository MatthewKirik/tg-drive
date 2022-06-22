namespace Repositories.Exceptions;

public class TgUserInfoNotFoundException : EntityNotFoundException
{
    public TgUserInfoNotFoundException(long userInfoId)
        : base("User info", userInfoId)
    {
    }
}
