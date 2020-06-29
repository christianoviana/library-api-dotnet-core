using Library.API.Infrastructure.Entity;
using Library.API.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Library.API.Infrastructure.Repository
{
    public class RepositorySync<T> : IDisposable, IRepository<T> where T : class
    {
        protected ApplicationDbContext _context;

        public RepositorySync(ApplicationDbContext context)
        {
            this._context = context;
        }

        public void Add(T model)
        {
            if (model == null)
            {
                throw new ArgumentException("The model cannot be null");
            }

            this._context.Set<T>().Add(model);
        }

        public void DeleteById(object id)
        {
            T model = this._context.Set<T>().Find(id);

            if (model == null)
            {
                throw new ArgumentException($"The entity id={id} cannot be found");
            }
            else
            {
                this._context.Set<T>().Remove(model);
            }
        }

        public void Delete(T model)
        {
            if (model == null)
            {
                throw new ArgumentException($"The model cannot be null");
            }
                     
            this._context.Set<T>().Attach(model);
            this._context.Entry(model).State = EntityState.Deleted;
        }

        public void Udate(T model)
        {
            this._context.Set<T>().Attach(model);
            this._context.Entry(model).State = EntityState.Modified;
        }

        public IEnumerable<T> GetAll()
        {
           return this._context.Set<T>().ToList();
        }

        public IQueryable<T> GetAllAsQueryable()
        {
            return this._context.Set<T>().AsNoTracking();
        }

        public T GetById(object id)
        {
            return this._context.Set<T>().Find(id);
        }

        public T GetById(IQueryable<T> query)
        {
            return query.AsNoTracking().FirstOrDefault();
        }

        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return this._context.Set<T>().AsNoTracking().Where(predicate).ToList();
        }

        // Better create a unit of work to commit an operations with many repositories
        public void Save()
        {
            this._context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._context != null)
                {
                    this._context.Dispose();
                    this._context = null;
                }
            }
        }
    }
}
