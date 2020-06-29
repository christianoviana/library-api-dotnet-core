using Library.API.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Infrastructure.Repository.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        IEnumerable<Book> GetBooksWithAuthors();
        IQueryable<Book> GetBooksWithAuthorsAsQueryable();
        Book GetBooksWithAuthorsById (Guid id);
    }
}
