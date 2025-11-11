using AutoMapper;
using InternApi.ModelDTO;
using InternApi.ModelEntity;

namespace InternApi.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile() {
            CreateMap<Intern, InternDTO>();
            CreateMap<InternDTO, Intern>();
        }
    }
}
