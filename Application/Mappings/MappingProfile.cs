using AutoMapper;
using Application.DTOs;
using Domain.Entities;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegistroVentaDto, RegistroVentaEDS>().ReverseMap();
        }
    }
}
