using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Library.API.Infrastructure.Repository.Interfaces
{
    public interface IRepository<T>
    {
        void Add(T model);
        void Udate(T model);
        void DeleteById(object id);
        void Delete(T model);
        IQueryable<T> GetAllAsQueryable();
        IEnumerable<T> GetAll();
        T GetById(object id);
        IEnumerable<T> GetWhere(Expression<Func<T, bool>> predicate);
        // Better create a unit of work to commit an operations with many repositories
        void Save();
    }
}
