using DataTransfer.Objects;

namespace DriveServices;

public interface IUserService
{
    Task GetUserInfo(long userId);
    Task SetUserInfo(UserInfoDto userInfo);
}