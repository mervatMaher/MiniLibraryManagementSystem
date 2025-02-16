using AutoMapper;
using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.Models;

namespace MiniLibraryManagementSystem.AutoMapperProfiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Author, AuthorDto>();
        }
    }
}
