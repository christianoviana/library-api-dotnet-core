using Library.API.Domain.Models;
using Library.API.Infrastructure.Entity;
using Library.API.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Infrastructure.Repository
{
    public class AuthorRepository : RepositorySync<Author>, IAuthorRepository
    {
        public AuthorRepository(ApplicationDbContext context) : base(context)
        {            
        }

        public IEnumerable<Book> GetAuthorsBooksById(Guid authorId)
        {

            IQueryable<Book> books = this._context.Authors
                                        .Include(b => b.BookAuthors)
                                        .ThenInclude(b => b.Book)
                                        .SelectMany(b => b.BookAuthors)
                                        .Where(ba => ba.AuthorId == authorId)
                                        .Select(e => e.Book);

            return books.ToList();
        }
    }
}
