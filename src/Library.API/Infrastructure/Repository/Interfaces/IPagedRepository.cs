using Library.API.Domain.Parameters;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Library.API.Infrastructure.Repository.Interfaces
{
    public interface IPagedRepository<T>: IRepositoryAsync<T>
    {
        Task<IEnumerable<T>> GetAllPagedAsync(PaginationParams parameters);
        Task<IEnumerable<T>> GetAllPagedAsync(PaginationParams parameters, Expression<Func<T, bool>> predicate);
    }
}
