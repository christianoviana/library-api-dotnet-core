using Library.API.Domain.Models;
using System;
using System.Collections.Generic;

namespace Library.API.Infrastructure.Repository.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        IEnumerable<Book> GetAuthorsBooksById(Guid authorId);
    }
}
