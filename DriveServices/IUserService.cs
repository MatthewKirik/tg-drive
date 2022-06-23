using DataTransfer.Objects;

namespace DriveServices;

public interface IUserService
{
    Task<UserInfoDto?> GetUserInfo(long userId);
    Task<UserInfoDto> SetUserInfo(UserInfoDto userInfo);
}
