using AutoMapper;
using COMP1640WebAPI.DataAccess.Models;
using COMP1640WebAPI.BusinesLogic.DTO;

namespace COMP1640WebAPI.BusinesLogic.MappingProfiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<UsersDTOPut, Users>().ForMember(dest => dest.roleId, opt => opt.MapFrom(scr => scr.roleId)).ForMember(dest => dest.userId, opt => opt.Ignore());
            CreateMap<Users, UsersDTOPut>().ForMember(dest => dest.roleId, opt => opt.MapFrom(scr => scr.roleId));

            CreateMap<UsersDTODelete, Users>().ForMember(dest => dest.userName, opt => opt.MapFrom(scr => scr.userName));
            CreateMap<Users, UsersDTODelete>().ForMember(dest => dest.userName, opt => opt.MapFrom(scr => scr.userName));

            CreateMap<UsersDTOPost, Users>().ForMember(dest => dest.userName, opt => opt.MapFrom(scr => scr.userName)).ForMember(dest => dest.userId, opt => opt.Ignore()).ForMember(dest => dest.roleId, opt => opt.Ignore());
            CreateMap<Users, UsersDTOPost>().ForMember(dest => dest.userName, opt => opt.MapFrom(scr => scr.userName));

            //CreateMap<StudentsDTOPostPut, Students>().ForMember(dest => dest.StudentName, opt => opt.MapFrom(scr => scr.StudentName)).ForMember(dest => dest.StudentId, opt => opt.Ignore());
            //CreateMap<Students, StudentsDTOPostPut>().ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.StudentName));
        }
    }
}
