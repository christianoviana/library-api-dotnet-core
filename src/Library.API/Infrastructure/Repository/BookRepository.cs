using Library.API.Domain.Models;
using Library.API.Infrastructure.Entity;
using Library.API.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Infrastructure.Repository
{
    public class BookRepository : RepositorySync<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<Book> GetBooksWithAuthors()
        {
            IQueryable<Book> books = this._context.Books
                                         .Include(b => b.BookAuthors)
                                         .ThenInclude(b => b.Author);

            return books.ToList();
        }

        public Book GetBooksWithAuthorsById (Guid id)
        {
            IQueryable<Book> books = this._context.Books
                                         .Include(b => b.BookAuthors)
                                         .ThenInclude(b => b.Author)
                                         .Where(b => b.Id == id);

            return books.FirstOrDefault();
        }

        public IQueryable<Book> GetBooksWithAuthorsAsQueryable()
        {
            IQueryable<Book> books = this._context.Books
                                       .Include(b => b.BookAuthors)
                                       .ThenInclude(b => b.Author);

            return books.AsNoTracking();
        }
    }
}
