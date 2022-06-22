using DataTransfer.Objects;
using Repositories;

namespace DriveServices.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserInfoDto?> GetUserInfo(long userId)
    {
        var userInfo = await _userRepository.GetUserInfo(userId);
        return userInfo;
    }

    public async Task<UserInfoDto> SetUserInfo(UserInfoDto userInfo)
    {
        var setUserInfo = await _userRepository.SetUserInfo(userInfo);
        return setUserInfo;
    }
}
