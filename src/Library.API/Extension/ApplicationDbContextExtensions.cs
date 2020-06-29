using Library.API.Domain.Models;
using Library.API.Infrastructure.Entity;
using Library.API.Infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using System;
using System.Linq;

namespace Library.API.Extension
{
    public static class ApplicationDbContextExtensions
    {
        public static void InitializerDatabase(this IApplicationBuilder app, ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Set<BookAuthor>().Any())
            {
                var Shirzad = new Author { Id = Guid.NewGuid(), FirstName = "Shirzad", LastName = "Chamine", Birthday = new DateTime(1972, 1, 1) };
                var Simon = new Author { Id = Guid.NewGuid(), FirstName = "Simon", LastName = "Sinek", Birthday = new DateTime(1874, 11, 30) };
                var Tudor = new Author { Id = Guid.NewGuid(), FirstName = " C.", LastName = "J.Tudor", Birthday = new DateTime(1956, 5, 22) };
                var Arthur = new Author { Id = Guid.NewGuid(), FirstName = "Arthur", LastName = "Alvin", Birthday = new DateTime(1959, 10, 2) };

                var book1 = new Book
                {
                    Id = Guid.NewGuid(),
                    Name = "Comece pelo porquê: Como grandes líderes inspiram pessoas e equipes a agir",
                    Description = "Nesse livro, você verá como pensam, agem e se comunicam os líderes que exercem a maior influência, e também descobrirá um modelo a partir do qual as pessoas podem ser inspiradas, movimentos podem ser criados e organizações, construídas. E tudo isso começa pelo porquê.",
                    Pages = 256,
                    Date = new DateTime(2018, 10, 16)
                };

                var book2 = new Book
                {
                    Id = Guid.NewGuid(),
                    Name = "Inteligência positiva",
                    Description = "Nesse livro, você verá como pensam, agem e se comunicam os líderes que exercem a maior influência, e também descobrirá um modelo a partir do qual as pessoas podem ser inspiradas, movimentos podem ser criados e organizações, construídas. E tudo isso começa pelo porquê.",
                    Pages = 144,
                    Date = new DateTime(2018, 10, 16)
                };

                var book3 = new Book
                {
                    Id = Guid.NewGuid(),
                    Name = "O homem de giz eBook",
                    Description = "Nesse livro, você verá como pensam, agem e se comunicam os líderes que exercem a maior influência, e também descobrirá um modelo a partir do qual as pessoas podem ser inspiradas, movimentos podem ser criados e organizações, construídas. E tudo isso começa pelo porquê.",
                    Pages = 256,
                    Date = new DateTime(2018, 10, 16)
                };


                context.Add(new BookAuthor() { Author = Shirzad , Book = book1});
                context.Add(new BookAuthor() { Author = Simon, Book = book2});
                context.Add(new BookAuthor() { Author = Tudor , Book = book3});
                context.Add(new BookAuthor() { Author = Arthur , Book = book3});

                context.SaveChanges();
            }
        }
    }
}
