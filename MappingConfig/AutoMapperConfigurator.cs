using AutoMapper;

namespace MappingConfig;

public class AutMapperConfigurator
{
    public static class AutoMapperConfigurator
    {
        public static IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                // Add mappings here like this:  mc.AddProfile(new MappingProfile());
            });

            var mapper = mapperConfig.CreateMapper();
            return mapper;
        }
    }
}
