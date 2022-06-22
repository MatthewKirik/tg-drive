using DataTransfer.Objects;

namespace Repositories;

public interface IUserRepository
{
    Task GetUserInfo(long userId);
    Task SetUserInfo(UserInfoDto userInfo);
}
