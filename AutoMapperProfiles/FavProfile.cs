using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.Models;
using AutoMapper;

namespace MiniLibraryManagementSystem.AutoMapperProfiles
{
    public class FavProfile : Profile 
    {
        public FavProfile() { 
            CreateMap<Favorite, FavsDto>()
                .ForMember(fd => fd.FavId , f => f.MapFrom(f => f.Id));
        }
    }
}

