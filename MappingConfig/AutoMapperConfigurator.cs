using AutoMapper;
using EfRepositories.Mappings;

namespace MappingConfig;

public static class AutoMapperConfigurator
{
    public static IMapper GetMapper()
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new EntityToDtoMappingProfile());
        });

        var mapper = mapperConfig.CreateMapper();
        return mapper;
    }
}
