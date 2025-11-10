using AutoMapper;
using InternApi.ModelDTO;
using InternApi.ModelEntity;

namespace InternApi.Mapping
{
    public class Mapperprofile : Profile
    {
        public Mapperprofile() {
            CreateMap<Intern, InternDTO>();
            CreateMap<InternDTO, Intern>();
        }
    }
}
