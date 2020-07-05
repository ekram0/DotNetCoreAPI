using AutoMapper;
using CoreCodeCamp.Model;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(des => des.Venue, src => src.MapFrom(s => s.Location.VenueName));

            CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(o=>o.Camp , op =>op.Ignore())
                .ForMember(p=>p.Speaker , tp=>tp.Ignore());

            CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}
