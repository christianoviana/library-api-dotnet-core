using AutoMapper;
using Library.API.Domain.DTO;
using Library.API.Domain.Models;
using Library.API.Extension;
using System;

namespace Library.API.Domain.Mappers.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Author, AuthorDTO>()
                    .ForMember(d => d.Name, o => o.MapFrom(d => string.Concat(d.FirstName, " ", d.LastName)))
                    .ForMember(d => d.Age, o => o.MapFrom(d => d.Birthday.GetAge()));

            CreateMap<Book, BookWithAuthorsDTO>()
                   .ForMember(d => d.Authors, o => o.MapFrom(d => d.BookAuthors))
                   .ForMember(d => d.Book, o => o.MapFrom(d => d));

            CreateMap<Book, BookDTO>();

            CreateMap<BookAuthor, AuthorDTO>()
                   .ForMember(d => d.Id, o => o.MapFrom(d => d.Author.Id))
                   .ForMember(d => d.Name, o => o.MapFrom(d => string.Concat(d.Author.FirstName, " ", d.Author.LastName)))
                   .ForMember(d => d.Age, o => o.MapFrom(d => d.Author.Birthday.GetAge()));

            CreateMap<BookAuthor, BookDTO>()
                 .ForMember(d => d.Id, o => o.MapFrom(d => d.Book.Id))
                 .ForMember(d => d.Name, o => o.MapFrom(d => d.Book.Name))
                 .ForMember(d => d.Description, o => o.MapFrom(d => d.Book.Description))
                 .ForMember(d => d.Pages, o => o.MapFrom(d => d.Book.Pages))
                 .ForMember(d => d.Date, o => o.MapFrom(d => d.Book.Date));

            CreateMap<AuthorCreationDTO, Author>();
            CreateMap<AuthorUpdateDTO, Author>();

            CreateMap<BookDTO, Book>();
        }
    }
}
