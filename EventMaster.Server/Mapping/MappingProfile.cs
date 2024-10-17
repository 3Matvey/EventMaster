using AutoMapper;
using EventMaster.Server.Entities;
using EventMaster.Server.Dto;

namespace EventMaster.Server.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<User, UserLoginDto>();
            CreateMap<User, UnregisterUserDto>();
            CreateMap<User, UserRegistrationDto>();
            CreateMap<Event, EventDto>();
        }
    }
}
