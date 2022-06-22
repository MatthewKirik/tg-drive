using AutoMapper;
using DataTransfer.Objects;
using EfRepositories.Entities;

namespace EfRepositories.Mappings;

public class EntityToDtoMappingProfile : Profile
{
    public EntityToDtoMappingProfile()
    {
        CreateMap<DirectoryEntity, DirectoryDto>().ReverseMap();
        CreateMap<FileEntity, FileDto>().ReverseMap();
        CreateMap<UserInfoDto, UserInfoDto>().ReverseMap();
    }
}
