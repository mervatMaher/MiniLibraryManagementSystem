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

            CreateMap<Book, BookWithAllAuthorsDto>()
                .ForMember(bt => bt.BookId, b => b.MapFrom(b => b.Id))
                .ForMember(bt => bt.BookTitle, b => b.MapFrom(b => b.Title))
                .ForMember(bt => bt.Authors, b => b.MapFrom(b => b.BookAuthors));
        }
    }
}
