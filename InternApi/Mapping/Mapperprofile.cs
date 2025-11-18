using AutoMapper;
using InternApi.ModelDTO;
using InternApi.ModelEntity;

namespace InternApi.Mapping
{
    public class MapperProfile : Profile
    {
        /// <summary>
        /// Used to convert Intern Model from DTO to Entity and from Entity to DTO. 
        /// </summary>
        public MapperProfile() {
            CreateMap<Intern, InternDTO>();
            CreateMap<InternDTO, Intern>();
        }
    }
}
