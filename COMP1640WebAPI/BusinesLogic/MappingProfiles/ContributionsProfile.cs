using AutoMapper;
using COMP1640WebAPI.DataAccess.Models;
using COMP1640WebAPI.BusinesLogic.DTO;

namespace COMP1640WebAPI.BusinesLogic.MappingProfiles
{
    public class ContributionsProfile : Profile
    {
        public ContributionsProfile()
        {
            CreateMap<ContributionsDTOPost, Contributions>().ForMember(dest => dest.userId, opt => opt.MapFrom(scr => scr.userId))
                                                            .ForMember(dest => dest.contributionId, opt => opt.Ignore())
                                                            .ForMember(dest => dest.filePaths, opt => opt.Ignore())
                                                            .ForMember(dest => dest.imagePaths, opt => opt.Ignore())
                                                            .ForMember(dest => dest.submissionDate, opt => opt.Ignore())
                                                            .ForMember(dest => dest.closureDate, opt => opt.Ignore())
                                                            .ForMember(dest => dest.status, opt => opt.Ignore())
                                                            .ForMember(dest => dest.approval, opt => opt.Ignore());
            CreateMap<Contributions, ContributionsDTOPost>().ForMember(dest => dest.userId, opt => opt.MapFrom(scr => scr.userId));

            CreateMap<ContributionsDTOPut, Contributions>().ForMember(dest => dest.title, opt => opt.MapFrom(scr => scr.title))
                                                           .ForMember(dest => dest.userId, opt => opt.Ignore())
                                                           .ForMember(dest => dest.contributionId, opt => opt.Ignore())
                                                           .ForMember(dest => dest.submissionDate, opt => opt.Ignore())
                                                           .ForMember(dest => dest.closureDate, opt => opt.Ignore())
                                                           .ForMember(dest => dest.facultyName, opt => opt.Ignore());
            CreateMap<Contributions, ContributionsDTOPut>().ForMember(dest => dest.title, opt => opt.MapFrom(scr => scr.title));
        }
    }
}
