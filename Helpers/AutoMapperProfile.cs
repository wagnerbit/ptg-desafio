using AutoMapper;
using WebApi.Entities;
using WebApi.Mapping;

namespace WebApi.Helpers {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile () {
            CreateMap<User, UserDto> ();
            CreateMap<UserDto, User> ();
            CreateMap<Phone, PhoneDto> ();
            CreateMap<PhoneDto, Phone> ();
        }
    }
}