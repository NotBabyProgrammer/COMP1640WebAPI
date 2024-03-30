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
                                                            .ForMember(dest => dest.filePath, opt => opt.Ignore())
                                                            .ForMember(dest => dest.imagePath, opt => opt.Ignore())
                                                            .ForMember(dest => dest.submissionDate, opt => opt.Ignore())
                                                            .ForMember(dest => dest.closureDate, opt => opt.Ignore())
                                                            .ForMember(dest => dest.status, opt => opt.Ignore())
                                                            .ForMember(dest => dest.approval, opt => opt.Ignore());
            CreateMap<Contributions, ContributionsDTOPost>().ForMember(dest => dest.userId, opt => opt.MapFrom(scr => scr.userId));
        }
    }
}
