using DataTransfer.Objects;

namespace Repositories;

public interface IUserRepository
{
    Task<UserInfoDto?> GetUserInfo(long userId);
    Task<UserInfoDto> SetUserInfo(UserInfoDto userInfo);
}
