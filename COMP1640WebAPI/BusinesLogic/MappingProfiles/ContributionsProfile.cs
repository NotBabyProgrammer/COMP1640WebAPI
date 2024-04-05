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
                                                            .ForMember(dest => dest.endDate, opt => opt.Ignore())
                                                            .ForMember(dest => dest.status, opt => opt.Ignore())
                                                            .ForMember(dest => dest.approval, opt => opt.Ignore());
            CreateMap<Contributions, ContributionsDTOPost>().ForMember(dest => dest.userId, opt => opt.MapFrom(scr => scr.userId));

            CreateMap<ContributionsDTOReview, Contributions>().ForMember(dest => dest.approval, opt => opt.MapFrom(scr => scr.approval))
                                                           .ForMember(dest => dest.userId, opt => opt.Ignore())
                                                           .ForMember(dest => dest.contributionId, opt => opt.Ignore())
                                                           .ForMember(dest => dest.submissionDate, opt => opt.Ignore())
                                                           .ForMember(dest => dest.endDate, opt => opt.Ignore())
                                                           .ForMember(dest => dest.facultyName, opt => opt.Ignore());
            CreateMap<Contributions, ContributionsDTOReview>().ForMember(dest => dest.approval, opt => opt.MapFrom(scr => scr.approval));
        }
    }
}
