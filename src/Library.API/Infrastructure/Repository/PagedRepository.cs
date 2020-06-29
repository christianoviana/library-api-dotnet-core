using Library.API.Domain.Parameters;
using Library.API.Infrastructure.Entity;
using Library.API.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Library.API.Infrastructure.Repository
{
    public class PagedRepository<T> : RepositoryAsync<T>, IPagedRepository<T> where T : class
    {      
        public PagedRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<T>> GetAllPagedAsync(PaginationParams parameters)
        {
            IEnumerable<T> data = await this.GetAllAsync();
            return data.Skip((parameters.page - 1) * parameters.size).Take(parameters.size);
        }

        public async Task<IEnumerable<T>> GetAllPagedAsync(PaginationParams parameters, Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> data = await this.GetWhere(predicate);
            return data.Skip((parameters.page - 1) * parameters.size).Take(parameters.size);
        }
    }
}
