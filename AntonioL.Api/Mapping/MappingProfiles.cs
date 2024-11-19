using AutoMapper;
using AntonioL.Share.Dtos;
using Microsoft.AspNetCore.Routing.Matching;
using AntonioL.Models.PruebaCoink;

namespace AntonioL.Api.Mapping
{
    /// <summary>
    /// Mapeo con Dtos
    /// </summary>
    public class MappingProfiles : Profile
    {
        /// <summary>
        /// asignaciones
        /// </summary>
        public MappingProfiles()
        {


            CreateMap<Pais, PaisDto>().ReverseMap();
            CreateMap<Departamento, DepartamentoDto>().ReverseMap();
            CreateMap<Municipio, MunicipioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();


        }
    }
}
