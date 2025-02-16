using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.Models;
using AutoMapper;

namespace MiniLibraryManagementSystem.AutoMapperProfiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDetailsDto>()
                .ForMember(bt => bt.BookId , b => b.MapFrom(b => b.Id));
        }
    }
}
