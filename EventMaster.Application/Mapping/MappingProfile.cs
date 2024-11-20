namespace EventMaster.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserLoginDto>().ReverseMap();
            CreateMap<User, UnregisterUserDto>().ReverseMap();
            CreateMap<User, UserRegistrationDto>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<Event, EventDto>().ReverseMap();
            CreateMap<LoginResultDto, User>().ReverseMap();
        }
    }
}
